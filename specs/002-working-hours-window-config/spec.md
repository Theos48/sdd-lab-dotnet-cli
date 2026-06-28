# Feature Specification: Working Hours Window Configuration

**Feature Branch**: `feature/002-working-hours-window-config`

**Created**: 2026-06-27

**Status**: Draft

**Input**: User description: "Extend the timezone meeting planner CLI so users can configure the working-hours window used for meeting suitability checks. Users should be able to provide a custom start time and end time for the working day, and the CLI should evaluate suitability using that custom window instead of the default 09:00 to 17:00 local-time rule. The feature should preserve the current timezone lookup and comparison behavior, remain offline and deterministic, and continue to provide clear error handling for invalid input."

## Clarifications

### Session 2026-06-27

- Q: What exact CLI inputs should users provide for the configurable working-hours window? -> A: Use `--working-hours-start <HH:mm>` and `--working-hours-end <HH:mm>`.
- Q: How should the configured working-hours window be reflected in successful comparison output? -> A: Add `Working hours window: <HH:mm>-<HH:mm>` to comparison output.
- Q: How should invalid working-hours ranges be handled when the end time is equal to or earlier than the start time? -> A: Reject the input with exit code `1`; overnight and zero-length windows are unsupported in v1.
- Q: Which error category takes precedence when working-hours arguments are invalid and place input also has problems? -> A: Working-hours argument errors take precedence and exit with code `1`.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Compare Places With Custom Working Hours (Priority: P1)

A user comparing two places provides a custom working-day start and end time so
meeting suitability is evaluated against that window instead of the default
09:00 to 17:00 rule.

**Why this priority**: This is the primary value of the feature and directly
supports users whose normal meeting window differs from the default.

**Independent Test**: Can be fully tested by comparing two valid places with a
custom start and end time, then verifying each place's working-hours status and
the combined meeting suitability result use the custom window.

**Acceptance Scenarios**:

1. **Given** two valid places and a custom window of 08:00 through before 16:00, **When** the user compares the places, **Then** each place is evaluated against 08:00 to 16:00 in its own local time and the output includes `Working hours window: 08:00-16:00`.
2. **Given** two valid places whose current local times both fall inside the custom window, **When** the user compares the places, **Then** each working-hours field is `within` and meeting suitability is `suitable`.
3. **Given** two valid places where one current local time falls outside the custom window, **When** the user compares the places, **Then** that place is marked `outside` and meeting suitability is `not suitable`.

---

### User Story 2 - Preserve Default Comparison Behavior (Priority: P2)

A user who does not provide a custom working-hours window continues to get the
existing meeting suitability behavior based on the default 09:00 to 17:00 local
weekday rule.

**Why this priority**: Existing users and light automation must not be forced to
change commands or reinterpret output when they do not need customization.

**Independent Test**: Can be fully tested by running an existing valid
comparison without custom working-hours input and verifying that the same output
labels, exit code, and default working-hours rule are used.

**Acceptance Scenarios**:

1. **Given** two valid places and no custom working-hours input, **When** the user compares the places, **Then** the CLI evaluates suitability using Monday through Friday, 09:00 inclusive through before 17:00.
2. **Given** a valid single-place lookup without comparison, **When** the user runs the lookup, **Then** the lookup output and exit semantics remain unchanged.

---

### User Story 3 - Understand Invalid Working-Hours Input (Priority: P3)

A user receives clear feedback when the custom working-hours window is missing,
malformed, unsupported, or logically invalid.

**Why this priority**: Incorrect working-hours input can produce misleading
meeting guidance, so failures must be explicit and easy to fix.

**Independent Test**: Can be fully tested by providing invalid custom start and
end time combinations and verifying clear error messages with non-zero exit
codes.

**Acceptance Scenarios**:

1. **Given** only a custom start time or only a custom end time, **When** the user runs a comparison, **Then** the CLI reports that both custom working-hours values are required together and exits with invalid-input status.
2. **Given** a malformed custom time such as `9am`, `24:00`, or `17:60`, **When** the user runs a comparison, **Then** the CLI reports the expected `HH:mm` format and exits with invalid-input status.
3. **Given** a custom end time that is equal to or earlier than the custom start time, **When** the user runs a comparison, **Then** the CLI reports that overnight or zero-length windows are not supported in v1 and exits with invalid-input status.

### Edge Cases

