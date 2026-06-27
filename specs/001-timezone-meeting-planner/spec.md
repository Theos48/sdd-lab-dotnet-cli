# Feature Specification: Timezone Meeting Planner

**Feature Branch**: `[001-timezone-meeting-planner]`

**Created**: 2026-06-26

**Status**: Draft

**Input**: User description: "Create a command-line utility that helps users determine the current local date and time for a place and compare it with another place when planning meetings across timezones. The utility should be easy to use for non-expert users, so it must support a dependable timezone-based input and may also support simpler location inputs such as common place names and Mexican postal codes. It must clearly show the local time for the requested place, the UTC offset, the time difference relative to a comparison place, and whether the compared times appear suitable for normal working hours. The behavior for invalid, unknown, or ambiguous input must be explicit and easy to understand. The first version should focus only on time lookup and meeting-planning support, not full calendar or scheduling workflows."

## Clarifications

### Session 2026-06-26

- Q: Which input forms should v1 support? -> A: Timezone identifiers plus a small documented alias list for common places; Mexican postal codes are rejected in v1.
- Q: How should ambiguous supported aliases be handled? -> A: Fail with an ambiguous-input error and list known matching aliases/timezones when available.
- Q: What output format should v1 use? -> A: Fixed human-readable labeled lines for place, local time, UTC offset, time difference, and working-hours result.
- Q: What comparison behavior should v1 provide? -> A: Report each place as within/outside working hours and a combined suitable result only when both are within hours.
- Q: What meeting-hours rule should v1 use? -> A: Weekdays from 09:00 inclusive through before 17:00, local to each place.

### Session 2026-06-27

- Q: What is the exact v1 alias-list scope? -> A: A fixed curated repository file; support is determined only by canonical IANA timezone identifiers or normalized aliases present in that file.
- Q: What exact successful output labels are required? -> A: Lookup labels are `Place`, `Timezone`, `Local date`, `Local time`, `UTC offset`; comparison adds `Working hours`, `Compared place`, `Compared timezone`, `Compared local date`, `Compared local time`, `Compared UTC offset`, `Compared working hours`, `Time difference`, `Meeting suitability`.
- Q: What exit code categories are required? -> A: `0` success, `1` invalid input, `2` unsupported input, `3` unknown input, `4` ambiguous input.
- Q: Is v1 output only human-readable or stable for automation? -> A: Output is human-readable only, but labels and field presence are stable enough for light automation.
- Q: How is local alias data maintained? -> A: Alias additions must be deterministic and validated: versioned file update, unique normalized alias, display name, valid IANA timezone, and ambiguity validation.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Look Up Local Time (Priority: P1)

A user provides one dependable timezone-based place input and sees the current
local date and time for that place with its UTC offset.

**Why this priority**: This is the core utility value and the minimum useful
behavior before comparison or meeting guidance can work.

**Independent Test**: Can be fully tested by providing a valid timezone-based
place input and verifying that the result identifies the place, local date,
local time, and UTC offset.

**Acceptance Scenarios**:

1. **Given** a valid timezone-based place input, **When** the user requests the current time, **Then** the utility shows labeled lines for resolved place, local date, local time, and UTC offset.
2. **Given** a valid timezone-based place input during a daylight-saving or offset-transition period, **When** the user requests the current time, **Then** the utility shows the offset that applies at the requested moment.

---

### User Story 2 - Compare Two Places for a Meeting (Priority: P2)

A user provides a requested place and a comparison place to understand the time
difference and whether both places appear to be in normal working hours.

**Why this priority**: Meeting planning requires comparing two local times, not
only looking up one place.

**Independent Test**: Can be fully tested by providing two valid place inputs
and verifying local time for each place, UTC offset for each place, the signed
time difference, and the working-hours assessment.

**Acceptance Scenarios**:

1. **Given** two valid place inputs, **When** the user compares them, **Then** the utility shows labeled lines for each local date and time, each UTC offset, the time difference, each place's working-hours status, and a combined suitability result.
2. **Given** two valid place inputs where only one place is within normal working hours, **When** the user compares them, **Then** the utility marks that place as suitable, marks the other place as outside working hours, and reports the combined result as not suitable for both places.

---

### User Story 3 - Understand Invalid or Ambiguous Input (Priority: P3)

A non-expert user receives clear feedback when a place cannot be resolved or
when a convenience input could match multiple places.

**Why this priority**: Clear failures prevent users from trusting an incorrect
meeting time and make the utility usable without timezone expertise.

**Independent Test**: Can be fully tested by providing unknown, invalid, and
ambiguous place inputs and verifying that each failure produces a clear message
and a non-zero exit code.

**Acceptance Scenarios**:

1. **Given** an unknown place input, **When** the user requests a lookup, **Then** the utility explains that the place was not recognized and gives an example of accepted timezone-based input.
2. **Given** an ambiguous supported alias input, **When** the user requests a lookup, **Then** the utility explains that the input is ambiguous, lists known matching aliases or timezones when available, and asks for a more specific timezone-based input.
3. **Given** a malformed timezone-based input, **When** the user requests a lookup, **Then** the utility reports the input as invalid and exits with a non-zero status.

### Edge Cases

