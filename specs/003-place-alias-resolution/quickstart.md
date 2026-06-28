# Quickstart: Place Alias Resolution

## Prerequisites

- Docker
- Docker Compose
- `make`

Do not install the .NET SDK on the Fedora host. Use the repository Makefile and
Compose workflow.

## Setup

```bash
make install
```

## Run Tests

```bash
make test
```

Expected result: all automated tests pass, including tests for alias
normalization, selected Mexican postal-code resolution, unsupported postal
codes, ambiguity handling, catalog validation, and preservation of comparison
and working-hours behavior.

## Run Lint

```bash
make lint
```

Expected result: formatting verification passes with no changes required.

## Validate Existing IANA Timezone Lookup

```bash
make dev ARGS="--place America/Mexico_City"
```

Expected outcome:

- Exit code `0`.
- Output keeps existing labels.
- `Timezone: America/Mexico_City` is present.

## Validate Supported Alias Lookup

```bash
make dev ARGS="--place 'mexico city'"
```

Expected outcome:

- Exit code `0`.
- `Place: Mexico City` is present.
- `Timezone: America/Mexico_City` is present.

## Validate Alias Normalization

```bash
make dev ARGS="--place '  MEXICO---City  '"
```

Expected outcome:

- Exit code `0`.
- Input resolves as the supported `mexico city` alias.
- Existing lookup labels remain unchanged.

## Validate Supported Mexican Postal Code Lookup

```bash
make dev ARGS="--place 01000"
```

Expected outcome:

- Exit code `0`.
- Output resolves to `Timezone: America/Mexico_City`.
- Leading zero in the input remains meaningful.

## Validate Another Supported Mexican Postal Code

```bash
make dev ARGS="--place 64000"
```

Expected outcome:

- Exit code `0`.
- Output resolves to `Timezone: America/Monterrey`.

## Validate Unsupported Mexican Postal Code

```bash
make dev ARGS="--place 99999"
```

Expected outcome:

- Exit code `2`.
- Stderr explains that Mexican postal code `99999` is not supported in v1.
- Stderr lists supported Mexican postal codes: `01000`, `64000`, and `44100`.

## Validate Alias Comparison

```bash
make dev ARGS="--place 'mexico city' --compare london"
```

Expected outcome:

- Exit code `0`.
- Requested place resolves to `America/Mexico_City`.
- Compared place resolves to `Europe/London`.
- Existing comparison labels, time difference, working-hours status, and meeting
  suitability labels remain present.

## Validate Postal Code With Custom Working Hours

```bash
make dev ARGS="--place 01000 --compare london --working-hours-start 08:30 --working-hours-end 16:45"
```

Expected outcome:

- Exit code `0`.
- Postal code resolves to `America/Mexico_City`.
- Output includes `Working hours window: 08:30-16:45`.
- Existing custom working-hours behavior is preserved.

## Validate Working-Hours Error Precedence

```bash
make dev ARGS="--place 99999 --compare nowhere --working-hours-start bad --working-hours-end 17:00"
```

Expected outcome:

- Exit code `1`.
- Stderr reports the invalid working-hours time before any unsupported or
  unknown place-resolution error.

## Validate Unknown Alias

```bash
make dev ARGS="--place Atlantis"
```

Expected outcome:

- Exit code `3`.
- Stderr explains that the place is unknown.

## Contract Reference

The CLI contract is documented in [contracts/cli.md](./contracts/cli.md).
The domain entities and validation rules are documented in
[data-model.md](./data-model.md).
