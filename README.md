# TimezoneCli

  Laboratorio minimo para una CLI en `.NET 8` ejecutada dentro de contenedores.

  Hoy el repositorio contiene:

  - una aplicacion de consola en `src/TimezoneCli`
  - un proyecto de pruebas en `tests/TimezoneCli.Tests`
  - un `Makefile` como interfaz estable del proyecto
  - un contenedor con `dotnet/sdk:8.0` para no instalar `.NET` en el host

  ## Que hace este proyecto

  Este repositorio contiene una CLI en `.NET 8` para consultar la fecha y hora
  local actual de un lugar y compararla con otro lugar al planear reuniones
  entre zonas horarias.

  El flujo del laboratorio esta pensado para que:

  - el codigo viva en este repo
  - el SDK de `.NET` viva dentro del contenedor
  - las dependencias se restauren desde el contenedor
  - las pruebas y validaciones se ejecuten con comandos `make`
  - la logica de dominio quede separada de la entrada/salida de consola
  - las entradas invalidas fallen con mensajes claros y codigos distintos de cero

  La entrada canonica de v1 son identificadores IANA como
  `America/Mexico_City`. Tambien existe una lista local y versionada de aliases
  soportados en `src/TimezoneCli/Data/place-aliases.json`.

  ## Requisitos

  Necesitas tener disponibles en el host:

  - Docker
  - Docker Compose
  - `make`

  No necesitas instalar `.NET`.

  ## Importante

  Este laboratorio **no instala `.NET` en el host**.

  El SDK y los comandos de `dotnet` se ejecutan dentro del contenedor definido en
  `compose.yaml` y `Dockerfile`.

  ## Uso rapido

  Desde la raiz del repo:

  ```bash
  make install
  make dev
  make test
  make lint
  make stop
  ```

  ## Instalar dependencias del laboratorio

  Restaura las dependencias NuGet del proyecto dentro del contenedor:

  ```bash
  make install
  ```

  Esto ejecuta dotnet restore dentro del servicio app.

  ## Correr la aplicacion

  Ejecuta la CLI desde el contenedor:

  ```bash
  make dev
  ```

  Para pasar argumentos a la CLI, usa Docker Compose directamente:

  ```bash
  docker compose run --rm app dotnet run --project src/TimezoneCli -- --place America/Mexico_City
  ```

  Comparacion entre dos lugares:

  ```bash
  docker compose run --rm app dotnet run --project src/TimezoneCli -- --place America/Mexico_City --compare Europe/London
  ```

  La comparacion usa horario laboral local de lunes a viernes, desde `09:00`
  inclusive hasta antes de `17:00`, y muestra la ventana aplicada:
  `Working hours window: 09:00-17:00`.

  Para configurar una ventana de horario laboral por comando, proporciona ambos
  limites en formato estricto `HH:mm`:

  ```bash
  docker compose run --rm app dotnet run --project src/TimezoneCli -- --place America/Mexico_City --compare Europe/London --working-hours-start 08:30 --working-hours-end 16:45
  ```

  Las opciones `--working-hours-start` y `--working-hours-end` solo son validas
  juntas y cuando se usa `--compare`. La hora final debe ser posterior a la hora
  inicial; las ventanas nocturnas no se soportan en esta version.

  Aliases soportados en v1:

  - `mexico city` -> `America/Mexico_City`
  - `london` -> `Europe/London`
  - `new york` -> `America/New_York`
  - `tokyo` -> `Asia/Tokyo`

  Codigos de salida:

  - `0`: exito
  - `1`: entrada invalida
  - `2`: entrada no soportada, incluidos codigos postales mexicanos
  - `3`: lugar desconocido
  - `4`: entrada ambigua

  Los codigos postales mexicanos, por ejemplo `01000`, no se resuelven en v1.
  La CLI falla explicitamente y sugiere usar un timezone IANA.

  ## Correr pruebas

  Ejecuta las pruebas automatizadas del repositorio:

  ```bash
  make test
  ```

  Esto corre dotnet test dentro del contenedor.

  ## Validar formato y estilo

  Verifica el formato del codigo sin modificar archivos:

  ```bash
  make lint
  ```

  Esto ejecuta:

  ```bash
  dotnet format --verify-no-changes
  ```

  ## Reglas de desarrollo

  - Usa `make` como interfaz estable del proyecto.
  - Mantén el SDK de `.NET` dentro del contenedor; no instales `.NET` en el host.
  - Cubre cada comportamiento terminado con pruebas automatizadas.
  - Mantén la logica de dominio separada de argumentos, stdout, stderr y codigos de salida.
  - Antes de cerrar cambios, ejecuta `make test` y `make lint`.

  ## Parar el entorno

  Detiene el entorno de Compose sin borrar volumenes:

  ```bash
  make stop
  ```

  ## Comandos disponibles

  ```bash
  make install
  make dev
  make test
  make lint
  make shell
  make stop
  make clean
  ```

  ## Estructura del proyecto

  src/TimezoneCli/               Aplicacion de consola
  tests/TimezoneCli.Tests/       Pruebas automatizadas
  Dockerfile                     Imagen base con .NET SDK 8
  compose.yaml                   Servicio del laboratorio
  Makefile                       Comandos estables del proyecto
  global.json                    Version del SDK esperada
  Directory.Build.props          Propiedades comunes de los proyectos

  ## Notas

  - TargetFramework del proyecto y de las pruebas: net8.0
  - make clean retira contenedores huerfanos, pero no elimina volumenes
  - el cache de NuGet se conserva en el volumen nuget_cache
