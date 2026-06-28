# Feature Specification: Place Alias Resolution

**Feature Branch**: `003-place-alias-resolution`

**Created**: 2026-06-28

**Status**: Draft

**Input**: User description: "Allow users to resolve supported place aliases from a curated local repository file, including common city names and selected Mexican postal codes, while keeping IANA timezone identifiers as the canonical source of truth. The CLI should clearly distinguish supported aliases from unknown or ambiguous inputs, remain offline and deterministic, and preserve the existing comparison and working-hours behavior."

## Clarifications

### Session 2026-06-28

- Q: What is the exact initial catalog scope for v1? → A: Keep existing city aliases and add a small fixed set of selected Mexican postal codes for v1.
- Q: Which Mexican postal codes are selected for v1? → A: Support `01000` → `America/Mexico_City`, `64000` → `America/Monterrey`, and `44100` → `America/Mexico_City`.
- Q: What normalization rules apply to alias input? → A: Normalize casing, surrounding whitespace, repeated whitespace, punctuation separators, and accents; fuzzy typo matching is out of scope.
- Q: How should resolved aliases and postal codes appear in output? → A: Keep existing labels: `Place` shows the catalog display name and `Timezone` shows the canonical IANA timezone.
- Q: What maintenance expectations apply to the curated catalog? → A: Automated validation must reject duplicate normalized inputs, invalid timezones, missing required fields, and malformed postal codes.

## User Scenarios & Testing *(mandatory)*

### User Story 1 - Resolve Supported Place Aliases (Priority: P1)

A user enters a supported common place name instead of a timezone identifier and
the CLI resolves it to the canonical timezone used for lookup and comparison.

**Why this priority**: This is the primary usability improvement. Non-expert
users should not need to know the exact IANA timezone identifier when the place
is explicitly supported by the curated catalog.

**Independent Test**: Can be fully tested by running a lookup with a supported
alias and verifying that the output shows the alias as the requested place while
using the expected canonical timezone, local date, local time, and UTC offset.

**Acceptance Scenarios**:

1. **Given** a supported city alias exists in the curated catalog, **When** the user runs a lookup with that alias, **Then** the CLI resolves it to the catalog's canonical IANA timezone and returns a successful lookup.
2. **Given** a supported city alias is entered with different casing, surrounding whitespace, repeated whitespace, punctuation separators, or accents, **When** the user runs a lookup, **Then** the CLI resolves the normalized alias to the same canonical timezone.
3. **Given** a supported alias is used as either `--place` or `--compare`, **When** the user runs a comparison, **Then** the comparison uses the resolved canonical timezones while preserving existing comparison output and exit semantics.
4. **Given** a supported alias or postal code resolves successfully, **When** output is produced, **Then** existing labels are preserved with `Place` showing the catalog display name and `Timezone` showing the canonical IANA timezone.

---

### User Story 2 - Resolve Selected Mexican Postal Codes (Priority: P2)

A user enters a Mexican postal code that is explicitly supported by the curated
catalog and the CLI resolves it to the catalog's canonical timezone.

**Why this priority**: Mexican postal codes were previously called out as
unsupported input. Supporting a selected, deterministic subset provides a clear
increment without pretending to provide full postal-code geocoding.

**Independent Test**: Can be fully tested by running a lookup with a supported
Mexican postal code and verifying successful resolution to the expected
canonical IANA timezone, plus by running an unsupported postal code and
verifying the explicit unsupported-input behavior.

**Acceptance Scenarios**:

1. **Given** Mexican postal code `01000`, `64000`, or `44100` is present in the curated catalog, **When** the user runs a lookup with that code, **Then** the CLI resolves it to the catalog's canonical IANA timezone and returns success.
2. **Given** a Mexican postal code is not explicitly present in the curated catalog, **When** the user runs a lookup with that code, **Then** the CLI returns the existing unsupported-input category with a clear message that only selected Mexican postal codes are supported.
3. **Given** a supported Mexican postal code is used in a comparison, **When** the user runs the comparison, **Then** existing time-difference, working-hours, and meeting-suitability behavior are preserved.

