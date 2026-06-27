# Tasks: Timezone Meeting Planner

**Input**: Design documents from `specs/001-timezone-meeting-planner/`

**Prerequisites**: `plan.md`, `spec.md`, `research.md`, `data-model.md`, `contracts/cli.md`, `quickstart.md`

**Tests**: Automated tests are REQUIRED for every completed behavior. Each user story includes test tasks before implementation tasks and must be validated through `make test`.

**Organization**: Tasks are grouped by user story so each story can be implemented and tested independently after the shared foundation is complete.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or independent concerns
- **[Story]**: User story traceability marker: `[US1]`, `[US2]`, or `[US3]`
- Every task includes an exact repository path or project validation target

---

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Prepare the existing .NET 8 CLI lab structure without changing the container-first workflow.

- [X] T001 Confirm the existing Makefile and Docker Compose workflow remains the only required execution path in `Makefile` and `compose.yaml`
- [X] T002 [P] Create planned CLI, domain, and data directories in `src/TimezoneCli/Cli/`, `src/TimezoneCli/Domain/`, and `src/TimezoneCli/Data/`
- [X] T003 [P] Replace the placeholder test entry point with feature-specific test files by removing or superseding `tests/TimezoneCli.Tests/UnitTest1.cs`
- [X] T004 [P] Add a deterministic test clock helper for current-moment behavior in `tests/TimezoneCli.Tests/FixedTimeProvider.cs`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Establish shared models, catalog loading, and CLI boundary types used by all user stories.

**CRITICAL**: No user story work should begin until this phase is complete.

- [X] T005 [P] Add alias catalog validation tests for schema, required fields, duplicate normalized aliases, and valid IANA timezone identifiers in `tests/TimezoneCli.Tests/AliasCatalogTests.cs`
- [X] T006 [P] Define shared domain value types for place input, aliases, resolved places, comparisons, and working-hours assessments in `src/TimezoneCli/Domain/PlaceInput.cs`, `src/TimezoneCli/Domain/SupportedAlias.cs`, `src/TimezoneCli/Domain/AliasCatalog.cs`, `src/TimezoneCli/Domain/ResolvedPlace.cs`, `src/TimezoneCli/Domain/TimeComparison.cs`, and `src/TimezoneCli/Domain/WorkingHoursAssessment.cs`
- [X] T007 [P] Define CLI boundary types and documented exit code constants in `src/TimezoneCli/Cli/CliOptions.cs`, `src/TimezoneCli/Cli/CliResult.cs`, and `src/TimezoneCli/Cli/ExitCodes.cs`
- [X] T008 Implement local alias catalog loading and deterministic validation in `src/TimezoneCli/Domain/PlaceAliasCatalog.cs`
- [X] T009 Add the initial versioned curated alias catalog in `src/TimezoneCli/Data/place-aliases.json` with exactly these v1 seed aliases: `mexico city` -> `America/Mexico_City`, `london` -> `Europe/London`, `new york` -> `America/New_York`, and `tokyo` -> `Asia/Tokyo`
- [X] T010 Add shared resolution result/error types for invalid, unsupported, unknown, ambiguous, and success outcomes in `src/TimezoneCli/Domain/ResolutionResult.cs`

**Checkpoint**: Foundation ready. User story implementation can now begin.

---

## Phase 3: User Story 1 - Look Up Local Time (Priority: P1) MVP

**Goal**: A user provides one IANA timezone identifier or supported alias and receives stable labeled local date, local time, and UTC offset output.

**Independent Test**: Run the CLI with `--place America/Mexico_City` and verify stdout contains `Place`, `Timezone`, `Local date`, `Local time`, and `UTC offset` with exit code `0`.

### Tests for User Story 1 (REQUIRED)

- [X] T011 [P] [US1] Add IANA timezone resolution tests, including display name and canonical timezone output, in `tests/TimezoneCli.Tests/TimezoneResolverTests.cs`
- [X] T012 [P] [US1] Add current-moment local date, local time, and UTC offset tests using the fixed clock helper in `tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs`
- [X] T013 [P] [US1] Add successful lookup label, field-presence, and value-format tests for `YYYY-MM-DD`, `HH:mm`, and `+HH:mm|-HH:mm` in `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

### Implementation for User Story 1

- [X] T014 [P] [US1] Implement canonical IANA timezone resolution without external APIs in `src/TimezoneCli/Domain/TimezoneResolver.cs`
- [X] T015 [US1] Implement single-place current-time lookup using injectable current-time behavior in `src/TimezoneCli/Domain/TimeComparisonService.cs`
- [X] T016 [US1] Implement successful lookup stdout formatting with exact labels in `src/TimezoneCli/Cli/CliResultWriter.cs`
- [X] T017 [US1] Wire `--place` lookup execution through the CLI boundary in `src/TimezoneCli/Program.cs`
- [X] T018 [US1] Validate User Story 1 behavior through `make test` covering `tests/TimezoneCli.Tests/TimezoneResolverTests.cs`, `tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs`, and `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

