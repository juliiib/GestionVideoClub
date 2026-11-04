# DTOs de respuesta, códigos HTTP y dónde vive cada regla

Este documento explica tres correcciones de diseño/seguridad hechas antes de deployar: que
ningún controller devuelva entidades del dominio, que los códigos HTTP reflejen el error real
(409 vs. 404), y que la devolución de un alquiler sea una decisión del dominio, no del servicio.

## 1. Por qué ningún controller devuelve una entidad

Antes de este cambio, `RentsController`, `FinesController`, `CopiesController`,
`FormatsController` y los `GET` de `MoviesController` devolvían directamente `Rent`, `Fine`,
`Copy`, `Format` y `Movie`. Esto era un problema por tres motivos:

**a) Fuga de datos sensibles.** Por el `AutoInclude` configurado en `VideoClubDbContext`, serializar
un `Rent` serializa también su `Client` y su `Employee` completos — incluyendo `PasswordHash`,
`DNI` y `Salary`. Y como `GET /api/rents` está habilitado para `EmployeesAndClients`, **cualquier
cliente logueado podía ver el hash de contraseña de todos los usuarios del sistema** con solo
pedir sus propios alquileres. Lo mismo pasaba con `FinesController`, que serializaba la `Rent`
entera (y por lo tanto el `Client`/`Employee` de esa renta).

**b) Acoplar el contrato HTTP al modelo de persistencia.** Si una entidad devuelve exactamente lo
que el `DbContext` mapea, cualquier cambio interno (agregar una propiedad, cambiar un nombre,
agregar una navegación) rompe silenciosamente el contrato que consume el front o cualquier otro
cliente de la API, sin que nadie lo haya decidido a propósito.

**c) Ciclos de serialización.** `Rent.Fine` apunta a una `Fine` que apunta de vuelta a su `Rent`
(`Fine.Rent`); `Copy.Movie`/`Copy.Format` no tienen vuelta pero otras relaciones sí. Serializar
esas entidades directamente generaba ciclos, que se estaban tapando con:

```csharp
.AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles)
```

Eso es un parche: en vez de arreglar el ciclo (no exponer la entidad completa), simplemente le
decía al serializador "cuando encuentres una referencia repetida, saltéala". El resultado es un
JSON impredecible (a veces un campo aparece, a veces se omite silenciosamente porque ya se
visitó). Ahora que ningún controller serializa una entidad —solo DTOs planos, armados a mano con
exactamente los campos que se quieren exponer— no puede haber ciclos, y `IgnoreCycles` se sacó de
`Program.cs`.

### Los DTOs nuevos

Todos siguen el mismo patrón que ya existía en `ClientResponse`/`EmployeeResponse`: un `record`
con un método estático `From(entidad)`.

| DTO | Reemplaza a | Dato "aplanado" que agrega |
|---|---|---|
| `FormatResponse` | `Format` | — (ya era plano) |
| `CopyResponse` | `Copy` | `MovieName`, `FormatName` (en vez de los objetos `Movie`/`Format` completos) |
| `RentResponse` | `Rent` | `CopyInternalCode`, `MovieName`, `ClientFullName`, `ClientEmail`, `EmployeeFullName`, y `FineId` (nullable) **en vez de** la `Fine` completa |
| `FineResponse` | `Fine` | `ClientId`/`ClientFullName` (resueltos a través de `Fine.Rent.Client`) **en vez de** la `Rent` completa |
| `MovieResponse` | `Movie` | ya existía desde la integración con OMDb; ahora también lo usan los `GET` |

`RentResponse` expone `FineId` (un `Guid?`) en vez de la `Fine` entera a propósito: para saber si
un alquiler tiene multa alcanza con el id — si se necesitan los detalles de la multa, están en
`GET /api/fines/{id}`.

### Capas que cambiaron

Siguiendo la regla de que Application no devuelve tipos de Domain hacia afuera:
`ICopyService`, `IFormatService`, `IRentService`, `IFineService` e `IMovieService` ahora
devuelven los DTOs de respuesta (`CopyResponse`, `FormatResponse`, etc.) en vez de las entidades.
`IClientService`/`IEmployeeService` no se tocaron porque ya devolvían la entidad únicamente hacia
el controller, que ya hacía `ClientResponse.From(...)`/`EmployeeResponse.From(...)` antes de
responder — ahí el problema nunca estuvo en el controller, sino en los otros cinco.

Para que los `From()` tengan todos los datos que necesitan sin hacer una query extra, se agregó
`AutoInclude` para `Rent.Fine` en `VideoClubDbContext` (las demás navegaciones que usan los DTOs
ya tenían `AutoInclude`).

## 2. Códigos HTTP: por qué 409 y no 404

La regla general del proyecto es:

