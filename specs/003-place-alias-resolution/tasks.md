# Tasks: Place Alias Resolution

**Input**: Design documents from `/specs/003-place-alias-resolution/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/cli.md`, `quickstart.md`

**Tests**: Automated tests are required for every completed behavior. Write or update tests before implementation changes when the behavior is testable upfront.

**Organization**: Tasks are grouped by user story so each story can be implemented and verified independently.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or does not depend on incomplete tasks.
- **[Story]**: Maps the task to a user story from `spec.md`.
- Every task includes an exact file path.

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm the existing .NET 8 CLI, container, and Makefile workflow remain the implementation boundary.

- [X] T001 Confirm the feature scope and constraints in `specs/003-place-alias-resolution/spec.md`, `specs/003-place-alias-resolution/plan.md`, and `specs/003-place-alias-resolution/contracts/cli.md`
- [X] T002 Confirm no solution, target framework, package, Docker, Compose, or Makefile structure changes are required in `src/TimezoneCli/TimezoneCli.csproj`, `tests/TimezoneCli.Tests/TimezoneCli.Tests.csproj`, `Dockerfile`, `compose.yaml`, and `Makefile`
- [X] T003 [P] Review the existing catalog, resolver, CLI writer, and comparison boundaries in `src/TimezoneCli/Data/place-aliases.json`, `src/TimezoneCli/Domain/PlaceAliasCatalog.cs`, `src/TimezoneCli/Domain/AliasResolver.cs`, `src/TimezoneCli/Cli/CliResultWriter.cs`, and `src/TimezoneCli/Domain/TimeComparisonService.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Extend the catalog model, normalization, and validation that all user stories depend on.

**CRITICAL**: No user story work should begin until this phase is complete.

- [X] T004 [P] Add catalog schema validation tests for required entry fields, supported categories, and invalid timezones in `tests/TimezoneCli.Tests/AliasCatalogTests.cs`
- [X] T005 Add catalog validation tests for duplicate normalized inputs, malformed Mexican postal codes, and alias values that look like five-digit postal codes in `tests/TimezoneCli.Tests/AliasCatalogTests.cs`
- [X] T006 Extend catalog entry data to include input form, normalized input, display name, canonical timezone, and support category in `src/TimezoneCli/Domain/SupportedAlias.cs`
- [X] T007 Update catalog container and validation logic for the new schema, category rules, postal-code rules, and duplicate normalized-input checks in `src/TimezoneCli/Domain/AliasCatalog.cs` and `src/TimezoneCli/Domain/PlaceAliasCatalog.cs`
- [X] T008 Update deterministic normalization for casing, surrounding whitespace, repeated whitespace, punctuation separators, and accents using .NET `System.Globalization` Unicode normalization without external dependencies in `src/TimezoneCli/Domain/PlaceAliasCatalog.cs`
- [X] T009 Add a regression test proving the legacy `alias`/`normalizedAlias` catalog shape is rejected with clear validation errors in `tests/TimezoneCli.Tests/AliasCatalogTests.cs`
- [X] T010 Update the existing city alias entries to the new versioned catalog shape with category `alias` in `src/TimezoneCli/Data/place-aliases.json`
- [X] T011 Run the foundational catalog validation tests through `make test` using `Makefile`

**Checkpoint**: Catalog data can be loaded and validated deterministically before story-specific behavior is implemented.

---

## Phase 3: User Story 1 - Resolve Supported Place Aliases (Priority: P1) MVP

**Goal**: A supported city alias resolves to the canonical IANA timezone for lookup and comparison while preserving existing output labels.

**Independent Test**: Run alias lookup and alias comparison tests and verify `Place` uses the catalog display name while `Timezone` uses the canonical IANA timezone.

### Tests for User Story 1

- [X] T012 [P] [US1] Add alias lookup tests covering all existing catalog aliases and canonical timezone outputs in `tests/TimezoneCli.Tests/AliasResolverTests.cs`
- [X] T013 [US1] Add alias normalization tests for case, surrounding whitespace, repeated whitespace, punctuation separators, and accents in `tests/TimezoneCli.Tests/AliasResolverTests.cs`
- [X] T014 [P] [US1] Add alias comparison preservation tests for requested and compared aliases in `tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs`
- [X] T015 [P] [US1] Add output label tests proving alias lookup and comparison keep `Place` and `Timezone` semantics in `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

### Implementation for User Story 1