**Checkpoint**: User Story 1 is independently functional and testable.

---

## Phase 4: User Story 2 - Compare Two Places for a Meeting (Priority: P2)

**Goal**: A user compares one requested place with one comparison place and sees both local times, both UTC offsets, signed time difference, per-place working-hours status, and combined meeting suitability.

**Independent Test**: Run the CLI with `--place America/Mexico_City --compare Europe/London` and verify comparison labels, signed time difference, working-hours statuses, and exit code `0`.

### Tests for User Story 2 (REQUIRED)

- [X] T019 [P] [US2] Add comparison tests for same timezone, different timezone, different local date, and signed time difference in `tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs`
- [X] T020 [P] [US2] Add working-hours tests for weekday 09:00 inclusive, weekday 17:00 exclusive, after-hours, and weekend behavior in `tests/TimezoneCli.Tests/WorkingHoursPolicyTests.cs`
- [X] T021 [P] [US2] Add successful comparison label, field-presence, and value-format tests for `within|outside`, signed `Time difference`, and `suitable|not suitable` in `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

### Implementation for User Story 2

- [X] T022 [P] [US2] Implement the v1 Monday-Friday 09:00 inclusive to before 17:00 policy in `src/TimezoneCli/Domain/WorkingHoursPolicy.cs`
- [X] T023 [US2] Extend comparison behavior for two resolved places, signed time difference, and combined suitability in `src/TimezoneCli/Domain/TimeComparisonService.cs`
- [X] T024 [US2] Extend stdout formatting with exact comparison labels in `src/TimezoneCli/Cli/CliResultWriter.cs`
- [X] T025 [US2] Wire optional `--compare` execution and at-most-one comparison support in `src/TimezoneCli/Program.cs`
- [X] T026 [US2] Validate User Story 2 behavior through `make test` covering `tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs`, `tests/TimezoneCli.Tests/WorkingHoursPolicyTests.cs`, and `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

**Checkpoint**: User Stories 1 and 2 are independently functional and testable.

---

## Phase 5: User Story 3 - Understand Invalid or Ambiguous Input (Priority: P3)

**Goal**: Invalid, unsupported, unknown, missing, too-many, and ambiguous inputs fail explicitly with clear stderr messages and the documented non-zero exit codes.

**Independent Test**: Run malformed, unknown, Mexican postal-code, missing `--place`, duplicate `--compare`, and ambiguous alias cases and verify stderr text plus exit codes `1`, `2`, `3`, or `4` as documented.

### Tests for User Story 3 (REQUIRED)

