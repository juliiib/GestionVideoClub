# Autenticación con JWT

Este documento explica cómo quedó armada la autenticación del proyecto: login con email y
password, passwords hasheados con BCrypt, y todos los endpoints (salvo el login) protegidos con
JWT. Complementa a [`PERSISTENCIA.md`](./PERSISTENCIA.md) (que tiene la sección 10, "Swagger").

## 1. El flujo completo

1. El cliente hace `POST /api/auth/login` con `{ "email": "...", "password": "..." }`.
2. `AuthController` (el único endpoint con `[AllowAnonymous]`) delega en `AuthService.Login`.
3. `AuthService` busca el usuario por email (`IUserRepository.GetByEmail`). Si no existe, o si
   `IPasswordHashService.Verify` dice que el password no coincide con el hash guardado, devuelve
   `null` → el controller responde `401 Unauthorized`.
4. Si coincide, `AuthService` le pide a `ITokenService.GenerateToken` un JWT firmado, con el id,
   el email y el rol del usuario (`"Client"` o `"Employee"`, según el nombre de la clase
   concreta). El controller responde `200 OK` con `{ "token": "..." }`.
5. El cliente guarda ese token y lo manda en cada request siguiente como
   `Authorization: Bearer <token>`.
6. **En Swagger**: arriba a la derecha hay un botón **"Authorize"**. Ahí se pega *solo* el token
   (sin escribir la palabra `Bearer`, Swashbuckle la agrega solo porque el `Scheme` configurado es
   `"bearer"`). Una vez autorizado, todos los `Try it out` de la página mandan ese header
   automáticamente.
7. Cada controller protegido (todos menos `AuthController`) tiene `[Authorize]` a nivel de clase.
   El middleware de autenticación valida el JWT en cada request; si falta, es inválido o venció,
   responde `401` antes de que el controller se ejecute.

## 2. Qué va en cada capa, y por qué

Se respetó la regla de dependencias que ya tenía el proyecto: Domain no conoce ningún paquete
externo, Application solo conoce las interfaces del Domain, y todo lo concreto vive en
Infrastructure (o en Presentation, cuando es configuración del propio servidor HTTP).

| Pieza | Capa | Por qué ahí |
|---|---|---|
| `Person.Email`, `Person.PasswordHash` | Domain (`Entities/Person.cs`) | Son datos del usuario, no de "cómo se autentica" — el dominio modela que toda `Person` tiene credenciales, sin saber qué algoritmo de hash o qué formato de token se usa. |
| `IUserRepository`, `IPasswordHashService`, `ITokenService` | Domain (`Interfaces/`) | Son los *contratos* que Application necesita para hacer login, pero las implementaciones (EF Core, BCrypt, JWT) son detalles de infraestructura. Así Application puede compilar y testearse sin saber que existe SQL Server, BCrypt o JWT. |
| `LoginRequest`, `LoginResponse`, `IAuthService`, `AuthService` | Application | La lógica de "¿este login es válido?" es una regla de negocio/aplicación: no le importa si el hash se verifica con BCrypt o con otra cosa, ni cómo se arma el token por dentro — solo usa las interfaces del Domain. |
| `BCryptPasswordHashService`, `JwtTokenService`, `JwtSettings`, `UserRepository` | Infrastructure (`ExternalServices/`, `Repositories/`) | Son las implementaciones concretas: la librería BCrypt.Net-Next, la librería de JWT de Microsoft, y el `DbContext`. Si el día de mañana se cambia BCrypt por Argon2, o JWT por otro esquema, solo se tocan estos archivos. |
| Paquete `Microsoft.AspNetCore.Authentication.JwtBearer`, el middleware (`AddAuthentication`/`AddJwtBearer`/`UseAuthentication`), `AuthController`, `[Authorize]`, la configuración de Swagger | Presentation | Es la aplicación ASP.NET Core la que decide *cómo* se expone la autenticación por HTTP (qué middleware usar, qué controller atiende el login, qué endpoints exige token). |

## 3. BCrypt: por qué nunca se guarda el password

El password en texto plano **nunca** se guarda en ningún lado. `BCryptPasswordHashService.Hash`
aplica el algoritmo BCrypt sobre el password, que produce un *hash* — una cadena que no se puede
revertir para recuperar el password original. BCrypt además incluye internamente una "sal"
(salt) aleatoria distinta en cada hash, así que dos usuarios con el mismo password terminan con
hashes distintos en la base, y nadie puede comparar hashes para adivinar quién usa la misma
contraseña que otro.