---

### User Story 3 - Understand Unknown And Ambiguous Inputs (Priority: P3)

A user receives clear feedback when an alias or postal-code-like input cannot be
resolved deterministically.

**Why this priority**: Alias input is convenient only if failures are explicit.
The CLI must avoid silently guessing when a user input is unknown or ambiguous.

**Independent Test**: Can be fully tested by using an unknown alias, an
unsupported Mexican postal code, and an intentionally ambiguous catalog entry,
then verifying the error category, message, and exit code for each case.

**Acceptance Scenarios**:

1. **Given** a place name is not a valid IANA timezone and is not present in the curated catalog, **When** the user runs lookup or comparison, **Then** the CLI returns the unknown-input category with a clear message.
2. **Given** a user input matches multiple supported catalog entries, **When** the user runs lookup or comparison, **Then** the CLI returns the ambiguous-input category, lists the known matching options when available, and does not choose automatically.
3. **Given** supported alias input is combined with custom working-hours options, **When** the user runs a comparison, **Then** the existing working-hours validation rules and precedence remain unchanged.

### Edge Cases

- A user enters an IANA timezone identifier that also resembles a supported alias.
- A supported alias differs only by case, accents, punctuation, or extra spaces.
- A user enters a misspelled alias that would require fuzzy matching.
- A supported alias maps to the same canonical timezone as another alias.
- A supported alias input is used for `--place`, `--compare`, or both.
- A supported Mexican postal code begins with one or more leading zeroes.
- A numeric input looks like a Mexican postal code but is not explicitly supported.
- A catalog entry would create more than one match for the same normalized input.
- A catalog edit introduces a duplicate normalized input, invalid timezone, missing required field, or malformed postal code.
- Alias resolution succeeds while working-hours input is invalid.
- Working-hours input is valid while alias or postal code resolution fails.
- The curated catalog contains no entry for a user-provided common place name.
- The feature must not require network access or an interactive disambiguation prompt.

## Requirements *(mandatory)*

### Functional Requirements

- **FR-001**: The CLI MUST resolve place inputs from a curated, versioned repository catalog of supported aliases in addition to canonical IANA timezone identifiers.
- **FR-002**: IANA timezone identifiers MUST remain the canonical source of truth for timezone lookup, local date/time calculation, UTC offset calculation, and comparison behavior.
- **FR-003**: The curated catalog MUST keep the existing supported city aliases and explicitly define each alias and the canonical IANA timezone it resolves to.
- **FR-004**: The curated catalog MUST add the selected v1 Mexican postal codes `01000`, `64000`, and `44100`.
- **FR-005**: Postal code `01000` MUST resolve to `America/Mexico_City`, `64000` MUST resolve to `America/Monterrey`, and `44100` MUST resolve to `America/Mexico_City`.
- **FR-006**: Mexican postal codes not explicitly present in the curated catalog MUST remain unsupported and fail with the unsupported-input category.
- **FR-007**: Alias and postal code matching MUST be deterministic and must not use external APIs, remote lookup, databases, or inferred geocoding.
- **FR-008**: Alias matching MUST normalize casing, surrounding whitespace, repeated whitespace, punctuation separators, and accents without changing the meaning of the input.
- **FR-009**: Alias matching MUST NOT use fuzzy typo matching, partial search, or inferred matches.
- **FR-010**: Inputs that match more than one supported catalog entry MUST fail as ambiguous input, list known matching options when available, and must not be resolved automatically.
- **FR-011**: Inputs that are neither valid IANA timezone identifiers nor supported catalog entries MUST fail as unknown input, except for unsupported Mexican postal codes which MUST use the unsupported-input category.
- **FR-012**: Supported aliases and postal codes MUST work consistently for single-place lookup and two-place comparison.
- **FR-013**: Supported aliases and postal codes MUST preserve existing comparison behavior, including time difference, working-hours status, meeting suitability, custom working-hours validation, output labels, and exit semantics.
- **FR-014**: Invalid working-hours argument validation MUST continue to take precedence over alias, postal code, and place-resolution failures.
- **FR-015**: CLI behavior MUST continue to define stable arguments, output labels, errors, and exit codes for supported, unknown, unsupported, and ambiguous inputs.
- **FR-016**: Successful alias and postal-code output MUST preserve existing output labels; `Place` MUST show the catalog display name and `Timezone` MUST show the canonical IANA timezone.
- **FR-017**: Invalid, unknown, unsupported, or ambiguous inputs MUST fail with clear user-facing messages and non-zero exit codes.
- **FR-018**: Alias catalog behavior, postal-code support decisions, ambiguity handling, and comparison preservation MUST be covered by automated tests independent of terminal input/output where domain behavior is involved.
- **FR-019**: Future additions to the curated catalog MUST remain deterministic and reviewable by requiring each entry to declare its input form, display label, canonical IANA timezone, and support category.
- **FR-020**: Catalog validation MUST reject duplicate normalized inputs, invalid canonical IANA timezones, missing required fields, and malformed Mexican postal codes.

