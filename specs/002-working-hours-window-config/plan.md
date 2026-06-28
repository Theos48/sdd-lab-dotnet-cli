# Implementation Plan: Working Hours Window Configuration

**Branch**: `feature/002-working-hours-window-config` | **Date**: 2026-06-27 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/002-working-hours-window-config/spec.md`

## Summary

Extend the existing timezone meeting planner CLI so comparison commands can use
a configurable working-hours window supplied through
`--working-hours-start <HH:mm>` and `--working-hours-end <HH:mm>`. The
implementation will keep the current .NET 8 CLI/test-project layout, parse and
validate the two flags before place resolution, represent the active
working-hours window as domain data, apply the same window to both compared
places in their local time, add the `Working hours window` labeled output to
all successful comparisons, and preserve offline timezone/alias behavior.

## Technical Context

**Language/Version**: .NET 8 CLI (`net8.0`)

**Primary Dependencies**: .NET standard library, existing xUnit test stack,
existing `dotnet format` workflow; no new runtime dependencies planned

**Storage**: Existing local versioned alias data file remains unchanged at
`src/TimezoneCli/Data/place-aliases.json`; no database, user preferences, or
generated runtime state

**Testing**: Automated tests through `make test`; formatting through
`make lint`

**Target Platform**: Existing containerized project workflow through Docker
Compose and Makefile targets; no host .NET SDK requirement

**Project Type**: Single executable CLI with separated domain logic and test
project

**Performance Goals**: Preserve the spec outcome that users can complete a
valid two-place comparison with custom working hours in under 15 seconds

**Constraints**: Strict `HH:mm` 24-hour parsing, both custom flags required
together, invalid working-hours arguments exit with code `1` before place
resolution, no overnight or zero-length windows, no external APIs, no network
lookup, no persistent configuration, no calendar/scheduling workflow

**Scale/Scope**: One requested place and at most one comparison place per
invocation; one per-command working-hours window shared by both places and
evaluated in each place's local time

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Containerized reproducibility**: PASS. The plan uses existing `make`
  targets backed by Docker Compose and does not require a host .NET SDK.
- **Make-driven CLI contract**: PASS. The changed arguments, stdout label,
  stderr categories, and exit-code precedence are specified in the CLI
  contract.
- **Testable domain boundaries**: PASS. Parsing/output remains in `Cli/`;
  working-hours window validation and assessment remain in `Domain/`.
- **Automated coverage**: PASS. Tests are required for parsing, validation,
  precedence, boundary evaluation, comparison behavior, and output labels.
- **Explicit failure semantics**: PASS. Malformed, missing-pair, lookup-only,
  zero-length, and overnight working-hours inputs fail with exit code `1`.
- **Scope control**: PASS. The design adds no databases, background services,
  web stack, external API, network dependency, or persistent configuration.
- **Quality gate**: PASS. Closing implementation work requires `make test` and
  `make lint`.

## Project Structure

### Documentation (this feature)

```text
specs/002-working-hours-window-config/
├── plan.md
├── research.md
├── data-model.md
├── quickstart.md
├── contracts/
│   └── cli.md
└── tasks.md
```

### Source Code (repository root)

```text
src/TimezoneCli/
├── Program.cs
├── Cli/
│   ├── CliOptions.cs
│   ├── CliParser.cs
│   ├── CliResultWriter.cs
│   └── ExitCodes.cs
└── Domain/
    ├── TimeComparison.cs
    ├── TimeComparisonService.cs
    ├── WorkingHoursAssessment.cs
    └── WorkingHoursPolicy.cs

tests/TimezoneCli.Tests/
├── CliParserTests.cs
├── CliResultWriterTests.cs
├── TimeComparisonServiceTests.cs
└── WorkingHoursPolicyTests.cs
```

Additional small value-object or error-helper files under `src/TimezoneCli/Cli`
or `src/TimezoneCli/Domain` are allowed when they preserve the existing
CLI/domain separation and keep behavior independently testable.

**Structure Decision**: Reuse the existing single CLI project and separate test
project. Extend CLI parsing/options and result writing in `Cli/`. Extend
working-hours policy and comparison behavior in `Domain/`. Do not change
solution structure, target frameworks, package versions, alias catalog storage,
or container workflow.

## Complexity Tracking

No constitution violations or added complexity are planned.

## Phase 0: Research

Research decisions are captured in [research.md](./research.md). All technical
context questions are resolved.

## Phase 1: Design And Contracts

Design artifacts generated:

- [data-model.md](./data-model.md)
- [contracts/cli.md](./contracts/cli.md)
- [quickstart.md](./quickstart.md)

Post-design Constitution Check remains PASS: the generated design keeps
container-first execution, uses Makefile validation, preserves CLI/domain
separation, requires automated coverage, documents explicit failure semantics,
and avoids unapproved infrastructure.
