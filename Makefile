.PHONY: install dev test lint shell stop clean

install:
	docker compose run --rm app dotnet restore

dev:
	docker compose run --rm app dotnet run --project src/TimezoneCli

test:
	docker compose run --rm app dotnet test

lint:
	docker compose run --rm app dotnet format --verify-no-changes

shell:
	docker compose run --rm app sh

stop:
	docker compose down

clean:
	docker compose down --remove-orphans
