# Quickstart: Working Hours Window Configuration

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

Expected result: all automated tests pass, including tests for parsing,
working-hours validation, boundary behavior, comparison suitability, output
labels, and error precedence.

## Run Lint

```bash
make lint
```

Expected result: formatting verification passes with no changes required.

## Validate Default Comparison Output

```bash
make dev ARGS="--place America/Mexico_City --compare Europe/London"
```

Expected outcome:

- Exit code `0`.
- Existing comparison labels remain present.
- Output includes `Working hours window: 09:00-17:00`.

## Validate Custom Working-Hours Comparison

```bash
make dev ARGS="--place America/Mexico_City --compare Europe/London --working-hours-start 08:30 --working-hours-end 16:45"
```

Expected outcome:

- Exit code `0`.
- Output includes `Working hours window: 08:30-16:45`.
- `Working hours`, `Compared working hours`, and `Meeting suitability` are
  evaluated using 08:30 inclusive through before 16:45 in each place's local
  time.

## Validate Equals-Form Flags

```bash
make dev ARGS="--place=America/Mexico_City --compare=Europe/London --working-hours-start=08:30 --working-hours-end=16:45"
```

Expected outcome:

- Exit code `0`.
- Output includes `Working hours window: 08:30-16:45`.

## Validate Missing Pair Error

```bash
make dev ARGS="--place America/Mexico_City --compare Europe/London --working-hours-start 08:30"
```

Expected outcome:

- Exit code `1`.
- Stderr explains that `--working-hours-start` and `--working-hours-end` must
  be provided together.

## Validate Lookup-Only Error

```bash
make dev ARGS="--place America/Mexico_City --working-hours-start 08:30 --working-hours-end 16:45"
```

Expected outcome:

- Exit code `1`.
- Stderr explains that working-hours options require `--compare`.

## Validate Malformed Time Error

```bash
make dev ARGS="--place America/Mexico_City --compare Europe/London --working-hours-start 9am --working-hours-end 17:00"
```

Expected outcome:

- Exit code `1`.
- Stderr explains that working-hours times must use `HH:mm` 24-hour format.

## Validate Invalid Range Error

```bash
make dev ARGS="--place America/Mexico_City --compare Europe/London --working-hours-start 17:00 --working-hours-end 09:00"
```

Expected outcome:

- Exit code `1`.
- Stderr explains that `--working-hours-end` must be later than
  `--working-hours-start`.
- Overnight windows are not accepted in v1.

## Validate Error Precedence

```bash
make dev ARGS="--place Atlantis --compare Nowhere --working-hours-start bad --working-hours-end 17:00"
```

Expected outcome:

- Exit code `1`.
- Stderr reports the invalid working-hours time before any unknown-place error.
