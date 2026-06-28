.PHONY: install dev test lint shell stop clean

ARGS ?=

install:
	docker compose run --rm app dotnet restore

dev:
	docker compose run --rm app dotnet run --project src/TimezoneCli -- $(ARGS)

test:
	docker compose run --rm app dotnet test tests/TimezoneCli.Tests/TimezoneCli.Tests.csproj

lint:
	docker compose run --rm app dotnet format tests/TimezoneCli.Tests/TimezoneCli.Tests.csproj --verify-no-changes

shell:
	docker compose run --rm app sh

stop:
	docker compose down

clean:
	docker compose down --remove-orphans
