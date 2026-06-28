# Research: Working Hours Window Configuration

## Decision: Use strict `HH:mm` parsing with `TimeOnly`

**Rationale**: The feature requires minute-level local-clock times with a
stable 24-hour format. .NET 8 provides `TimeOnly`, which represents a time of
day without date or timezone coupling and supports strict parsing through the
standard library. This keeps validation deterministic and avoids new
dependencies.

**Alternatives considered**:

- Accept flexible inputs such as `9am`: rejected because the spec requires
  explicit, predictable invalid-input behavior.
- Use `DateTime` or `DateTimeOffset`: rejected because working-hours input is a
  local time-of-day window, not an instant.
- Add a parsing package: rejected because standard .NET APIs are sufficient.

## Decision: Model the active working-hours window as domain data

**Rationale**: A validated working-hours window with inclusive start and
exclusive end belongs to domain behavior because it drives per-place assessment
and combined meeting suitability. Keeping it out of console output/parsing code
preserves testable domain boundaries.

**Alternatives considered**:

- Keep fixed static fields in `WorkingHoursPolicy`: rejected because custom
  windows need per-command behavior.
- Pass raw strings into comparison logic: rejected because parsing and
  validation would leak across layers and complicate tests.

## Decision: Preserve default behavior with an explicit default window

**Rationale**: Existing comparison commands without custom flags must continue
to evaluate Monday through Friday, 09:00 inclusive through before 17:00. An
explicit default window makes both default and custom paths use the same
assessment rules and boundary tests.

**Alternatives considered**:

- Maintain separate default and custom code paths: rejected because duplicated
  logic increases regression risk.
- Require users to always provide working-hours flags: rejected because the
  spec requires existing behavior to remain unchanged.

## Decision: Validate working-hours arguments before place resolution

**Rationale**: The clarified contract requires malformed working-hours input,
missing paired values, lookup-only use, and invalid ranges to fail as argument
errors with exit code `1`, taking precedence over place-resolution errors.
This gives users direct feedback for command-shape problems and keeps exit
code categories deterministic.

**Alternatives considered**:

- Resolve places before checking working-hours arguments: rejected because it
  could return unsupported, unknown, or ambiguous place errors when the command
  arguments are already invalid.
- Report multiple errors at once: rejected for v1 to keep error output simple
  and consistent with the existing one-error CLI flow.

## Decision: Reject overnight and zero-length windows in v1

**Rationale**: End times equal to or earlier than start times introduce
cross-date semantics, weekday boundary questions, and more edge cases. The
spec explicitly keeps v1 simple by rejecting those ranges with exit code `1`.

**Alternatives considered**:

- Treat the end time as the next day: rejected because it changes weekday and
  weekend evaluation complexity.
- Swap start and end automatically: rejected because it hides user mistakes.

## Decision: Add a stable `Working hours window` output label

**Rationale**: Comparison output should show which window was applied so users
and lightweight automation do not need to infer whether the default or a custom
window was used. The label is present for all successful comparisons, including
default-window comparisons.

**Alternatives considered**:

- Do not print the active window: rejected because results would be harder to
  audit.
- Print prose only for custom windows: rejected because conditional field
  presence is harder to test and automate.

## Decision: Keep timezone and alias resolution unchanged

**Rationale**: Configurable working hours do not change how places are
resolved. IANA timezone identifiers remain the canonical source of truth, the
local alias catalog remains the only convenience input source, and the feature
stays offline and deterministic.

**Alternatives considered**:

- Add external location or timezone services: rejected by v1 scope and project
  constitution.
- Add persistent user working-hours preferences: rejected because the feature
  is per-command only.
