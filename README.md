# TimezoneCli

  Laboratorio minimo para una CLI en `.NET 8` ejecutada dentro de contenedores.

  Hoy el repositorio contiene:

  - una aplicacion de consola en `src/TimezoneCli`
  - un proyecto de pruebas en `tests/TimezoneCli.Tests`
  - un `Makefile` como interfaz estable del proyecto
  - un contenedor con `dotnet/sdk:8.0` para no instalar `.NET` en el host

  ## Que hace este proyecto

  Este repositorio es una base reproducible para desarrollar una CLI en `.NET 8`.

  El flujo del laboratorio esta pensado para que:

  - el codigo viva en este repo
  - el SDK de `.NET` viva dentro del contenedor
  - las dependencias se restauren desde el contenedor
  - las pruebas y validaciones se ejecuten con comandos `make`

  En el estado actual, la app es una consola minima y el proyecto sirve como base
  para seguir desarrollando la CLI.

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

  ## Instalar dependencias del laboratorio

  Restaura las dependencias NuGet del proyecto dentro del contenedor:

  make install

  Esto ejecuta dotnet restore dentro del servicio app.

  ## Correr la aplicacion

  Ejecuta la CLI desde el contenedor:

  make dev

  En el estado actual, este comando corre el proyecto src/TimezoneCli.

  ## Correr pruebas

  Ejecuta las pruebas automatizadas del repositorio:

  make test

  Esto corre dotnet test dentro del contenedor.

  ## Validar formato y estilo

  Verifica el formato del codigo sin modificar archivos:

  make lint

  Esto ejecuta:

  dotnet format --verify-no-changes

  ## Parar el entorno

  Detiene el entorno de Compose sin borrar volumenes:

  make stop

  ## Comandos disponibles

  make install
  make dev
  make test
  make lint
  make shell
  make stop
  make clean

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
