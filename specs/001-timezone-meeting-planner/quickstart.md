# Quickstart: Timezone Meeting Planner

## Prerequisites

- Docker
- Docker Compose
- `make`

No host .NET SDK is required.

## Setup

```bash
make install
```

## Run Tests

```bash
make test
```

Expected outcome: all automated tests pass, including domain tests for timezone
resolution, alias resolution, ambiguity handling, comparison rules,
working-hours boundaries, and CLI output/error behavior.

## Validate Formatting

```bash
make lint
```

Expected outcome: formatting verification completes without changes.

## Run A Single-Place Lookup

```bash
docker compose run --rm app dotnet run --project src/TimezoneCli -- --place America/Mexico_City
```

Expected outcome:

- Exit code `0`.
- Stdout includes stable labels for `Place`, `Timezone`, `Local date`,
  `Local time`, and `UTC offset`.
- No stderr output.

## Run A Two-Place Comparison

```bash
docker compose run --rm app dotnet run --project src/TimezoneCli -- --place America/Mexico_City --compare America/New_York
```

Expected outcome:

- Exit code `0`.
- Stdout includes stable labels for both places, both UTC offsets,
  `Time difference`, each working-hours result, and `Meeting suitability`.
- `Meeting suitability` is `suitable` only when both places are within normal
  working hours.

## Validate Unsupported Mexican Postal Code

```bash
docker compose run --rm app dotnet run --project src/TimezoneCli -- --place 01000
```

Expected outcome:

- Exit code `2`.
- Stderr explains that Mexican postal codes are not supported in v1.
- Stderr suggests using an IANA timezone such as `America/Mexico_City`.

## Validate Ambiguous Alias

Use a test alias from the local alias catalog that is intentionally ambiguous,
or a fixture-specific alias in automated tests.

Expected outcome:

- Exit code `4`.
- Stderr explains that the place is ambiguous.
- Known matching aliases or timezones are listed when available.
- No interactive prompt appears and no automatic timezone is selected.

## Contract Reference

The CLI contract is documented in [contracts/cli.md](./contracts/cli.md).
The domain entities and validation rules are documented in
[data-model.md](./data-model.md).
