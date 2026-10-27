# Datos de prueba: el seeder

Este documento explica cómo se cargan datos de prueba realistas en la base para poder probar la
API desde Swagger sin tener que dar de alta todo a mano (formatos, películas, copias, empleados,
clientes, alquileres y multas).

## 1. Por qué un seeder en runtime y no `HasData`

El proyecto ya usa `HasData` para una sola fila: el empleado admin (`VideoClubDbContext`,
sección 33-48), porque es un dato fijo que tiene que existir *siempre*, incluso en una base
recién creada, y EF lo versiona como parte de la migración.

Los datos de prueba de este documento son distintos: son muchas filas, relacionadas entre sí
(copias que apuntan a películas y formatos, alquileres que apuntan a copias/clientes/empleados,
multas que apuntan a alquileres), y la idea es poder "resetear y volver a probar" sin generar una
migración nueva cada vez que se agrega o cambia un dato de prueba. Por eso `DbSeeder.SeedAsync`
corre en runtime, al arrancar la aplicación — no en una migración.

## 2. Por qué usa los constructores del dominio

`DbSeeder` no asigna propiedades por reflection ni crea objetos "a mano" saltándose las reglas de
negocio. Cada entidad se crea con su constructor público (`new Movie(...)`, `new Rent(...)`, etc.)
y, cuando hace falta cambiar su estado después de creada, se usan los métodos de dominio
existentes: `copy.SetAvailability(...)`, `rent.ReturnCopy(...)`, `fine.PayFine()`. Así los datos de
prueba quedan sujetos a las mismas validaciones que cualquier dato que entre por la API — si
alguna regla de dominio cambia, el seeder se entera (falla al compilar o al correr) en vez de
quedar con datos inconsistentes.

## 3. Qué significa que sea idempotente

"Idempotente" acá quiere decir: correr el seeder dos veces (por ejemplo, reiniciar la app dos
veces en desarrollo) no duplica los datos ni tira una excepción. Se logra con una guarda al
principio del método:

```csharp
if (await context.Movies.AnyAsync())
{
    return;
}
```

Si ya hay al menos una película cargada, se asume que el seeder ya corrió antes y no hace nada
más. Todo el resto de la carga (formatos, copias, empleados, clientes, alquileres, multas) queda
protegido por esa única condición, porque en este dominio no tiene sentido que exista, por
ejemplo, una copia sin ninguna película.

## 4. Un solo `SaveChangesAsync`

Todas las entidades se agregan al `DbContext` con `Add`/`AddRange` y se persisten juntas con un
único `await context.SaveChangesAsync()` al final. Esto es todo o nada: si algo falla a mitad de
camino (por ejemplo, una validación de dominio), no queda la base en un estado parcial con, por
ejemplo, copias sin sus alquileres.

## 5. Por qué solo corre en `Development`

El seeder vive dentro del mismo bloque `if (app.Environment.IsDevelopment())` que ya configuraba
Swagger en `Program.cs`. Son datos ficticios (películas, clientes y alquileres inventados) que no
tienen sentido en un ambiente real — no se quiere que una base de producción recién migrada
termine con "Carlos Fernández" como cliente.

## 6. Por qué `CreateScope()`

`VideoClubDbContext` y `IPasswordHashService` están registrados como `Scoped` (`AddDbContext`,
`AddScoped`), lo que normalmente significa "una instancia por request HTTP". En el arranque de la
aplicación, antes de que `app.Run()` empiece a atender requests, no existe ningún request — y por
lo tanto no existe ningún scope abierto del que resolver esas dependencias. `app.Services.CreateScope()`
crea un scope manualmente para ese momento puntual; el `using` lo descarta (y con él, el
`DbContext` que usó) apenas termina de migrar y sembrar los datos.

## 7. Usuarios de prueba

| Email | Password | Rol |
|---|---|---|
| `admin@videoclub.com` | `Admin123!` | Employee (sembrado por `HasData`, no por el seeder) |
| `juan.perez@videoclub.com` | `Empleado123!` | Employee |
| `maria.gomez@videoclub.com` | `Empleado123!` | Employee |
| `carlos.fernandez@mail.com` | `Cliente123!` | Client |
| `lucia.martinez@mail.com` | `Cliente123!` | Client |
| `martin.gomez@mail.com` | `Cliente123!` | Client |
| `florencia.lopez@mail.com` | `Cliente123!` | Client |
| `diego.rodriguez@mail.com` | `Cliente123!` | Client |

Para loguearse: `POST /api/auth/login` con el email y password de la tabla, y usar el token
devuelto en el botón "Authorize" de Swagger.

## 8. Cómo resetear la base y volver a sembrar

```bash
dotnet ef database drop --force --project GestionVideoClub.Infrastructure --startup-project GestionVideoClub
```

y volver a correr la app (`dotnet run --project GestionVideoClub`, o F5). Al arrancar en
Development, `Program.cs` aplica las migraciones pendientes (`MigrateAsync`) y, como la base está
vacía, `DbSeeder.SeedAsync` vuelve a cargar todos los datos de prueba descriptos arriba.
