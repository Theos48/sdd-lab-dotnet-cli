# Implementation Plan: Timezone Meeting Planner

**Branch**: `main` | **Date**: 2026-06-27 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/001-timezone-meeting-planner/spec.md`

## Summary

Build a small .NET 8 command-line utility that reports current local date/time
for one place and optionally compares it with a second place for meeting
planning. The implementation will keep `src/TimezoneCli` as the single
executable entry point, keep `tests/TimezoneCli.Tests` as the separate test
project, use IANA timezone identifiers as canonical input, read a small
versioned local alias file for common place names, reject Mexican postal codes
in v1, and avoid external APIs, databases, services, or network access.

## Technical Context

**Language/Version**: .NET 8 CLI (`net8.0`)

**Primary Dependencies**: .NET standard library, existing xUnit test stack,
existing `dotnet format` workflow; no new runtime dependencies planned

**Storage**: Local versioned alias data file in the repository, planned at
`src/TimezoneCli/Data/place-aliases.json`; no database or generated runtime
state

**Testing**: Automated tests through `make test`; formatting through
`make lint`

**Target Platform**: Containerized project workflow through existing Docker
Compose service and Makefile targets

**Project Type**: Single executable CLI with separated domain logic and test
project

**Performance Goals**: Lookup or comparison completes in under 10 seconds for
single-place lookup and under 15 seconds for two-place comparison, matching the
spec success criteria

**Constraints**: No host .NET SDK requirement, no external APIs, no database, no
network access in v1, no full calendar or scheduling workflow, no interactive
prompting for ambiguous input

**Scale/Scope**: One requested place and at most one comparison place per
invocation; small documented alias list only; Mexican postal codes rejected as
unsupported in v1

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Containerized reproducibility**: PASS. All validation and development flows
  use existing `make` targets backed by Docker Compose; no host .NET SDK is
  required.
- **Make-driven CLI contract**: PASS. The contract defines `--place` and
  optional `--compare`, stable labeled stdout, stderr errors, and non-zero exit
  codes.
- **Testable domain boundaries**: PASS. Planning separates CLI parsing/output
  from timezone resolution, comparison, ambiguity handling, and working-hours
  domain services.
- **Automated coverage**: PASS. Tests are required for lookup, comparison,
  aliases, postal-code rejection, ambiguity, invalid input, output labels, and
  working-hours boundaries.
- **Explicit failure semantics**: PASS. Unknown, invalid, missing, unsupported,
  ambiguous, and too-many-comparison inputs fail explicitly with non-zero exit
  codes.
- **Scope control**: PASS. The plan uses local alias data only and introduces no
  databases, web stacks, background services, or external API access.
- **Quality gate**: PASS. Closing implementation work requires `make test` and
  `make lint`.

## Project Structure

### Documentation (this feature)

```text
specs/001-timezone-meeting-planner/
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
├── Data/
│   └── place-aliases.json
└── Domain/
    ├── AliasResolver.cs
    ├── PlaceAliasCatalog.cs
    ├── TimeComparisonService.cs
    ├── TimezoneResolver.cs
    └── WorkingHoursPolicy.cs

tests/TimezoneCli.Tests/
├── AliasResolverTests.cs
├── CliParserTests.cs
├── CliResultWriterTests.cs
├── TimeComparisonServiceTests.cs
├── TimezoneResolverTests.cs
└── WorkingHoursPolicyTests.cs
```

The source tree above lists primary service and boundary files. Additional small
value-object, result-type, test-helper, and smoke-test files under
`src/TimezoneCli/` and `tests/TimezoneCli.Tests/` are allowed when they preserve
clarity, testability, and the planned CLI/domain separation.

**Structure Decision**: Use the existing single CLI project and separate test
project. Add `Cli/` for argument parsing, exit-code mapping, and output
formatting. Add `Domain/` for independently testable timezone resolution,
comparison, ambiguity handling, and working-hours rules. Add `Data/` for the
small local alias file.

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