| Excepción | Código | Significa |
|---|---|---|
| `ArgumentException` | 400 Bad Request | Los datos que mandó el cliente son inválidos en sí mismos (independientemente de si el recurso existe) |
| `KeyNotFoundException` | 404 Not Found | El recurso que se pidió no existe |
| `InvalidOperationException` | 409 Conflict | El recurso **sí existe**, pero la operación pedida no es válida dado su estado actual |
| `HttpRequestException` / `TaskCanceledException` (solo en `MoviesController.Create`, hacia OMDb) | 503 Service Unavailable | No se pudo cumplir la operación porque una dependencia externa no respondió |

404 y 409 se confunden fácil, pero la diferencia importa: un 404 le dice al cliente "fijate el id,
puede que esté mal". Un 409 le dice "el id está bien, pero no podés hacer esto ahora" — son
errores de naturaleza distinta y un cliente de la API (o un frontend) reacciona distinto a cada
uno (por ejemplo, reintentar tiene sentido después de un 409 si cambia el estado, pero no después
de un 404).

Dos lugares tenían esto mal:

- **`FineService.PayFine`**: atrapaba la `InvalidOperationException` de `fine.PayFine()` (que la
  entidad tira si la multa ya estaba pagada) y devolvía `false` — y el controller respondía
  **404 "Fine not found"** aunque la multa existiera y solo estuviera ya pagada. Ahora el servicio
  deja pasar la excepción (solo devuelve `false` si la multa no existe), y el controller la
  atrapa y responde **409** con el mensaje real ("The fine has already been paid.").
- **`RentService.ReturnCopy`**: devolvía `false` tanto si la renta no existía como si ya no estaba
  activa, y el controller respondía 404 en los dos casos. Ahora solo devuelve `false` si la renta
  no existe (→ 404); si ya fue devuelta, `rent.Return(...)` tira `InvalidOperationException` (→
  409 en el controller).

## 3. `Rent.Return` decide el estado: por qué se movió al dominio

Antes, `RentService.ReturnCopy` decidía si el alquiler quedaba `Returned` u `Overdue` comparando
fechas, y se lo pasaba a `rent.ReturnCopy(estado, fecha)` como si fuera un dato externo. Esa es
una regla de negocio ("¿se devolvió a tiempo o tarde?"), no un detalle de orquestación — y las
reglas de negocio van en la entidad, no en el servicio que la orquesta.

Ahora `Rent.Return(DateTime returnDate)` concentra toda esa lógica:

- Si el alquiler no está `Active` → `InvalidOperationException("La renta ya fue devuelta.")`
  (no tiene sentido devolver dos veces el mismo alquiler).
- Si la fecha de devolución es anterior a la fecha de alquiler → `ArgumentException` (una fecha
  de devolución no puede estar en el pasado respecto de cuándo se alquiló).
- Decide el estado: `Overdue` si `returnDate > ExpectedReturnDate`, si no `Returned`.
- Libera la copia (`Copy.SetAvailability(true)`) — salvo que la copia esté marcada como `damaged`,
  en cuyo caso no tiene sentido volver a ofrecerla para alquilar hasta que alguien la revise.

`RentService.ReturnCopy` quedó reducido a buscar la renta (`false` si no existe) y delegar:
`rent.Return(request.ReturnDate)`. Ya no decide estados ni toca la disponibilidad de la copia —
eso es responsabilidad de la entidad.

`DbSeeder` se actualizó para usar `rent.Return(fecha)` en vez del viejo `ReturnCopy(estado, fecha)`
+ `SetAvailability(true)` manual; las dos rentas devueltas a tiempo y las dos tarde siguen
quedando con el estado correcto (`Returned`/`Overdue`) porque ahora lo calcula el propio dominio.

## 4. El empleado sale del token, no del body

Antes, `CreateRentRequest` incluía un `EmployeeId` que mandaba quien llamara a la API — es decir,
un cliente (o cualquiera con un token válido de empleado) podía registrar un alquiler
"a nombre de" cualquier otro empleado. El empleado que registra un alquiler es, por definición,
el que está logueado haciendo la operación.

- Se sacó `EmployeeId` de `CreateRentRequest`.
- `RentsController.Create` obtiene el id del empleado del claim de identidad del token
  (`ClaimTypes.NameIdentifier`, con `JwtRegisteredClaimNames.Sub` como respaldo, porque el handler
  de JWT bearer mapea el claim `"sub"` del token a `ClaimTypes.NameIdentifier` por convención). Si
  no puede resolverse un `Guid` válido de ahí, responde `401` — aunque en la práctica no debería
  pasar nunca, porque el endpoint ya exige `[Authorize]`.
- `RentService.AddRent` ahora recibe `(CreateRentRequest request, Guid employeeId)` en vez de leer
  el id del request.

## 5. Verificación de que no queda ningún `PasswordHash` expuesto

Se revisó Presentation y Application: ningún controller devuelve `Person`, `Client`, `Employee`,
`Rent`, `Fine`, `Copy`, `Format` ni `Movie` directamente — todos pasan por un DTO de respuesta
(`ClientResponse`, `EmployeeResponse`, `CopyResponse`, `FormatResponse`, `RentResponse`,
`FineResponse`, `MovieResponse`). Ninguno de esos DTOs tiene un campo `PasswordHash`.
