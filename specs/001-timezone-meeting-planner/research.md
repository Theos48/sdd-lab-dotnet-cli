# Research: Timezone Meeting Planner

## Decision: Use .NET 8 standard library timezone APIs with IANA IDs

**Rationale**: The project already targets .NET 8 and runs inside a Linux
container, where IANA timezone identifiers such as `America/Mexico_City` are
the natural canonical identifiers. `TimeZoneInfo` can resolve those IDs in the
container without adding a third-party package or a network dependency.

**Alternatives considered**:

- Third-party timezone libraries: rejected for v1 because built-in APIs cover
  the required lookup and offset behavior.
- External timezone APIs: rejected because v1 must work without network access.
- Windows timezone IDs as canonical input: rejected because the supported
  container environment uses IANA identifiers.

## Decision: Use a local versioned alias data file

**Rationale**: A small repository-owned alias file keeps common-place aliases
auditable, deterministic, testable, and offline. It also makes ambiguity rules
explicit without requiring a geocoding service or postal-code database.

**Alternatives considered**:

- Hard-coded aliases in source code: rejected because data changes would be
  mixed with domain logic.
- External geocoding or postal-code services: rejected because v1 forbids
  network access and external APIs.
- Full postal-code dataset: rejected because Mexican postal codes are out of
  scope for v1 and would expand data maintenance requirements.

## Decision: Reject Mexican postal codes in v1

**Rationale**: Postal-code support requires a maintained mapping from postal
codes to places or timezones and clear handling for codes crossing geographic
or administrative boundaries. The clarified v1 scope requires postal codes to
fail explicitly with guidance to use timezone identifiers.

**Alternatives considered**:

- Prefix-based postal-code heuristics: rejected because they risk incorrect
  timezone resolution.
- Full local postal-code lookup: rejected as too much data and maintenance for
  v1.

## Decision: Keep CLI parsing/output separate from domain services

**Rationale**: The constitution requires domain behavior to be testable without
terminal coupling. Parsing command options and writing labeled output are CLI
concerns, while input resolution, local time calculation, comparison, and
working-hours assessment are domain concerns.

**Alternatives considered**:

- Put all behavior in `Program.cs`: rejected because it couples behavior to
  console I/O and makes focused testing harder.
- Add a separate class library project: rejected for v1 because the existing
  CLI project can expose testable domain classes without changing solution
  structure.

## Decision: Use `TimeProvider` or equivalent injectable clock boundary

**Rationale**: Current-time behavior must be testable at daylight-saving
transitions, date-boundary differences, and working-hours boundaries. .NET 8
provides `TimeProvider`, which avoids a custom time abstraction unless a small
wrapper becomes clearer during implementation.

**Alternatives considered**:

- Direct calls to current system time inside domain logic: rejected because
  tests would be time-dependent.
- Third-party clock abstractions: rejected because .NET 8 provides enough
  standard-library support.

## Decision: Use fixed labeled human-readable output

**Rationale**: The clarified spec requires stable labeled lines that are easy
for non-experts and straightforward for automated tests to verify. JSON output
is not needed for v1.

**Alternatives considered**:

- Free-form prose: rejected because it is harder to test and scan.
- Table output: rejected because labels are simpler and less sensitive to
  terminal width.
- Optional JSON: deferred until an automation requirement appears.

## Decision: Non-interactive ambiguity handling

**Rationale**: The CLI must fail explicitly for ambiguous aliases and must not
guess or prompt interactively. Listing known matches when available helps users
recover while preserving deterministic command behavior.

**Alternatives considered**:

- Interactive selection prompt: rejected because it complicates automation and
  tests.
- Automatic best-match selection: rejected because it risks silently wrong
  meeting times.

## Decision: Working-hours policy is weekday 09:00 <= time < 17:00

**Rationale**: The clarified v1 rule is a simple local-time policy. The
half-open end boundary avoids treating exactly 17:00 as a normal meeting start.

**Alternatives considered**:

- Inclusive 17:00 end boundary: rejected because it implies availability at the
  end of the workday.
- Configurable hours, holidays, or organization calendars: rejected as
  scheduling workflow scope outside v1.
