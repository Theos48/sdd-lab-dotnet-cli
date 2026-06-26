# AGENTS.md

  ## Proposito

  Este repositorio es un laboratorio minimo para una CLI en `.NET 8`.

  Su objetivo es construir, ejecutar y verificar el proyecto dentro de
  contenedores, sin instalar runtimes de proyecto en el host Fedora.

  ## Realidad del proyecto

  Toma el repositorio como fuente de verdad.

  Empieza por leer:
  - `README.md` para el uso humano del proyecto
  - `Makefile` para el flujo soportado
  - `compose.yaml` y `Dockerfile` para el entorno de ejecucion
  - `global.json`, `Directory.Build.props` y `*.csproj` para la configuracion de `.NET`

  No asumas ASP.NET, base de datos ni servicios extra si no existen realmente en
  el repo.

  ## Reglas operativas

  - Usa `make` como interfaz principal del proyecto.
  - Prefiere los comandos del proyecto sobre comandos ad hoc del host.
  - No instales un SDK global de `.NET` en el host.
  - No dependas de `dotnet` del host si la tarea puede ejecutarse dentro del contenedor.
  - No modifiques archivos `.env` reales.
  - No borres volumenes, caches ni datos persistentes sin aprobacion explicita.
  - No ejecutes `docker compose down -v`.
  - No cambies `TargetFramework`, versiones de paquetes ni la estructura de la solucion salvo que la tarea lo pida de forma explicita.

  ## Flujo normal

  Ejecuta desde la raiz del repo:

  ```bash
  make install
  make test
  make lint
  make dev

  Si necesitas una shell dentro del contenedor:

  make shell

  Para detener el entorno sin borrar datos:

  make stop

  ## Verificacion

  Antes de cerrar cambios de codigo o configuracion, ejecuta:

  make test
  make lint

<!-- SPECKIT START -->
For additional context about technologies to be used, project structure,
shell commands, and other important information, read the current plan
<!-- SPECKIT END -->
