# Persistencia con EF Core + SQL Server

Este documento explica cómo quedó armada la persistencia del proyecto después de pasar de
repositorios en memoria (`List<T>`) a Entity Framework Core sobre SQL Server, y por qué cada
pieza está donde está.

## 1. Paquetes y por qué van en esa capa

| Paquete | Proyecto | Por qué ahí |
|---|---|---|
| `Microsoft.EntityFrameworkCore.SqlServer` (10.0.13) | `GestionVideoClub.Infrastructure` | Es el único lugar que conoce "cómo" se persiste. El `DbContext`, los repositorios y el proveedor de SQL Server viven todos en Infrastructure. |
| `Microsoft.EntityFrameworkCore.Design` (10.0.13, `PrivateAssets=all`) | `GestionVideoClub` (Presentation) | Las herramientas de diseño (`dotnet ef migrations add`, scaffolding) necesitan un proyecto "ejecutable" (el *startup project*) para poder construir el `IServiceProvider` y leer la configuración. Por eso el paquete va en Presentation, no en Infrastructure. `PrivateAssets=all` evita que esta dependencia de herramientas se filtre a quien consuma el proyecto. |

**Domain y Application no referencian EF Core.** Siguen sin saber que existe una base de datos:
Domain define las entidades y las interfaces de repositorio (contratos), Application las usa a
través de esas interfaces. Esto es "persistence ignorance": si el día de mañana se cambia EF Core
por Dapper o Mongo, Domain y Application no se tocan — solo Infrastructure.

## 2. Connection string

En `GestionVideoClub/appsettings.json`:

```json
"ConnectionStrings": {
  "GestionVideoClubDb": "Server=localhost;Database=GestionVideoClub;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=True"
}
```

- `Trusted_Connection=True`: usa autenticación de Windows (no hace falta usuario/contraseña en el
  connection string).
- `TrustServerCertificate=True`: evita el error de certificado SSL no confiable en desarrollo
  local (sin esto, el driver rechaza la conexión si SQL Server no tiene un certificado válido
  firmado por una CA reconocida).
- `MultipleActiveResultSets=True`: permite tener más de un `DbCommand` activo sobre la misma
  conexión al mismo tiempo, que EF Core necesita en ciertos escenarios de carga de navegaciones.

En `appsettings.Development.json` se repite la misma connection string (por si en desarrollo se
quiere apuntar a algo distinto el día de mañana) y además se agrega:

```json
"Microsoft.EntityFrameworkCore.Database.Command": "Information"
```

Esto hace que, solo en Development, se vea en la consola el SQL real que genera cada operación
de EF Core — útil para aprender/depurar, pero demasiado ruido para producción.

## 3. Cambios en las entidades del Domain

- **`Guid Id` en vez de `int ID` con contador estático**: el contador `nextID` era un truco para
  simular autoincremento en memoria; con una base de datos real no hace falta, y usar `Guid`
  evita colisiones de ID entre inserts concurrentes sin depender de que la base asigne el valor
  (el ID se genera en el dominio, antes de guardar).
- **Todas las propiedades pasan a `{ get; private set; }`**: con listas en memoria no importaba,
  pero EF Core necesita poder "materializar" la entidad reconstruyendo sus propiedades al leerla
  de la base. Con `private set` la propiedad sigue siendo de solo lectura desde afuera del
  dominio, pero EF (que usa reflection/backing fields) puede setearla.
- **Constructor sin parámetros (`private`, o `protected` en `Person` por ser abstracta)**: EF Core
  necesita poder instanciar la entidad sin pasar por las validaciones del constructor de negocio
  (si no, fallaría al leer una fila existente). Al ser `private`/`protected`, nadie fuera de EF
  puede crear una entidad "vacía" por accidente. Los strings se inicializan en `string.Empty` y
  las navegaciones (`Movie`, `Format`, `Copy`, `Client`, `Employee`, `Rent`) en `null!`: ese `!`
  le dice al compilador "sé que esto es null ahora, pero EF lo va a llenar antes de que se use" —
  si no, el proyecto (que tiene `Nullable` habilitado) tiraría warnings por todos lados.
- **Claves foráneas explícitas** (`Copy.MovieId`/`FormatId`, `Rent.CopyId`/`ClientId`/`EmployeeId`,
  `Fine.RentId`): EF Core puede inferir FKs solo a partir de la navegación, pero tenerlas como
  propiedad explícita permite filtrar/comparar por el ID sin tener que cargar la entidad
  relacionada completa, y hace mas claro el modelo relacional en el código.
- **`Rent.Fine` pasa a `Fine?`**: antes era `Fine` no-nullable pero nunca se asignaba en el
  constructor (de hecho generaba un warning del compilador: "sale del constructor sin valor").
  Una renta recién creada legítimamente no tiene multa todavía, así que lo correcto es que sea
  opcional.
- **`Rent.ReturnDate` pasa a `DateTime?`**: antes quedaba en `default(DateTime)` (01/01/0001)
  mientras la renta estaba activa, lo cual es un valor "mentira". `null` representa correctamente
  "todavía no se devolvió".