### Key Entities *(include if feature involves data)*

- **Supported Alias**: A curated user-facing place name that resolves to one canonical IANA timezone and can be used wherever a place input is accepted.
- **Supported Mexican Postal Code**: A selected Mexican postal code explicitly present in the curated catalog and mapped to one canonical IANA timezone.
- **Canonical Timezone**: The IANA timezone identifier used internally as the source of truth for local date/time, UTC offset, and time difference behavior.
- **Ambiguous Place Input**: A user input that matches multiple supported catalog entries and therefore cannot be resolved safely without a more specific input.
- **Unsupported Postal Code Input**: A numeric Mexican postal-code-like input that is recognized as a postal code form but is not included in the selected supported catalog.

## Success Criteria *(mandatory)*

### Measurable Outcomes

- **SC-001**: 100% of supported aliases in the curated catalog resolve to their documented canonical IANA timezone for lookup and comparison.
- **SC-002**: Postal codes `01000`, `64000`, and `44100` resolve to their documented canonical IANA timezone for lookup and comparison.
- **SC-003**: 100% of unsupported Mexican postal-code-like inputs return a clear unsupported-input message and non-zero exit code.
- **SC-004**: 100% of ambiguous catalog matches return the ambiguous-input category without selecting a timezone automatically.
- **SC-005**: Existing IANA timezone lookup, comparison, and custom working-hours behavior remain unchanged for previously supported commands.
- **SC-006**: A supported alias or supported Mexican postal-code lookup completes in under 15 seconds using the existing project execution workflow.
- **SC-007**: All completed behavior is covered by automated tests and passes the required project checks before work is closed.
- **SC-008**: 100% of catalog validation failures for duplicate normalized inputs, invalid timezones, missing required fields, and malformed postal codes are detected by automated tests.

## Assumptions

- The feature supports only aliases and Mexican postal codes explicitly present in the curated catalog; it does not provide full city search, fuzzy matching, geocoding, or a complete Mexican postal-code database.
- The initial curated catalog includes the existing city aliases plus selected Mexican postal codes `01000`, `64000`, and `44100` for v1.
- Postal-code inputs are treated as strings so leading zeroes remain significant.
- Accent, punctuation, and whitespace normalization may be supported only when it remains deterministic and testable.
- Fuzzy typo matching, partial text search, and inferred nearest-match behavior are out of scope.
- The feature does not add interactive prompts, saved user preferences, calendar workflows, or scheduling workflows.
- The feature remains offline and deterministic.
- The feature runs through existing `make` and Docker Compose workflows.
- No database, background service, web stack, external API, remote data source, or extra infrastructure is introduced.