Para validar un login, `Verify(password, passwordHash)` no "desencripta" el hash: vuelve a
aplicar el mismo algoritmo (usando la sal que ya viene embebida en el hash guardado) sobre el
password que mandó el usuario, y compara el resultado. Si coincide, el password era el correcto.

Por eso `IPasswordHashService` tiene solo `Hash` y `Verify`, nunca un método que "recupere" el
password — esa operación no existe ni debería existir.

## 4. `TokenValidationParameters`: qué valida cada campo

Al llegar un request con `Authorization: Bearer <token>`, el middleware de JWT Bearer valida el
token contra estos parámetros antes de dejarlo pasar:

- **`ValidateIssuer` + `ValidIssuer`**: comprueba que el claim `iss` del token sea exactamente
  `"GestionVideoClub.Api"` — rechaza un token válido pero emitido por otro sistema.
- **`ValidateAudience` + `ValidAudience`**: comprueba que el claim `aud` sea
  `"GestionVideoClub.Clients"` — rechaza un token que era válido pero pensado para otra API.
- **`ValidateIssuerSigningKey` + `IssuerSigningKey`**: recalcula la firma HMAC-SHA256 del token
  con la misma clave simétrica (`Jwt:Key`) que se usó para firmarlo al generarlo, y la compara.
  Si alguien modificó el payload del token (por ejemplo, para cambiarse el rol a `"Employee"`),
  la firma ya no coincide y se rechaza.
- **`ValidateLifetime`**: comprueba que la fecha actual esté entre el claim `nbf` (si existiera) y
  `exp` — rechaza un token vencido.
- **`ClockSkew = TimeSpan.Zero`**: por defecto, .NET le da 5 minutos de margen a la validación de
  vencimiento (por si el reloj del servidor que emitió el token y el que lo valida no están
  perfectamente sincronizados). Poniendo `ClockSkew` en cero, un token vence exactamente a los
  `ExpirationMinutes` minutos, sin ese margen extra.

## 5. Por qué `UseAuthentication()` va antes de `UseAuthorization()`

El pipeline de middlewares de ASP.NET Core se ejecuta en el orden en que se registra.
`UseAuthentication()` es el que lee el header `Authorization`, valida el JWT y, si es válido,
construye el `ClaimsPrincipal` (quién es el usuario y qué rol tiene) y lo deja disponible en
`HttpContext.User`. `UseAuthorization()` es el que después decide si ese usuario puede entrar al
endpoint (por los atributos `[Authorize]`). Si `UseAuthorization()` se ejecutara primero, no
tendría ningún `ClaimsPrincipal` todavía para evaluar, y todo request terminaría rechazado como
anónimo sin importar el token que mandara.

## 6. TPC (Table-Per-Concrete-type)

Antes de este cambio, `Person` era abstracta y no tenía ningún `DbSet<Person>` propio, así que
Entity Framework, por descarte, ya mapeaba cada subtipo concreto (`Client`, `Employee`) a su
propia tabla con las columnas heredadas de `Person` incluidas. Pero para poder hacer
`context.Persons.FirstOrDefault(p => p.Email == email)` en `UserRepository` (buscar por email sin
saber de antemano si es un `Client` o un `Employee`) hace falta exponer `DbSet<Person> Persons`.

En el momento en que se expone un `DbSet` del tipo *base* de una jerarquía, EF Core deja de poder
asumir la estrategia anterior por descarte y, por convención, elegiría **TPH** (Table-Per-
-Hierarchy: una sola tabla `Persons` con todas las columnas de todos los subtipos juntas y una
columna discriminadora que dice si la fila es un `Client` o un `Employee`). Eso hubiera
significado migrar los datos existentes a una tabla nueva.

Por eso se configuró explícitamente:

```csharp
modelBuilder.Entity<Person>().UseTpcMappingStrategy();
```

**TPC (Table-Per-Concrete-type)** le dice a EF Core: "seguí usando una tabla por cada tipo
concreto (`Clients`, `Employees`), cada una con sus propias columnas (incluidas las heredadas de
`Person`), sin tabla `Persons` física y sin discriminador". `context.Persons` sigue funcionando
como una consulta LINQ (EF arma un `UNION ALL` entre `Clients` y `Employees` por debajo cuando
hace falta), pero no agrega ninguna tabla nueva. Por eso la migración `AddAuthentication` no tiene
ningún `DROP TABLE` ni recreación de `Clients`/`Employees`: solo agrega las columnas `Email` y
`PasswordHash` a cada tabla existente.

## 7. Usuario sembrado (seed)