- **`Person` no tiene `DbSet<Person>` propio**: al no exponer un `DbSet<Person>` en el
  `DbContext` (solo `DbSet<Client>` y `DbSet<Employee>`), EF Core usa la estrategia TPH/TPT por
  omisión para jerarquías sin `DbSet` de la base: como cada subtipo concreto tiene su propio
  `DbSet`, EF crea una tabla `Clients` y una tabla `Employees`, cada una con las columnas de
  `Person` más las propias. Es la convención más simple para este caso (no hay instancias de
  `Person` "pelado", solo de sus hijos).

## 4. El `DbContext`

`GestionVideoClub.Infrastructure/Persistence/VideoClubDbContext.cs` expone los `DbSet` como
`=> Set<T>()` (propiedad calculada) en vez de campos asignados en el constructor: es la forma
recomendada actual, evita tener que inicializarlos a mano y funciona igual con el constructor que
recibe `DbContextOptions<VideoClubDbContext>` desde la inyección de dependencias.

En `OnModelCreating` solo se configura lo que la convención de EF Core no puede deducir sola:

- **`Rent` 1 a 1 con `Fine`**: `HasOne(r => r.Fine).WithOne(f => f.Rent).HasForeignKey<Fine>(f => f.RentId)`.
  Por convención EF no puede adivinar solo con las navegaciones si es 1 a 1 o 1 a muchos cuando
  hay una FK explícita de un lado; hay que decírselo explícitamente y aclarar en qué entidad vive
  la FK (en `Fine`, no en `Rent`).
- **`Rent -> Client`, `Rent -> Employee`, `Rent -> Copy` con `OnDelete(DeleteBehavior.Restrict)`**:
  por convención EF Core pone `Cascade` en FKs no-nulas, lo que significaría que borrar un
  cliente, un empleado o una copia borraría en cascada su historial de alquileres. Eso no es lo
  que se quiere: el historial se mantiene, y si hace falta borrar algo que todavía tiene alquileres
  asociados, que falle explícitamente en vez de borrar datos en cascada.
- **`Copy -> Movie`, `Copy -> Format`**: configuración simple de FK (una película/formato puede
  tener muchas copias). No se restringe el delete acá porque no lo pidió el enunciado, pero se
  deja la relación explícita por claridad.
- **`AutoInclude()`** en `Copy.Movie`, `Copy.Format`, `Rent.Copy`, `Rent.Client`, `Rent.Employee`,
  `Fine.Rent`: estas navegaciones se necesitan casi siempre que se consulta la entidad principal
  (por ejemplo, no tiene sentido devolver un `Rent` sin saber de qué `Copy` o `Client` se trata).
  `AutoInclude` hace que EF las traiga automáticamente sin tener que escribir `.Include(...)` en
  cada repositorio.
- **`HasPrecision(18, 2)`** en `Employee.Salary` y `Fine.Amount`: EF Core mapea `decimal` sin
  precisión/escala explícitas con una advertencia, porque SQL Server necesita saber de antemano
  cuántos dígitos totales y cuántos decimales va a guardar la columna. `(18, 2)` es el estándar
  para valores monetarios (hasta 16 dígitos enteros + 2 decimales).

## 5. Por qué se eliminaron los métodos `Update*` de los repositorios

Con listas en memoria, "actualizar" significaba reemplazar el objeto en la lista a mano
(`list[list.IndexOf(x)] = x`). Con EF Core esto ya no hace falta: el `DbContext` tiene un
*change tracker* que, mientras la entidad fue leída con ese mismo contexto (como pasa siempre
acá, porque el repositorio y el contexto son `Scoped` y viven durante el mismo request), detecta
solo qué propiedades cambiaron cuando se llama a `SaveChanges()`. Por eso los repositorios quedaron
solo con `Add*`, `GetAll*`, `GetById`, `Remove*` (cuando existía) y `SaveChanges()`.

## 6. Dónde va `SaveChanges()`

`SaveChanges()` se llama una vez en cada **servicio de Application**, al final de cada operación
de escritura (alta, baja, `PayFine`, `ReturnCopy`, las actualizaciones de Client/Employee/Copy).
No se llama dentro del repositorio en cada método `Add*`/`Remove*`: el repositorio solo registra
el cambio en el `DbContext` (`Add`, `Remove`, o simplemente nada si fue una modificación de
propiedades, porque el change tracker ya la vio). Así, si un día una operación necesita tocar más
de una entidad (como `AddRent`, que además marca la copia como no disponible), todo se guarda en
un solo `SaveChanges()` — una sola transacción implícita — en vez de uno por cada entidad tocada.

## 7. Por qué todo es `Scoped`