- The requested place and comparison place resolve to the same timezone.
- The two places have different calendar dates at the same moment.
- A place is currently on a UTC offset that differs from its usual standard offset.
- A supported alias input is recognized but maps to more than one possible timezone.
- An ambiguous alias has known matching aliases or timezones that can be shown to the user.
- An ambiguous alias has no safe match list available and must still fail without guessing.
- A Mexican postal code is entered and must be rejected as unsupported in v1.
- Required input is missing.
- More comparison places are provided than the first version supports.
- Output labels must remain stable across successful lookup and comparison results.
- One compared place is within normal working hours and the other is outside normal working hours.
- A local time is exactly 09:00 on a weekday and must be treated as within normal working hours.
- A local time is exactly 17:00 on a weekday and must be treated as outside normal working hours.
- A supported alias is added to the local alias file with a duplicate normalized alias and must be treated as invalid alias data before release.
- A supported alias references a timezone identifier that is not valid in the supported container environment and must be treated as invalid alias data before release.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The utility MUST expose a stable way for users to provide one requested place and, optionally, one comparison place.
- **FR-002**: The utility MUST accept dependable timezone-based inputs as the required v1 input format.
- **FR-002a**: The utility MAY accept aliases only from a fixed curated repository file, and support MUST be determined solely by canonical IANA timezone identifiers or normalized aliases present in that file.
- **FR-002b**: Each supported alias MUST include a user-facing alias, normalized alias key, display name, and canonical IANA timezone identifier.
- **FR-003**: The utility MUST show the resolved place, current local date, current local time, and UTC offset for a valid requested place.
- **FR-004**: When a comparison place is provided, the utility MUST show the resolved comparison place, its local date, local time, and UTC offset.
- **FR-005**: When two places are provided, the utility MUST show the signed time difference between the requested place and the comparison place.
- **FR-006**: When two places are provided, the utility MUST show whether each place is within normal working hours and MUST report the combined result as suitable only when both places are within normal working hours.
- **FR-007**: The utility MUST treat normal working hours as Monday through Friday, 09:00 inclusive through before 17:00 at each resolved place, unless a future specification changes this rule.
- **FR-008**: The utility MUST clearly distinguish unsupported convenience inputs from valid timezone-based inputs and supported aliases.
- **FR-009**: The utility MUST reject Mexican postal codes as unsupported in v1 and explain that timezone identifiers are the dependable input form.
- **FR-010**: Unknown, invalid, missing, unsupported, or ambiguous input MUST produce a clear user-facing error message and a non-zero exit code.
- **FR-010a**: Ambiguous supported aliases MUST fail without interactive prompting or automatic selection, and the error MUST list known matching aliases or timezones when available.
- **FR-011**: The utility MUST NOT create, modify, or manage calendar events, invitations, reminders, recurring schedules, attendee lists, or availability records.
- **FR-012**: Domain behavior for time lookup, comparison, input resolution, and working-hours assessment MUST be testable independently from console input and output.
- **FR-013**: Successful single-place lookup output MUST use exactly these labels: `Place`, `Timezone`, `Local date`, `Local time`, `UTC offset`.
- **FR-014**: Successful comparison output MUST include all single-place lookup labels plus exactly these comparison labels: `Working hours`, `Compared place`, `Compared timezone`, `Compared local date`, `Compared local time`, `Compared UTC offset`, `Compared working hours`, `Time difference`, `Meeting suitability`.
- **FR-015**: The first version MUST NOT require users to parse free-form prose to identify successful result fields.
- **FR-016**: The utility MUST use exit code `0` for success, `1` for invalid input, `2` for unsupported input, `3` for unknown input, and `4` for ambiguous input.
- **FR-017**: V1 output MUST remain human-readable only, but successful output labels and required field presence MUST remain stable for light automation.
- **FR-018**: Alias data changes MUST be deterministic and validated before release: the repository file version changes, normalized aliases are unique unless explicitly modeled as ambiguous, display names are present, timezone identifiers are valid IANA identifiers in the supported container environment, and ambiguity behavior is covered.

### Key Entities

- **Place Input**: A user-provided value representing a requested or comparison place, including timezone-based inputs and supported aliases.
- **Supported Alias**: A documented convenience name in the fixed curated repository alias file, with alias, normalized alias key, display name, and canonical IANA timezone identifier.
- **Resolved Place**: A place input after successful resolution, including display name, timezone identity, current local date and time, and UTC offset.
- **Time Comparison**: The relationship between two resolved places at the same moment, including each local time and the signed time difference.
- **Working-Hours Assessment**: A per-place result indicating whether each resolved place is within normal working hours plus a combined result that is suitable only when both places are within normal working hours.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can look up the current local time and UTC offset for a valid timezone-based input in under 10 seconds.
- **SC-002**: A user can compare two valid places and identify the time difference and working-hours suitability in under 15 seconds.
- **SC-003**: 100% of invalid, unknown, missing, unsupported, and ambiguous input cases return a clear error message and a non-zero exit code.
- **SC-004**: 100% of completed lookup, comparison, and invalid-input behavior is covered by automated tests.
- **SC-005**: The first version excludes full calendar and scheduling workflows, verified by the absence of event creation, invitation, reminder, recurrence, attendee, and availability-management behavior.
- **SC-006**: 100% of successful lookup and comparison results expose the required result fields through stable labels.
- **SC-007**: 100% of input failure categories map to the documented exit code category for invalid, unsupported, unknown, or ambiguous input.
- **SC-008**: Every alias data change is reviewed against deterministic maintenance rules before release.

## Assumptions

- Users are people planning meetings manually and need quick timezone guidance, not calendar automation.
- Timezone-based input is the guaranteed v1 input path.
- Convenience input in v1 is limited to a fixed curated repository alias file; arbitrary place-name lookup and Mexican postal codes are out of scope for v1.
- Working-hours suitability is a simple guidance signal, not a guarantee of user availability, holidays, lunch breaks, or organization-specific schedules.
- Working-hours suitability uses only the local weekday and local clock time for each resolved place.
- The comparison is evaluated for the current moment only.
- The first version compares one requested place with at most one comparison place.
