# Implementation Plan: Place Alias Resolution

**Branch**: `003-place-alias-resolution` | **Date**: 2026-06-28 | **Spec**: [spec.md](./spec.md)

**Input**: Feature specification from `specs/003-place-alias-resolution/spec.md`

## Summary

Extend the existing timezone meeting planner CLI so place inputs can resolve
through the curated local alias catalog as well as canonical IANA timezone
identifiers. The feature keeps IANA timezone IDs as the canonical source of
truth, preserves the existing lookup/comparison/working-hours CLI behavior, and
adds selected Mexican postal codes `01000`, `64000`, and `44100` as explicitly
supported catalog entries. Alias matching will remain offline and deterministic
while expanding normalization to handle casing, whitespace, punctuation
separators, and accents without fuzzy matching.

## Technical Context

**Language/Version**: .NET 8 CLI (`net8.0`)

**Primary Dependencies**: .NET standard library, existing xUnit test stack,
existing `dotnet format` workflow; no new runtime dependencies planned

**Storage**: Existing local versioned catalog file at
`src/TimezoneCli/Data/place-aliases.json`; no database, generated runtime
state, remote lookup, or user preferences

**Testing**: Automated tests through `make test`; formatting through
`make lint`

**Target Platform**: Existing containerized project workflow through Docker
Compose and Makefile targets; no host .NET SDK requirement

**Project Type**: Single executable CLI with separated domain logic and test
project

**Performance Goals**: Supported alias and supported Mexican postal-code lookup
complete in under 15 seconds through the existing project execution workflow

**Constraints**: Offline and deterministic; IANA timezone IDs remain canonical;
only catalog-listed aliases and postal codes are supported; no fuzzy matching,
partial search, inferred geocoding, external APIs, databases, background
services, web stack, or persistent configuration

**Scale/Scope**: Existing city aliases plus selected Mexican postal codes
`01000`, `64000`, and `44100`; one requested place and at most one comparison
place per invocation; existing custom working-hours behavior is preserved

## Constitution Check

*GATE: Must pass before Phase 0 research. Re-check after Phase 1 design.*

- **Containerized reproducibility**: PASS. The plan uses existing `make`
  targets backed by Docker Compose and does not require a host .NET SDK.
- **Make-driven CLI contract**: PASS. The changed place inputs, output
  preservation, errors, and exit-code categories are specified in the CLI
  contract.
- **Testable domain boundaries**: PASS. Catalog loading, normalization,
  validation, and resolution remain in `Domain/`; CLI parsing/output stays in
  `Cli/`.
- **Automated coverage**: PASS. Tests are required for alias resolution,
  selected postal-code resolution, unsupported postal codes, ambiguity,
  normalization, catalog validation, and behavior preservation.
- **Explicit failure semantics**: PASS. Unknown, unsupported, ambiguous, and
  invalid catalog states have clear error behavior and non-zero exit codes.
- **Scope control**: PASS. The design adds no databases, background services,
  web stack, external API, network dependency, or persistent configuration.
- **Quality gate**: PASS. Closing implementation work requires `make test` and
  `make lint`.

## Project Structure

### Documentation (this feature)

```text
specs/003-place-alias-resolution/
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
    ├── AliasCatalog.cs
    ├── AliasResolver.cs
    ├── PlaceAliasCatalog.cs
    ├── PlaceResolution.cs
    ├── ResolvedPlace.cs
    ├── SupportedAlias.cs
    ├── TimeComparisonService.cs
    └── TimezoneResolver.cs

tests/TimezoneCli.Tests/
├── AliasCatalogTests.cs
├── AliasResolverTests.cs
├── CliResultWriterTests.cs
├── PerformanceSmokeTests.cs
└── TimeComparisonServiceTests.cs
```

**Structure Decision**: Reuse the existing single CLI project and separate test
project. Extend the existing alias catalog data model and resolver in
`Domain/`. Update the local catalog JSON in `Data/`. Preserve CLI argument
shape and output labels, with only error text adjusted where unsupported
postal-code support decisions become more specific. Do not change solution
structure, target frameworks, package versions, Makefile workflow, or container
workflow.

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