`AddDbContext` registra el `DbContext` como `Scoped` por defecto, y tiene que ser así: un
`DbContext` no es thread-safe y guarda el change tracker de ese request puntual, así que
compartirlo entre requests (`Singleton`) corrompería datos entre usuarios distintos, y crear uno
nuevo por cada inyección (`Transient`) rompería el change tracking entre repositorios del mismo
request (por ejemplo, `RentService` usa cuatro repositorios distintos en la misma operación; si
cada uno tuviera su propio `DbContext`, modificar una `Copy` desde `ICopyRepository` no se vería
reflejado en el `DbContext` que usa `IRentRepository`). Por eso los repositorios y los servicios
que dependen de ellos también quedaron todos como `Scoped`: viven y mueren con el mismo request
que el `DbContext`.

## 8. Lógica de negocio que se ajustó al pasar a EF

- **`RentService.AddRent`**: ahora valida que `Copy.IsAvailable` sea `true` antes de crear el
  alquiler (si no, `InvalidOperationException` → 409 Conflict) y marca la copia como no
  disponible (`copy.SetAvailability(false)`) en la misma operación. No se crea ninguna multa
  automáticamente en el alta.
- **`RentService.ReturnCopy`**: además de cambiar el estado del alquiler (`Returned` u
  `Overdue`, según si se pasó la fecha esperada), vuelve a marcar la copia como disponible
  (`copy.SetAvailability(true)`). Si la devolución llega tarde, no se genera ninguna multa
  automáticamente — eso queda como una operación separada contra `FinesController`.
- **Excepciones**: cuando un servicio no encuentra una entidad relacionada que necesita para
  crear otra (por ejemplo, el `MovieId` de una `Copy`, o el `RentId` de una `Fine`), tira
  `KeyNotFoundException` en vez de `ArgumentException`. La idea es separar "el dato que me
  mandaron está mal formado" (`ArgumentException`, validación del propio constructor de la
  entidad → 400) de "el dato está bien formado pero lo que referencia no existe"
  (`KeyNotFoundException` → 404).
- **Updates parciales**: `UpdateClientRequest`, `UpdateEmployeeContactRequest` y
  `UpdateEmployeeJobDetailsRequest` tienen todos sus campos opcionales (`string?`, `decimal?`).
  Antes el servicio llamaba a los métodos de dominio sin chequear si venían en `null`, lo cual
  rompía con `UpdateEmployeeJobDetailsRequest.Salary.Value` si `Salary` era `null`. Ahora cada
  campo se actualiza solo si vino distinto de `null`.

## 9. Comandos de migración

Ya se generó la migración `Inicial` en
`GestionVideoClub.Infrastructure/Persistence/Migrations/`. **No se ejecutó** `database update`
(no se tocó ninguna base real). Para aplicarla:

**Con `dotnet ef` (CLI):**

```bash
dotnet ef database update --project GestionVideoClub.Infrastructure --startup-project GestionVideoClub
```

Si en algún momento hace falta regenerar la migración desde cero (por ejemplo, después de otro
cambio de modelo):

```bash
dotnet ef migrations add NombreDeLaMigracion --project GestionVideoClub.Infrastructure --startup-project GestionVideoClub --output-dir Persistence/Migrations
```

**Con la Package Manager Console (Visual Studio)**, seleccionando `GestionVideoClub` como
"Default project":

```powershell
Add-Migration Inicial -Project GestionVideoClub.Infrastructure -StartupProject GestionVideoClub -OutputDir Persistence/Migrations
Update-Database -Project GestionVideoClub.Infrastructure -StartupProject GestionVideoClub
```

## 10. Swagger

Se reemplazó `Microsoft.AspNetCore.OpenApi` (`AddOpenApi()`/`MapOpenApi()`) por Swagger con
Swashbuckle, que es lo que usa el proyecto de referencia.

- **Paquete**: `Swashbuckle.AspNetCore` (10.2.3) va en `GestionVideoClub` (Presentation), igual
  que `Microsoft.EntityFrameworkCore.Design` — son paquetes que solo le importan a la aplicación
  que arranca el servidor HTTP, no a Domain/Application/Infrastructure.
- **`AddSwaggerGen()`**: inspecciona los controllers (vía `AddEndpointsApiExplorer()`, que
  registra el explorador de endpoints de minimal APIs/MVC) y genera en memoria la especificación
  OpenAPI del API — rutas, verbos, DTOs de entrada/salida — sin tocar código de los controllers.
- **`UseSwagger()`**: expone esa especificación generada como JSON en `/swagger/v1/swagger.json`.
- **`UseSwaggerUI()`**: sirve la interfaz web interactiva en `/swagger`, que lee ese JSON y
  permite probar cada endpoint desde el navegador.
- **Por qué solo en `Development`**: no tiene sentido (y es una superficie de exposición
  innecesaria) publicar la documentación interactiva de todo el API en un ambiente de
  producción; por eso ambas llamadas quedan dentro del mismo `if (app.Environment.IsDevelopment())`
  que ya existía.
- **`launchSettings.json`**: los perfiles `http` y `https` tienen `"launchBrowser": true` y
  `"launchUrl": "swagger"`, así que al correr el proyecto (F5 o `dotnet run`) se abre
  automáticamente el navegador en `/swagger` en vez de quedar en blanco.
