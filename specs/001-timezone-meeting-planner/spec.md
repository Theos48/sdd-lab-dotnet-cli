# Feature Specification: Timezone Meeting Planner

**Feature Branch**: `[001-timezone-meeting-planner]`

**Created**: 2026-06-26

**Status**: Draft

**Input**: User description: "Create a command-line utility that helps users determine the current local date and time for a place and compare it with another place when planning meetings across timezones. The utility should be easy to use for non-expert users, so it must support a dependable timezone-based input and may also support simpler location inputs such as common place names and Mexican postal codes. It must clearly show the local time for the requested place, the UTC offset, the time difference relative to a comparison place, and whether the compared times appear suitable for normal working hours. The behavior for invalid, unknown, or ambiguous input must be explicit and easy to understand. The first version should focus only on time lookup and meeting-planning support, not full calendar or scheduling workflows."

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

1. **Given** a valid timezone-based place input, **When** the user requests the current time, **Then** the utility shows the resolved place, local date, local time, and UTC offset.
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

1. **Given** two valid place inputs, **When** the user compares them, **Then** the utility shows each local date and time, each UTC offset, the time difference, and whether both times are within normal working hours.
2. **Given** two valid place inputs where only one place is within normal working hours, **When** the user compares them, **Then** the utility clearly identifies that the compared time is not suitable for both places.

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
2. **Given** an ambiguous convenience place input, **When** the user requests a lookup, **Then** the utility explains that the input is ambiguous and asks for a more specific timezone-based input.
3. **Given** a malformed timezone-based input, **When** the user requests a lookup, **Then** the utility reports the input as invalid and exits with a non-zero status.

### Edge Cases

- The requested place and comparison place resolve to the same timezone.
- The two places have different calendar dates at the same moment.
- A place is currently on a UTC offset that differs from its usual standard offset.
- A convenience input is recognized but maps to more than one possible timezone.
- A Mexican postal code is entered but is not recognized or is outside supported coverage.
- Required input is missing.
- More comparison places are provided than the first version supports.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The utility MUST expose a stable way for users to provide one requested place and, optionally, one comparison place.
- **FR-002**: The utility MUST accept dependable timezone-based inputs as the required v1 input format.
- **FR-003**: The utility MUST show the resolved place, current local date, current local time, and UTC offset for a valid requested place.
- **FR-004**: When a comparison place is provided, the utility MUST show the resolved comparison place, its local date, local time, and UTC offset.
- **FR-005**: When two places are provided, the utility MUST show the signed time difference between the requested place and the comparison place.
- **FR-006**: When two places are provided, the utility MUST state whether both local times appear suitable for normal working hours.
- **FR-007**: The utility MUST treat normal working hours as Monday through Friday, 09:00 through 17:00 inclusive at each resolved place, unless a future specification changes this rule.
- **FR-008**: The utility MUST clearly distinguish unsupported convenience inputs from valid timezone-based inputs.
- **FR-009**: If common place names or Mexican postal codes are supported, the utility MUST resolve them only when the match is unambiguous.
- **FR-010**: Unknown, invalid, missing, unsupported, or ambiguous input MUST produce a clear user-facing error message and a non-zero exit code.
- **FR-011**: The utility MUST NOT create, modify, or manage calendar events, invitations, reminders, recurring schedules, attendee lists, or availability records.
- **FR-012**: Domain behavior for time lookup, comparison, input resolution, and working-hours assessment MUST be testable independently from console input and output.

### Key Entities

- **Place Input**: A user-provided value representing a requested or comparison place, including timezone-based inputs and any supported convenience inputs.
- **Resolved Place**: A place input after successful resolution, including display name, timezone identity, current local date and time, and UTC offset.
- **Time Comparison**: The relationship between two resolved places at the same moment, including each local time and the signed time difference.
- **Working-Hours Assessment**: A result indicating whether one or both resolved places are within normal working hours.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: A user can look up the current local time and UTC offset for a valid timezone-based input in under 10 seconds.
- **SC-002**: A user can compare two valid places and identify the time difference and working-hours suitability in under 15 seconds.
- **SC-003**: 100% of invalid, unknown, missing, unsupported, and ambiguous input cases return a clear error message and a non-zero exit code.
- **SC-004**: 100% of completed lookup, comparison, and invalid-input behavior is covered by automated tests.
- **SC-005**: The first version excludes full calendar and scheduling workflows, verified by the absence of event creation, invitation, reminder, recurrence, attendee, and availability-management behavior.

## Assumptions

- Users are people planning meetings manually and need quick timezone guidance, not calendar automation.
- Timezone-based input is the guaranteed v1 input path; convenience inputs such as common place names and Mexican postal codes are acceptable only when they can be resolved unambiguously.
- Working-hours suitability is a simple guidance signal, not a guarantee of user availability, holidays, lunch breaks, or organization-specific schedules.
- The comparison is evaluated for the current moment only.
- The first version compares one requested place with at most one comparison place.