La migración inserta un empleado fijo para poder loguearse la primera vez (si no, nadie podría
loguearse nunca, porque dar de alta clientes o empleados nuevos ya requiere estar autenticado):

- **Email**: `admin@videoclub.com`
- **Password**: `Admin123!`

El hash guardado (`$2a$11$2b/Af2TsTkMq5.WzDc5h3eRJ9gqIpjC6.UiFRD9Gy3gaTmnAUVqU6`) se generó
corriendo de verdad `BCrypt.Net.BCrypt.HashPassword("Admin123!")` — no se inventó a mano — y se
verificó con `BCrypt.Net.BCrypt.Verify("Admin123!", hash)` antes de pegarlo en la migración.

### Cómo probarlo

1. Aplicar la migración (ver el comando en la sección 9 de `PERSISTENCIA.md`; a propósito **no**
   se ejecutó todavía).
2. `POST /api/auth/login` con:
   ```json
   { "email": "admin@videoclub.com", "password": "Admin123!" }
   ```
3. Copiar el `token` de la respuesta.
4. En Swagger, click en **Authorize**, pegar el token, **Authorize** de nuevo y **Close**.
5. Cualquier endpoint (por ejemplo `GET /api/clients`) ya debería responder sin `401`.

## 8. Filas que ya existían antes de este cambio

Si en la base ya había `Clients` o `Employees` creados antes de agregar `Email`/`PasswordHash`
(como el cliente de prueba que se insertó al verificar la persistencia con EF Core), la migración
les asigna un email placeholder único basado en su propio `Id`
(`client-<id>@placeholder.local` / `employee-<id>@placeholder.local`) **antes** de crear el índice
único sobre `Email` — si no, el índice fallaría al encontrar más de una fila con el mismo valor
(`""`, el default de la columna nueva). Esas cuentas viejas no tienen password real, así que no
pueden loguearse hasta que alguien les actualice el email y el password a mano (hoy no hay un
endpoint para eso; se puede hacer con una actualización directa en la base, o agregando un
endpoint específico si se necesita).

## 9. Autorización por políticas

Hasta ahora `[Authorize]` solo exigía "tener un token válido", sin importar el rol. Esto agrega
una segunda capa: además de estar autenticado, el rol del token tiene que estar permitido para
ese endpoint puntual.

### Qué es una política y por qué se usa en vez de `[Authorize(Roles = "...")]`

Una *policy* de autorización es una regla con nombre, definida una sola vez en `Program.cs`
(`AddAuthorization`), que después se referencia por ese nombre desde cualquier controller o
acción. La alternativa más directa sería escribir el rol a mano en cada atributo:

```csharp
[Authorize(Roles = "Employee,Client")]
```

El problema es que esa cadena `"Employee,Client"` queda repetida, literal, en cada controller que
necesite esa misma combinación — si el día de mañana se agrega un tercer tipo de usuario, o se
decide que los clientes ya no pueden hacer algo que antes sí, hay que ir a buscar y cambiar esa
cadena en todos los lugares donde aparece, con el riesgo de olvidarse alguno.

Con una política, la regla "qué roles entran" se define **una sola vez**
(`Policies.EmployeesOnly`, `Policies.EmployeesAndClients`) con un nombre que describe *para qué
sirve*, no *qué roles son*. Los controllers solo dicen "necesito la política `EmployeesOnly`"
sin saber (ni importarles) qué roles hay detrás. Si la regla cambia, se cambia en un solo lugar
(`Program.cs`) y todos los controllers que usan esa política quedan actualizados automáticamente.

### Por qué `nameof(Employee)` / `nameof(Client)` y no `"Employee"` / `"Client"`

```csharp
options.AddPolicy(Policies.EmployeesOnly,
    policy => policy.RequireRole(nameof(Employee)));
```

El rol que viaja en el token es, literalmente, el nombre de la clase C# del usuario
(`user.GetType().Name`, ver sección 1). Usar `nameof(Employee)` en vez del string suelto
`"Employee"` ata esa comparación al nombre real de la clase del Domain: si alguna vez se
renombrara la entidad `Employee` a otra cosa, el compilador rompería esta línea (en vez de
quedar un string desincronizado y fallar en silencio en producción, dejando pasar o bloqueando a
todo el mundo sin que nadie se dé cuenta por qué).

### Tabla: controller/endpoint → política → quién entra

