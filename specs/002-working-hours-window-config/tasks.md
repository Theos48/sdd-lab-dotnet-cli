# Tasks: Working Hours Window Configuration

**Input**: Design documents from `specs/002-working-hours-window-config/`

**Prerequisites**: [plan.md](./plan.md), [spec.md](./spec.md), [research.md](./research.md), [data-model.md](./data-model.md), [contracts/cli.md](./contracts/cli.md), [quickstart.md](./quickstart.md)

**Tests**: Automated tests are REQUIRED for every completed behavior by project constitution and feature success criteria.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel because it touches different files or only reads context
- **[Story]**: User story label for story phases only: [US1], [US2], [US3]
- Every task includes exact file paths

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Confirm the existing project shape and avoid introducing extra infrastructure.

- [X] T001 Review feature requirements and implementation constraints in specs/002-working-hours-window-config/spec.md, specs/002-working-hours-window-config/plan.md, and specs/002-working-hours-window-config/contracts/cli.md
- [X] T002 [P] Review existing CLI parsing, options, output, and exit-code boundaries in src/TimezoneCli/Cli/CliOptions.cs, src/TimezoneCli/Cli/CliParser.cs, src/TimezoneCli/Cli/CliResultWriter.cs, and src/TimezoneCli/Cli/ExitCodes.cs
- [X] T003 [P] Review existing domain working-hours and comparison behavior in src/TimezoneCli/Domain/WorkingHoursPolicy.cs, src/TimezoneCli/Domain/TimeComparison.cs, and src/TimezoneCli/Domain/TimeComparisonService.cs

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Add the shared working-hours window model and default/custom assessment behavior used by all stories.

**CRITICAL**: No user story work can begin until this phase is complete.

- [X] T004 [P] Add domain tests for strict `HH:mm` parsing, default `09:00-17:00`, inclusive start, exclusive end, equal range rejection, and overnight range rejection in tests/TimezoneCli.Tests/WorkingHoursWindowTests.cs
- [X] T005 Add `WorkingHoursWindow` domain model with default window, strict parsing, formatting, and range validation in src/TimezoneCli/Domain/WorkingHoursWindow.cs
- [X] T006 [P] Update working-hours policy tests for custom windows and preserved weekend behavior in tests/TimezoneCli.Tests/WorkingHoursPolicyTests.cs
- [X] T007 Update `WorkingHoursPolicy` to assess a `ResolvedPlace` against a provided `WorkingHoursWindow` while preserving default behavior in src/TimezoneCli/Domain/WorkingHoursPolicy.cs
- [X] T008 Update `TimeComparison` to carry the active `WorkingHoursWindow` used for assessment in src/TimezoneCli/Domain/TimeComparison.cs
- [X] T009 Run foundational automated tests through `make test` using Makefile

**Checkpoint**: Foundation ready - user story implementation can now begin.

---

## Phase 3: User Story 1 - Compare Places With Custom Working Hours (Priority: P1) MVP

**Goal**: A user compares two valid places using `--working-hours-start <HH:mm>` and `--working-hours-end <HH:mm>`, and suitability is evaluated with that custom window in each place's local time.

**Independent Test**: Compare two valid places with a custom window and verify `Working hours`, `Compared working hours`, `Meeting suitability`, and `Working hours window` all reflect the custom window.

### Tests for User Story 1 (REQUIRED)

- [X] T010 [P] [US1] Add comparison service tests proving a custom `WorkingHoursWindow` applies to both requested and compared places in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs
- [X] T011 [P] [US1] Add parser tests for valid separated and equals-form `--working-hours-start` and `--working-hours-end` inputs in tests/TimezoneCli.Tests/CliParserTests.cs
- [X] T012 [P] [US1] Add output tests for `Working hours window: 08:30-16:45` in successful custom comparisons in tests/TimezoneCli.Tests/CliResultWriterTests.cs

### Implementation for User Story 1

- [X] T013 [US1] Extend `CliOptions` to carry the optional parsed `WorkingHoursWindow` for comparison commands in src/TimezoneCli/Cli/CliOptions.cs
- [X] T014 [US1] Extend `CliParser` to parse valid `--working-hours-start` and `--working-hours-end` separated and equals-form inputs in src/TimezoneCli/Cli/CliParser.cs
- [X] T015 [US1] Update `TimeComparisonService.Compare` to accept and use the active `WorkingHoursWindow` when assessing both places in src/TimezoneCli/Domain/TimeComparisonService.cs
- [X] T016 [US1] Wire parsed custom working-hours options from `Program.cs` into `TimeComparisonService.Compare` in src/TimezoneCli/Program.cs
- [X] T017 [US1] Update comparison output to include `Working hours window: <HH:mm>-<HH:mm>` in src/TimezoneCli/Cli/CliResultWriter.cs
- [X] T018 [US1] Run User Story 1 automated tests through `make test` using Makefile