- A custom start time exactly matches a place's current local time and must be treated as within working hours.
- A custom end time exactly matches a place's current local time and must be treated as outside working hours.
- A custom window matches the default 09:00 to 17:00 rule and must produce the same suitability result as omitting the custom window.
- A custom window uses minute precision, such as 08:30 to 16:45.
- Custom working-hours input is provided without a comparison place.
- Only one custom working-hours boundary is provided.
- The custom end time is earlier than the custom start time, implying an overnight window.
- The custom end time equals the custom start time, implying a zero-length window.
- Place resolution fails while custom working-hours input is otherwise valid.
- Custom working-hours input is invalid while place inputs are otherwise valid.
- Custom working-hours input is invalid while place input is also unsupported, unknown, ambiguous, or invalid.
- Existing unsupported, unknown, or ambiguous place input behavior occurs together with valid custom working-hours input.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The CLI MUST expose `--working-hours-start <HH:mm>` and `--working-hours-end <HH:mm>` for users to provide a custom working-hours start time and end time for comparison results.
- **FR-002**: Custom working-hours start and end values MUST use `HH:mm` 24-hour local-clock format with two-digit hours and minutes.
- **FR-003**: Users MUST provide both custom working-hours start and end values together; providing only one MUST fail as invalid input.
- **FR-004**: The custom working-hours start time MUST be inclusive and the custom working-hours end time MUST be exclusive.
- **FR-005**: The CLI MUST reject a custom working-hours end time that is equal to or earlier than the custom start time with exit code `1`, because overnight and zero-length working windows are out of scope for v1.
- **FR-006**: When a comparison place is provided with a valid custom working-hours window, the CLI MUST evaluate both places' working-hours status using that custom window in each place's local time.
- **FR-007**: When a comparison place is provided without a custom working-hours window, the CLI MUST preserve the existing default rule of Monday through Friday, 09:00 inclusive through before 17:00 local time.
- **FR-008**: Custom working-hours input MUST NOT change place resolution, local date output, local time output, UTC offset output, time difference output, existing successful output labels, or existing comparison exit semantics; existing labels MUST NOT be removed or renamed.
- **FR-009**: Custom working-hours input MUST be valid only for comparison behavior; providing custom working-hours input without a comparison place MUST fail with a clear invalid-input message.
- **FR-010**: Invalid custom working-hours input MUST fail with exit code `1` and a clear message explaining the specific problem and the expected `HH:mm` format when relevant.
- **FR-011**: Working-hours argument validation MUST take precedence over place resolution; when working-hours input is invalid, the CLI MUST exit with code `1` even if place input also has unsupported, unknown, ambiguous, or invalid values.
- **FR-012**: Existing unsupported, unknown, and ambiguous place-input failures MUST continue to use their documented exit code categories when working-hours input is absent or valid.
- **FR-013**: The feature MUST remain offline and deterministic, without external APIs, remote lookup, databases, background services, calendar integrations, or persistent user configuration.
- **FR-014**: Domain behavior for custom working-hours parsing, validation, boundary evaluation, and meeting suitability MUST be testable independently from console input and output.
- **FR-015**: Successful comparison output MUST continue to use the existing stable labels and field presence, except that it MUST add `Working hours window: <HH:mm>-<HH:mm>` to show the active working-hours window.
- **FR-016**: The `Working hours window` label MUST be present for every successful comparison, including default-window comparisons where the value is `09:00-17:00`.

### Key Entities

- **Working-Hours Window**: The local-time window used to decide whether a place is within working hours, defined by an inclusive start time and exclusive end time on the same local day.
- **Custom Working-Hours Input**: The pair of user-provided start and end times that replaces the default working-hours window for one comparison command.
- **Working-Hours Assessment**: The per-place `within` or `outside` result and combined meeting suitability result calculated from the active working-hours window.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can compare two valid places with a custom working-hours window and identify per-place working-hours status plus combined meeting suitability in under 15 seconds.
- **SC-002**: 100% of successful comparisons without custom working-hours input preserve the existing default 09:00 to 17:00 behavior and successful output labels.
- **SC-003**: 100% of malformed, missing-pair, zero-length, overnight, or lookup-only custom working-hours inputs return a clear error message and exit code `1`.
- **SC-004**: 100% of custom working-hours boundary cases are covered by automated tests, including inclusive start and exclusive end.
- **SC-005**: 100% of completed behavior passes the required project checks before work is closed.

## Assumptions

- The custom working-hours window is a per-command option, not a saved user preference.
- Custom working-hours are relevant only to comparison output because single-place lookup does not produce meeting suitability guidance.
- The same custom window applies to both compared places, evaluated in each place's local time.
- Minute precision is sufficient for v1; seconds and timezone-qualified working-hours values are out of scope.
- Overnight working windows are out of scope for v1 to keep behavior simple and unambiguous.
- The feature runs through existing `make` and Docker Compose workflows.
- No database, background service, web stack, external API, remote data source, or extra infrastructure is introduced.