| Controller | Endpoint | Política | Quién entra |
|---|---|---|---|
| `AuthController` | `POST /api/auth/login` | `[AllowAnonymous]` | Cualquiera (sin token) |
| `EmployeesController` | todos | `EmployeesOnly` | Solo `Employee` |
| `ClientsController` | todos | `EmployeesOnly` | Solo `Employee` |
| `FormatsController` | todos | `EmployeesOnly` | Solo `Employee` |
| `MoviesController` | `GET`, `GET /{id}` | `EmployeesAndClients` (de la clase) | `Employee` o `Client` |
| `MoviesController` | `POST` | `EmployeesAndClients` + `EmployeesOnly` | Solo `Employee` |
| `CopiesController` | `GET`, `GET /{id}` | `EmployeesAndClients` (de la clase) | `Employee` o `Client` |
| `CopiesController` | `POST`, `PATCH /{id}/state`, `PATCH /{id}/availability` | `EmployeesAndClients` + `EmployeesOnly` | Solo `Employee` |
| `RentsController` | `GET`, `GET /{id}` | `EmployeesAndClients` (de la clase) | `Employee` o `Client` |
| `RentsController` | `POST`, `PATCH /{id}/return` | `EmployeesAndClients` + `EmployeesOnly` | Solo `Employee` |
| `FinesController` | `GET`, `GET /{id}` | `EmployeesAndClients` (de la clase) | `Employee` o `Client` |
| `FinesController` | `POST`, `PATCH /{id}/pay` | `EmployeesAndClients` + `EmployeesOnly` | Solo `Employee` |

Esto refleja la regla de negocio: el empleado gestiona todo; el cliente solo puede *consultar*
(`GET`) el catálogo (`Movies`), las copias (`Copies`), sus rentas (`Rents`) y sus multas
(`Fines`) — nunca crearlas, modificarlas ni pagarlas. `Clients` y `Formats` son pura gestión
administrativa, así que quedan cerrados para clientes incluso en los `GET`.

### Por qué las políticas de clase y de método se combinan con AND

En ASP.NET Core, cuando hay un `[Authorize]` (o `[Authorize(Policy = ...)]`) tanto en la clase
como en un método de esa clase, **no se usa solo el del método** — se exigen **las dos**. Por eso
en `MoviesController`, `CopiesController`, `RentsController` y `FinesController` la clase tiene
la política más *permisiva* (`EmployeesAndClients`) y los métodos de escritura tienen, además, la
más *restrictiva* (`EmployeesOnly`):

- Un `GET` solo tiene la política de la clase → entra `Employee` o `Client`.
- Un `POST`/`PATCH` tiene la política de la clase **y** la del método → tiene que cumplir
  `EmployeesAndClients` **y** `EmployeesOnly` al mismo tiempo. Como `EmployeesOnly` ya es más
  estricta que `EmployeesAndClients` (todo `Employee` cumple ambas, ningún `Client` cumple la
  segunda), el resultado neto es "solo entra `Employee`".

Por esto mismo nunca se le puso `EmployeesOnly` a la *clase* de un controller mixto: si la clase
tuviera `EmployeesOnly`, ya bloquearía a los clientes incluso en los `GET`, sin importar qué
política tuviera cada método — el AND entre clase y método nunca puede volverse *más permisivo*
que el más restrictivo de los dos.

### 401 vs 403

- **401 Unauthorized**: no hay token, el token está mal formado, está vencido, o la firma no
  valida (todo lo que chequea `TokenValidationParameters`, ver sección 4). El middleware de
  autenticación (`UseAuthentication`) ni siquiera logra identificar quién es el usuario.
- **403 Forbidden**: el token es válido — se sabe perfectamente quién es el usuario y qué rol
  tiene — pero ese rol no cumple la política exigida por el endpoint. Esto lo decide
  `UseAuthorization`, un paso después de la autenticación.

En criollo: 401 es "no sé quién sos", 403 es "sé quién sos, y no te deja entrar acá".

### Cómo probarlo

1. **Login como cliente** (usando un `Client` dado de alta con `POST /api/clients` por un
   `Employee` ya logueado, email y password reales):
   - `GET /api/movies` → **200** (política de la clase, `EmployeesAndClients`).
   - `POST /api/movies` → **403** (le falta además `EmployeesOnly`).
   - `GET /api/employees` → **403** (`EmployeesController` es `EmployeesOnly` desde la clase).
2. **Login como empleado** (`admin@videoclub.com` / `Admin123!`, el usuario sembrado):
   - `GET /api/movies` → **200**.
   - `POST /api/movies` → **201** (cumple las dos políticas).
   - `GET /api/employees` → **200**.
3. **Sin token, o con un token vencido/alterado** → cualquier endpoint protegido da **401**, antes
   de que la política llegue a evaluarse.
