# Integración con una API externa: OMDb

Este documento explica cómo se integró [OMDb](https://www.omdbapi.com/) (Open Movie Database)
para completar los datos de una película al darla de alta, siguiendo el mismo patrón de capas
que el resto del proyecto.

## 1. Qué es OMDb y para qué se usa acá

OMDb es una API pública gratuita (con límite de requests) que, dado un título de película, devuelve
sus datos: género, duración, año, sinopsis, afiche, etc. En este proyecto se usa para que el
empleado, al cargar una película nueva, **no tenga que escribir a mano** nombre, género, duración
y año — solo manda el título (y opcionalmente el año, para desambiguar remakes/franquicias) y la
clasificación por edad (que es un dato propio del videoclub, no de OMDb). El resto lo completa la
consulta a OMDb.

## 2. Qué va en cada capa, y por qué

| Pieza | Capa | Por qué ahí |
|---|---|---|
| `MovieInfo` (record) | Domain (`Models/`) | Es el dato que trae OMDb, ya resuelto — no es una entidad que se persista, por eso vive en `Models/` y no en `Entities/`. El dominio lo necesita para poder construir un `Movie`, pero no sabe de dónde vino ese dato. |
| `IMovieInfoService` | Domain (`Interfaces/`) | Define *qué* se necesita ("buscar una película por título") sin saber *cómo* se consigue (HTTP, JSON, qué URL, qué API key). Así Application puede depender de esto sin saber que existe OMDb. |
| `IMovieService.AddMovieAsync`, `MovieService` | Application | Orquesta la regla de negocio: le pide el dato a `IMovieInfoService`, valida que no esté duplicado, y recién ahí construye y persiste el `Movie`. La entidad de dominio nunca hace I/O — es `MovieService` el que resuelve los datos externos *antes* de llamar al constructor de `Movie`. |
| `OmdbSettings`, `OmdbMovieInfoService` | Infrastructure (`ExternalServices/`) | La implementación concreta: `HttpClient`, la URL de OMDb, el formato de su JSON, el parseo de sus rarezas (`"136 min"`, `"2005–2010"`, `"N/A"`). Si el día de mañana se cambia OMDb por otra API de películas, solo se reescribe este archivo — `IMovieInfoService` y todo lo que lo usa queda igual. |
| Registro de `AddHttpClient`, sección `Omdb` en `appsettings.json`, validación de la `ApiKey` al arrancar | Presentation (`Program.cs`) | Es la aplicación la que decide cómo se configura el `HttpClient` (base address, timeout) y de dónde sale la configuración (`appsettings.json` + user-secrets). |

## 3. Por qué `AddHttpClient<T>` y no `new HttpClient()`

Crear un `HttpClient` a mano con `new HttpClient()` en cada request (o guardarlo como campo
estático sin cuidado) tiene problemas conocidos: agotamiento de sockets si se crea uno por
request, o DNS "congelado" si se reutiliza uno por mucho tiempo sin el manejo correcto. El
método `AddHttpClient<TClient, TImplementation>()` del paquete `Microsoft.Extensions.Http`
resuelve esto por abajo: usa un pool de `HttpMessageHandler` que rota para evitar el problema de
DNS, y además registra `OmdbMovieInfoService` en el contenedor de DI con un `HttpClient` ya
inyectado y configurado (`BaseAddress`, `Timeout`) — no hace falta instanciar nada a mano ni en
`Program.cs` ni en el propio servicio.

## 4. Por qué el timeout

```csharp
client.Timeout = TimeSpan.FromSeconds(10);
```

Sin un timeout explícito, `HttpClient` usa 100 segundos por defecto. Si OMDb está lento o
caído, un request de alta de película quedaría "colgado" ese tiempo largo antes de fallar,
bloqueando el hilo del request HTTP de nuestra propia API mucho más de lo razonable. Con 10
segundos, si OMDb no responde a tiempo, se corta antes y el usuario recibe un error rápido
(ver sección 6: ese timeout termina devolviendo `503`).

## 5. Por qué las clases del JSON son privadas

`OmdbResponse` (con sus `[JsonPropertyName("...")]` apuntando a los nombres exactos que usa OMDb:
`"Response"`, `"Title"`, `"imdbID"`, etc.) es una clase **privada**, anidada dentro de
`OmdbMovieInfoService`. Es el "contrato roto" entre OMDb y este archivo: nadie fuera de
`OmdbMovieInfoService` necesita saber que OMDb llama a la duración `"Runtime"` y la manda como el
string `"136 min"`, o que el año puede venir como `"2005–2010"`. Afuera de este archivo, lo único
que existe es `MovieInfo` — ya parseado, ya limpio, con tipos de C# normales (`int`, `string?`).
Si OMDb cambiara el formato de su JSON mañana, el único archivo que se tocaría es este.

## 6. El detalle de que OMDb responde 200 con `"Response": "False"`

A diferencia de la mayoría de las APIs REST, cuando OMDb no encuentra una película **no** responde
con un código HTTP de error (404, etc.) — responde `200 OK` con un cuerpo como:

```json
{ "Response": "False", "Error": "Movie not found!" }
```

Por eso `GetFromJsonAsync` nunca lanza una excepción en ese caso (desde el punto de vista HTTP, el
request fue exitoso). `OmdbMovieInfoService.SearchByTitleAsync` tiene que mirar explícitamente el
campo `"Response"` del body: si no es `"True"`, devuelve `null` — y es `MovieService` el que
decide qué hacer con ese `null` (convertirlo en `KeyNotFoundException` → `404`, ver sección 7).

## 7. Por qué 503 cuando falla la consulta a OMDb

Hay dos situaciones completamente distintas, y el controller las trata distinto:

- **OMDb respondió, pero no tiene esa película** → `SearchByTitleAsync` devuelve `null` →
  `MovieService.AddMovieAsync` tira `KeyNotFoundException` → el controller responde **404**: el
  recurso que se pidió (esa película) no existe.
- **No se pudo hablar con OMDb en absoluto** (está caído, no hay red, se cortó por el timeout de
  10 segundos) → `HttpRequestException` o `TaskCanceledException` (el timeout se manifiesta como
  cancelación) → el controller responde **503 Service Unavailable**: el problema no es que la
  película no exista, es que *no se pudo averiguar* — la dependencia externa no está disponible
  en este momento. Devolver 404 en ese caso sería mentir ("no existe") cuando en realidad es
  "no sé".

## 8. Configuración de la API key

La clave de OMDb (`Omdb:ApiKey`) **nunca** se escribe en texto plano en ningún archivo del
repositorio — en `appsettings.json` queda a propósito vacía:

```json
"Omdb": { "BaseUrl": "https://www.omdbapi.com/", "ApiKey": "" }
```

### En desarrollo: `dotnet user-secrets`

```bash
dotnet user-secrets set "Omdb:ApiKey" "<tu-key-real>" --project GestionVideoClub
```

Esto guarda la clave en un archivo *fuera* del repositorio (en el perfil del usuario, fuera de la
carpeta del proyecto), asociado al `UserSecretsId` del `.csproj`. En desarrollo, `IConfiguration`
lee esa clave y sobrescribe el valor vacío de `appsettings.json` — no hace falta tocar ningún
archivo versionado.

Si la clave sigue vacía al arrancar la aplicación (no se configuró ningún secreto), `Program.cs`
tira `InvalidOperationException` con el mismo mensaje que indica cómo configurarla, para que el
error sea inmediato y explícito en vez de fallar más adelante con un 401/403 confuso de OMDb.

### En producción: variable de entorno

ASP.NET Core traduce automáticamente las variables de entorno con `__` (doble guion bajo) como
separador de sección. Así, en producción (sin `user-secrets`, que es solo para desarrollo local),
la misma clave se configura con:

```bash
Omdb__ApiKey=<tu-key-real>
```

(el nombre completo de la sección más la propiedad, reemplazando cada `:` por `__`).

## 9. Cómo probarlo desde Swagger

1. Configurar la clave real con `dotnet user-secrets set "Omdb:ApiKey" "..." --project GestionVideoClub`
   (se necesita una cuenta gratuita en omdbapi.com para conseguir una).
2. Loguearse como empleado (`POST /api/auth/login` con `admin@videoclub.com` / `Admin123!`) y
   autorizar en Swagger con el token.
3. `POST /api/movies` con:
   ```json
   { "title": "Inception", "year": 2010, "clasification": 13 }
   ```
4. Debería responder `201 Created` con los datos resueltos desde OMDb: nombre, género, duración en
   minutos, año, sinopsis y afiche, además del `ImdbId` y la clasificación que mandó el empleado.
5. Repetir el mismo `POST` → ahora responde `409 Conflict` ("La película ya está cargada."),
   porque ya existe una película con ese mismo `ImdbId`.
6. `POST /api/movies` con un título inventado (`{ "title": "Esta Pelicula No Existe Zzz" }`) →
   `404 Not Found`.