**Checkpoint**: User Story 1 delivers the MVP custom-window comparison path.

---

## Phase 4: User Story 2 - Preserve Default Comparison Behavior (Priority: P2)

**Goal**: Existing lookup and comparison commands without custom working-hours flags continue to work, with comparison output also showing `Working hours window: 09:00-17:00`.

**Independent Test**: Run an existing valid comparison without custom flags and a lookup without comparison; verify existing labels and exit semantics remain stable while comparison includes the default window label.

### Tests for User Story 2 (REQUIRED)

- [X] T019 [P] [US2] Add comparison service tests proving omitted custom flags use `WorkingHoursWindow.Default` in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs
- [X] T020 [P] [US2] Add output tests proving default comparisons include `Working hours window: 09:00-17:00` and lookup output remains unchanged in tests/TimezoneCli.Tests/CliResultWriterTests.cs
- [X] T021 [P] [US2] Add parser tests proving absent working-hours flags keep existing `--place` and `--compare` behavior unchanged in tests/TimezoneCli.Tests/CliParserTests.cs

### Implementation for User Story 2

- [X] T022 [US2] Ensure `CliParser` leaves `CliOptions` working-hours configuration empty when no custom flags are supplied in src/TimezoneCli/Cli/CliParser.cs
- [X] T023 [US2] Ensure `TimeComparisonService.Compare` uses `WorkingHoursWindow.Default` when no custom window is supplied in src/TimezoneCli/Domain/TimeComparisonService.cs
- [X] T024 [US2] Ensure `CliResultWriter.Lookup` remains unchanged while `CliResultWriter.Comparison` prints the default window label for default comparisons in src/TimezoneCli/Cli/CliResultWriter.cs
- [X] T025 [US2] Run User Story 2 automated tests through `make test` using Makefile

**Checkpoint**: User Stories 1 and 2 both work independently and preserve compatibility.

---

## Phase 5: User Story 3 - Understand Invalid Working-Hours Input (Priority: P3)

**Goal**: Invalid working-hours inputs fail explicitly with clear messages and exit code `1`, and these argument errors take precedence over place-resolution errors.

**Independent Test**: Provide missing-pair flags, lookup-only flags, malformed times, equal ranges, overnight ranges, and invalid working-hours plus invalid place input; verify stderr messages and exit code `1`.

### Tests for User Story 3 (REQUIRED)

- [X] T026 [P] [US3] Add parser tests for missing paired flags, lookup-only flags, malformed times, equal ranges, overnight ranges, and invalid-working-hours precedence in tests/TimezoneCli.Tests/CliParserTests.cs
- [X] T027 [P] [US3] Add error output tests for working-hours missing-pair, lookup-only, malformed-time, and invalid-range messages in tests/TimezoneCli.Tests/CliResultWriterTests.cs
- [X] T028 [P] [US3] Add exit-code mapping tests proving all working-hours argument failures map to exit code `1` in tests/TimezoneCli.Tests/CliResultWriterTests.cs

### Implementation for User Story 3

- [X] T029 [US3] Extend `ResolutionErrorKind` and `ResolutionError` factories for working-hours missing-pair, lookup-only, malformed-time, and invalid-range errors in src/TimezoneCli/Domain/ResolutionResult.cs
- [X] T030 [US3] Implement working-hours argument validation precedence before place resolution in src/TimezoneCli/Cli/CliParser.cs
- [X] T031 [US3] Add working-hours-specific stderr messages in src/TimezoneCli/Cli/CliResultWriter.cs
- [X] T032 [US3] Ensure `ExitCodes.FromErrorKind` maps all working-hours argument errors to `ExitCodes.InvalidInput` in src/TimezoneCli/Cli/ExitCodes.cs
- [X] T033 [US3] Run User Story 3 automated tests through `make test` using Makefile

**Checkpoint**: All user stories are independently functional and invalid input behavior is explicit.

---

## Phase 6: Polish & Cross-Cutting Concerns

**Purpose**: Documentation, checklist cleanup, and required project verification.