- [X] T016 [US1] Update alias resolution to use the new catalog entry fields and category `alias` in `src/TimezoneCli/Domain/AliasResolver.cs`
- [X] T017 [US1] Preserve valid IANA timezone identifiers as the first canonical resolution path in `src/TimezoneCli/Domain/AliasResolver.cs`
- [X] T018 [US1] Preserve alias lookup output labels through resolved display name and canonical timezone values in `src/TimezoneCli/Cli/CliResultWriter.cs`
- [X] T019 [US1] Verify alias comparison still flows through resolved canonical timezones in `src/TimezoneCli/Domain/TimeComparisonService.cs`
- [X] T020 [US1] Run User Story 1 tests through `make test` using `Makefile`

**Checkpoint**: User Story 1 is independently functional and provides the MVP.

---

## Phase 4: User Story 2 - Resolve Selected Mexican Postal Codes (Priority: P2)

**Goal**: Supported Mexican postal codes `01000`, `64000`, and `44100` resolve from the local catalog, while other five-digit postal-code-like inputs remain explicitly unsupported.

**Independent Test**: Run postal-code lookup, comparison, unsupported-code, and output tests and verify exit codes `0` and `2` match the CLI contract.

### Tests for User Story 2

- [X] T021 [P] [US2] Add supported Mexican postal-code lookup tests for `01000`, `64000`, and `44100` in `tests/TimezoneCli.Tests/AliasResolverTests.cs`
- [X] T022 [P] [US2] Add postal-code comparison tests preserving time difference, working-hours status, meeting suitability, and custom working-hours windows in `tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs`
- [X] T023 [P] [US2] Add unsupported Mexican postal-code error text and exit-code tests for `99999` in `tests/TimezoneCli.Tests/CliResultWriterTests.cs`
- [X] T024 [P] [US2] Add leading-zero postal-code validation and resolution tests in `tests/TimezoneCli.Tests/AliasCatalogTests.cs`

### Implementation for User Story 2

- [X] T025 [US2] Add catalog entries for `01000`, `64000`, and `44100` with category `mexican-postal-code` in `src/TimezoneCli/Data/place-aliases.json`
- [X] T026 [US2] Update resolution order so supported postal-code catalog matches resolve before unsupported five-digit postal-code failures in `src/TimezoneCli/Domain/AliasResolver.cs`
- [X] T027 [US2] Update `ResolvedPlaceSource` to include `SupportedMexicanPostalCode` if missing and ensure resolved postal-code places use that source in `src/TimezoneCli/Domain/PlaceResolution.cs` and `src/TimezoneCli/Domain/ResolvedPlace.cs`
- [X] T028 [US2] Update unsupported Mexican postal-code error output to list `01000`, `64000`, and `44100` exactly as specified in `src/TimezoneCli/Cli/CliResultWriter.cs`
- [X] T029 [US2] Verify unsupported Mexican postal codes map to exit code `2` in `src/TimezoneCli/Cli/ExitCodes.cs`
- [X] T030 [US2] Run User Story 2 tests through `make test` using `Makefile`

**Checkpoint**: User Stories 1 and 2 both work independently and preserve existing comparison behavior.

---

## Phase 5: User Story 3 - Understand Unknown And Ambiguous Inputs (Priority: P3)

**Goal**: Unknown, unsupported, ambiguous, and invalid inputs fail explicitly without guessing, and working-hours validation keeps precedence.

**Independent Test**: Run unknown, unsupported, ambiguous, invalid timezone, and working-hours precedence tests and verify error text and exit codes `1`, `2`, `3`, and `4`.

### Tests for User Story 3

- [X] T031 [P] [US3] Add unknown alias tests for lookup and comparison error category `UnknownPlace` in `tests/TimezoneCli.Tests/AliasResolverTests.cs`
- [X] T032 [US3] Add ambiguous catalog match tests proving known matches are listed and no timezone is selected in `tests/TimezoneCli.Tests/AliasResolverTests.cs`
- [X] T033 [P] [US3] Add CLI error output tests for unknown, ambiguous, unsupported postal-code, and invalid timezone messages in `tests/TimezoneCli.Tests/CliResultWriterTests.cs`
- [X] T034 [P] [US3] Add working-hours precedence tests where invalid working-hours arguments fail before alias, postal-code, unknown, or ambiguous place-resolution errors in `tests/TimezoneCli.Tests/CliParserTests.cs`

### Implementation for User Story 3

- [X] T035 [US3] Preserve unknown-place and invalid-timezone classification after catalog resolution fails in `src/TimezoneCli/Domain/AliasResolver.cs`
- [X] T036 [US3] Preserve ambiguous-place classification and known-match formatting after duplicate in-memory matches in `src/TimezoneCli/Domain/AliasResolver.cs`
- [X] T037 [US3] Preserve working-hours argument validation precedence before place-resolution execution in `src/TimezoneCli/Cli/CliParser.cs` and `src/TimezoneCli/Program.cs`
- [X] T038 [US3] Verify exit-code mappings for invalid, unsupported, unknown, and ambiguous inputs in `src/TimezoneCli/Cli/ExitCodes.cs`
- [X] T039 [US3] Run User Story 3 tests through `make test` using `Makefile`