- [X] T027 [P] [US3] Add CLI parser tests for missing `--place`, malformed arguments, and too many comparison values in `tests/TimezoneCli.Tests/CliParserTests.cs`
- [X] T028 [P] [US3] Add alias resolution tests for supported aliases, Mexican postal-code rejection, unknown input, ambiguous aliases with known matches, and ambiguous aliases without safe matches in `tests/TimezoneCli.Tests/AliasResolverTests.cs`
- [X] T029 [P] [US3] Add stderr message and exit-code mapping tests for invalid, unsupported, unknown, and ambiguous input in `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

### Implementation for User Story 3

- [X] T030 [P] [US3] Implement non-interactive argument parsing and validation in `src/TimezoneCli/Cli/CliParser.cs`
- [X] T031 [US3] Implement alias, unsupported postal-code, unknown, and ambiguous input resolution in `src/TimezoneCli/Domain/AliasResolver.cs`
- [X] T032 [US3] Implement documented stderr messages and exit-code mapping in `src/TimezoneCli/Cli/CliResultWriter.cs` and `src/TimezoneCli/Cli/ExitCodes.cs`
- [X] T033 [US3] Wire all failure paths through `src/TimezoneCli/Program.cs` without falling back to guessed timezones or interactive prompts
- [X] T034 [US3] Validate User Story 3 behavior through `make test` covering `tests/TimezoneCli.Tests/CliParserTests.cs`, `tests/TimezoneCli.Tests/AliasResolverTests.cs`, and `tests/TimezoneCli.Tests/CliResultWriterTests.cs`

**Checkpoint**: All user stories are independently functional and explicitly tested.

---

## Phase 6: Polish & Cross-Cutting Validation

**Purpose**: Confirm documentation, reproducibility, and project gates before closing implementation work.

- [X] T035 [P] Update usage examples, supported inputs, unsupported Mexican postal-code behavior, and exit codes in `README.md`
- [X] T036 [P] Validate quickstart commands and expected output examples against the implemented behavior in `specs/001-timezone-meeting-planner/quickstart.md`
- [X] T037 [P] Verify no external API, database, background service, web stack, or network dependency was introduced in `src/TimezoneCli/TimezoneCli.csproj`
- [X] T038 Add automated smoke tests confirming lookup completes under 10 seconds and comparison completes under 15 seconds in `tests/TimezoneCli.Tests/PerformanceSmokeTests.cs`
- [X] T039 Run full automated tests through `make test` for `tests/TimezoneCli.Tests/TimezoneCli.Tests.csproj`
- [x] T040 Run formatting and lint validation through `make lint` for `src/TimezoneCli/TimezoneCli.csproj` and `tests/TimezoneCli.Tests/TimezoneCli.Tests.csproj`
- [x] T041 Review the CLI requirements checklist against the final implementation in `specs/001-timezone-meeting-planner/checklists/cli-requirements.md`

---

## Dependencies & Execution Order

### Phase Dependencies

- Phase 1 must complete before Phase 2.
- Phase 2 blocks all user story work.
- Phase 3 is the MVP and should be completed before broader validation.
- Phase 4 can start after Phase 2, but it integrates naturally with the lookup components from Phase 3.
- Phase 5 can start after Phase 2, but final wiring should be checked against both success paths from Phase 3 and Phase 4.
- Phase 6 depends on all selected user stories being complete.

### User Story Dependencies

- **US1 Look Up Local Time**: Depends only on the shared foundation.
- **US2 Compare Two Places**: Depends on the shared foundation and reuses lookup/resolution behavior; keep tests independent with fixed clock inputs.
- **US3 Invalid or Ambiguous Input**: Depends on the shared foundation and must validate both lookup-only and comparison invocation paths.

### Within Each User Story

- Write the listed tests first and verify they fail before implementation where behavior is not already present.
- Implement domain behavior before CLI formatting and `Program.cs` wiring.
- Keep CLI parsing/output separate from domain services so behavior remains testable without terminal coupling.
- Run the story-specific `make test` validation task before moving to the next story checkpoint.

---

## Parallel Opportunities

- T002, T003, and T004 can run in parallel after T001.
- T005, T006, and T007 can run in parallel because they cover tests, domain types, and CLI types separately.
- US1 test tasks T011, T012, and T013 can run in parallel.
- US2 test tasks T019, T020, and T021 can run in parallel.
- US3 test tasks T027, T028, and T029 can run in parallel.
- T014 and T022 can run in parallel after the foundation because timezone resolution and working-hours policy are separate domain services.
- T035, T036, and T037 can run in parallel during final polish.

---

## Parallel Example: User Story 1

```bash
Task: "T011 [US1] Add IANA timezone resolution tests in tests/TimezoneCli.Tests/TimezoneResolverTests.cs"
Task: "T012 [US1] Add current-moment lookup tests in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs"
Task: "T013 [US1] Add lookup output label tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
```

---

## Implementation Strategy

### MVP First

1. Complete Phase 1 setup.
2. Complete Phase 2 foundation.
3. Complete Phase 3 for lookup-only behavior.
4. Validate US1 independently with `make test`.

### Incremental Delivery

1. Deliver US1 lookup behavior as the MVP.
2. Add US2 comparison and working-hours behavior without changing US1 labels.
3. Add US3 explicit failure semantics and exit codes without changing success output labels.
4. Finish Phase 6 and close only after `make test` and `make lint` pass.

### Scope Control

- Do not add external APIs, databases, background services, web stacks, calendar workflows, or network access.
- Do not require a host .NET SDK; keep validation through Makefile and Docker Compose.
- Treat alias data as a fixed versioned repository artifact and validate changes deterministically.

---

## Phase 7: Convergence

- [X] T042 Add parser implementation and tests in `src/TimezoneCli/Cli/CliParser.cs` and `tests/TimezoneCli.Tests/CliParserTests.cs` so extra positional comparison values after `--compare`, such as `--place A --compare B C`, return the documented too-many-comparison error per FR-010 and `specs/001-timezone-meeting-planner/contracts/cli.md` (partial)