- [X] T034 [P] Update user-facing usage examples for custom working-hours flags and output label in README.md
- [X] T035 [P] Review and mark or annotate completed CLI contract requirement-quality items in specs/002-working-hours-window-config/checklists/cli-contract.md
- [X] T036 [P] Validate quickstart scenarios against the implemented CLI behavior in specs/002-working-hours-window-config/quickstart.md
- [X] T037 [P] Extend or validate performance smoke coverage for custom working-hours comparison in tests/TimezoneCli.Tests/PerformanceSmokeTests.cs
- [X] T038 Run the full automated test suite through `make test` using Makefile
- [X] T039 Run formatting validation through `make lint` using Makefile
- [X] T040 Review final implementation scope against no-extra-infrastructure constraints in specs/002-working-hours-window-config/plan.md and AGENTS.md

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies; can start immediately.
- **Foundational (Phase 2)**: Depends on Setup completion; blocks all user stories.
- **User Story 1 (Phase 3)**: Depends on Foundational; MVP.
- **User Story 2 (Phase 4)**: Depends on Foundational and should be validated after US1 integration because it preserves default behavior on the same comparison path.
- **User Story 3 (Phase 5)**: Depends on Foundational and CLI parser/error boundaries; can be implemented after US1 parser changes or in parallel with coordination.
- **Polish (Phase 6)**: Depends on all selected user stories.

### User Story Dependencies

- **US1 (P1)**: No dependency on US2 or US3 after Foundation; delivers custom-window comparison MVP.
- **US2 (P2)**: Uses the same active-window model as US1; validates default fallback and compatibility.
- **US3 (P3)**: Uses the parser/error model from Foundation and the CLI flags from US1; validates invalid-input behavior and precedence.

### Within Each User Story

- Tests first, before implementation.
- Domain/value model before domain service changes.
- Domain service changes before CLI `Program.cs` wiring.
- CLI parser changes before output/error integration.
- Run `make test` at each story checkpoint.

## Parallel Opportunities

- T002 and T003 can run in parallel during setup.
- T004 and T006 can be written in parallel before foundational implementation.
- T010, T011, and T012 can be written in parallel for US1.
- T019, T020, and T021 can be written in parallel for US2.
- T026, T027, and T028 can be written in parallel for US3.
- T034, T035, and T036 can run in parallel during polish.

## Parallel Example: User Story 1

```text
Task: "T010 [US1] Add comparison service tests in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs"
Task: "T011 [US1] Add parser tests in tests/TimezoneCli.Tests/CliParserTests.cs"
Task: "T012 [US1] Add output tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
```

## Parallel Example: User Story 2

```text
Task: "T019 [US2] Add default-window comparison tests in tests/TimezoneCli.Tests/TimeComparisonServiceTests.cs"
Task: "T020 [US2] Add default-window output tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
Task: "T021 [US2] Add parser compatibility tests in tests/TimezoneCli.Tests/CliParserTests.cs"
```

## Parallel Example: User Story 3

```text
Task: "T026 [US3] Add invalid working-hours parser tests in tests/TimezoneCli.Tests/CliParserTests.cs"
Task: "T027 [US3] Add working-hours error output tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
Task: "T028 [US3] Add working-hours exit-code mapping tests in tests/TimezoneCli.Tests/CliResultWriterTests.cs"
```

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup.
2. Complete Phase 2: Foundational.
3. Complete Phase 3: User Story 1.
4. Stop and validate with `make test` using Makefile.
5. Demo custom comparison with `--working-hours-start` and `--working-hours-end`.

### Incremental Delivery

1. Foundation adds `WorkingHoursWindow` and policy support.
2. US1 adds custom-window comparison.
3. US2 verifies default behavior and lookup compatibility.
4. US3 adds explicit invalid-input errors and precedence.
5. Polish updates docs and runs full `make test` plus `make lint`.

### Parallel Team Strategy

1. Complete Setup and Foundation together.
2. Work on US1 and US2 tests in parallel, coordinating shared comparison output files.
3. Work on US3 parser/error tests in parallel once flag names and error kinds are stable.
4. Merge story increments only after each story's tests pass.

## Notes

- [P] tasks = different files or read-only context work with no dependency on incomplete tasks.
- [US1], [US2], and [US3] labels map to the prioritized user stories in spec.md.
- Every completed behavior requires automated tests.
- Keep domain logic separate from console parsing/output.
- Do not install .NET on the host; use Makefile and Docker Compose workflows.
- Do not add databases, background services, web stacks, external APIs, or persistent user configuration.
- Closing implementation work requires `make test` and `make lint`.

## Phase 7: Convergence

- [X] T041 Support forwarding CLI arguments through `make dev` while preserving Docker Compose execution per Constitution II and plan target platform (partial)