**Checkpoint**: All failure categories are explicit, deterministic, and test-covered.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Validate documentation, quickstart scenarios, performance, and required quality gates.

- [X] T040 [P] Update command examples and supported alias/postal-code behavior in `README.md`
- [X] T041 [P] Add or update performance smoke coverage for supported alias and supported Mexican postal-code lookup under the existing workflow target in `tests/TimezoneCli.Tests/PerformanceSmokeTests.cs`
- [X] T042 Validate supported alias quickstart manually through `make dev ARGS="--place 'mexico city'"` using `specs/003-place-alias-resolution/quickstart.md`
- [X] T043 Validate supported Mexican postal-code quickstart manually through `make dev ARGS="--place 01000"` using `specs/003-place-alias-resolution/quickstart.md`
- [X] T044 Validate unsupported Mexican postal-code quickstart manually through `make dev ARGS="--place 99999"` using `specs/003-place-alias-resolution/quickstart.md`
- [X] T045 Run full automated test suite through `make test` using `Makefile`
- [X] T046 Run formatting verification through `make lint` using `Makefile`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies.
- **Foundational (Phase 2)**: Depends on Setup and blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational and is the MVP.
- **User Story 2 (Phase 4)**: Depends on Foundational; should be implemented after US1 in the default sequence because it reuses the catalog schema and resolver path.
- **User Story 3 (Phase 5)**: Depends on Foundational; can be implemented after US1/US2 to validate all failure categories.
- **Polish (Phase 6)**: Depends on all selected user stories.

### User Story Dependencies

- **US1**: Can start after Foundational; no dependency on US2 or US3.
- **US2**: Can start after Foundational; uses the same catalog and resolver foundation as US1.
- **US3**: Can start after Foundational; most useful after US1 and US2 behaviors exist because it validates their failure modes.

### Within Each User Story

- Write tests first and confirm they fail when practical.
- Update domain model and resolver behavior before CLI output behavior.
- Preserve CLI labels, argument shape, and exit-code semantics.
- Run `make test` at each story checkpoint.

---

## Parallel Opportunities

- T003 can run in parallel with T001 and T002 because it is read-only review.
- T004 and T005 can run before T006-T010 because they define expected validation behavior in tests.
- T012-T015 can be written in parallel because they target separate behavior slices for US1.
- T021-T024 can be written in parallel because they target postal-code lookup, comparison, output, and catalog validation separately.
- T031-T034 can be written in parallel because they target separate error categories and validation precedence.
- T040 and T041 can run in parallel during polish because they touch documentation and tests separately.

---

## Parallel Example: User Story 1

```bash
Task: "T012 [P] [US1] Add alias lookup tests in tests/TimezoneCli.Tests/AliasResolverTests.cs"
Task: "T014 [P] [US1] Add alias comparison preservation tests in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs"
Task: "T015 [P] [US1] Add output label tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
```

## Parallel Example: User Story 2

```bash
Task: "T021 [P] [US2] Add supported Mexican postal-code lookup tests in tests/TimezoneCli.Tests/AliasResolverTests.cs"
Task: "T022 [P] [US2] Add postal-code comparison tests in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs"
Task: "T023 [P] [US2] Add unsupported postal-code output tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
```

## Parallel Example: User Story 3

```bash
Task: "T031 [P] [US3] Add unknown alias tests in tests/TimezoneCli.Tests/AliasResolverTests.cs"
Task: "T033 [P] [US3] Add CLI error output tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
Task: "T034 [P] [US3] Add working-hours precedence tests in tests/TimezoneCli.Tests/CliParserTests.cs"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1 and Phase 2.
2. Complete Phase 3 for supported aliases.
3. Validate with `make test`.
4. Stop if only the MVP is desired.

### Incremental Delivery

1. Complete shared catalog schema and validation.
2. Deliver US1 alias resolution and output preservation.
3. Deliver US2 selected Mexican postal codes and unsupported-code messaging.
4. Deliver US3 unknown, ambiguous, invalid, and precedence behavior.
5. Run `make test` and `make lint` before closing work.

### Scope Guardrails

- Do not add external APIs, databases, geocoding, fuzzy matching, partial search, prompts, saved preferences, web stacks, background services, or host .NET SDK dependencies.
- Do not rename CLI arguments or add alias-specific output labels.
- Do not change target frameworks, package versions, Docker workflow, Compose workflow, or Makefile workflow unless a later spec explicitly requires it.
