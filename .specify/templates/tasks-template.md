---

description: "Task list template for feature implementation"
---

# Tasks: [FEATURE NAME]

**Input**: Design documents from `/specs/[###-feature-name]/`

**Prerequisites**: plan.md (required), spec.md (required for user stories), research.md, data-model.md, contracts/

**Tests**: Automated tests are REQUIRED for every completed behavior. Include
test tasks for success paths, invalid input, output/error text, and exit codes
that the feature changes.

**Organization**: Tasks are grouped by user story to enable independent implementation and testing of each story.

## Format: `[ID] [P?] [Story] Description`

- **[P]**: Can run in parallel (different files, no dependencies)
- **[Story]**: Which user story this task belongs to (e.g., US1, US2, US3)
- Include exact file paths in descriptions

## Path Conventions

- **CLI lab**: `src/TimezoneCli/`, `tests/TimezoneCli.Tests/`
- **Single project**: `src/`, `tests/` at repository root
- **Web app**: `backend/src/`, `frontend/src/`
- **Mobile**: `api/src/`, `ios/src/` or `android/src/`
- Paths shown below assume single project - adjust based on plan.md structure

<!--
  ============================================================================
  IMPORTANT: The tasks below are SAMPLE TASKS for illustration purposes only.

  The /speckit-tasks command MUST replace these with actual tasks based on:
  - User stories from spec.md (with their priorities P1, P2, P3...)
  - Feature requirements from plan.md
  - Entities from data-model.md
  - Endpoints from contracts/

  Tasks MUST be organized by user story so each story can be:
  - Implemented independently
  - Tested independently
  - Delivered as an MVP increment

  DO NOT keep these sample tasks in the generated tasks.md file.
  ============================================================================
-->

## Phase 1: Setup (Shared Infrastructure)

**Purpose**: Project initialization and basic structure

- [ ] T001 Confirm implementation uses existing `Makefile`, Docker Compose, and .NET 8 CLI structure
- [ ] T002 Identify domain logic files and console input/output boundary files in the implementation plan
- [ ] T003 [P] Confirm existing linting and formatting validation remains runnable through `make lint`

---

## Phase 2: Foundational (Blocking Prerequisites)

**Purpose**: Core infrastructure that MUST be complete before ANY user story can be implemented

**⚠️ CRITICAL**: No user story work can begin until this phase is complete

Examples of foundational tasks (adjust based on your project):

- [ ] T004 Define reusable domain behavior separate from console input/output
- [ ] T005 [P] Define CLI argument validation and failure semantics
- [ ] T006 [P] Define output/error text expectations for changed behavior
- [ ] T007 Create shared models/entities that all stories depend on, if required
- [ ] T008 Configure explicit error handling for invalid input paths
- [ ] T009 Verify no unapproved database, service, web stack, or infrastructure work is included

**Checkpoint**: Foundation ready - user story implementation can now begin in parallel

---

## Phase 3: User Story 1 - [Title] (Priority: P1) 🎯 MVP

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 1 (REQUIRED)

> **NOTE: Write these tests FIRST, ensure they FAIL before implementation**

- [ ] T010 [P] [US1] Domain behavior test in tests/TimezoneCli.Tests/[name]Tests.cs
- [ ] T011 [P] [US1] CLI input/output and exit-code test in tests/TimezoneCli.Tests/[name]CliTests.cs

### Implementation for User Story 1

- [ ] T012 [P] [US1] Create or update domain model in src/TimezoneCli/[path]
- [ ] T013 [P] [US1] Create or update domain service in src/TimezoneCli/[path]
- [ ] T014 [US1] Wire domain service to CLI boundary in src/TimezoneCli/[path]
- [ ] T015 [US1] Implement expected stdout behavior
- [ ] T016 [US1] Add validation, stderr messages, and non-zero exit codes for invalid input
- [ ] T017 [US1] Run `make test` for User Story 1

**Checkpoint**: At this point, User Story 1 should be fully functional and testable independently

---

## Phase 4: User Story 2 - [Title] (Priority: P2)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 2 (REQUIRED)

- [ ] T018 [P] [US2] Domain behavior test in tests/TimezoneCli.Tests/[name]Tests.cs
- [ ] T019 [P] [US2] CLI input/output and exit-code test in tests/TimezoneCli.Tests/[name]CliTests.cs

### Implementation for User Story 2

- [ ] T020 [P] [US2] Create or update domain model in src/TimezoneCli/[path]
- [ ] T021 [US2] Implement domain service in src/TimezoneCli/[path]
- [ ] T022 [US2] Wire CLI behavior in src/TimezoneCli/[path]
- [ ] T023 [US2] Integrate with User Story 1 components (if needed)

**Checkpoint**: At this point, User Stories 1 AND 2 should both work independently

---

## Phase 5: User Story 3 - [Title] (Priority: P3)

**Goal**: [Brief description of what this story delivers]

**Independent Test**: [How to verify this story works on its own]

### Tests for User Story 3 (REQUIRED)

- [ ] T024 [P] [US3] Domain behavior test in tests/TimezoneCli.Tests/[name]Tests.cs
- [ ] T025 [P] [US3] CLI input/output and exit-code test in tests/TimezoneCli.Tests/[name]CliTests.cs

### Implementation for User Story 3

- [ ] T026 [P] [US3] Create or update domain model in src/TimezoneCli/[path]
- [ ] T027 [US3] Implement domain service in src/TimezoneCli/[path]
- [ ] T028 [US3] Wire CLI behavior in src/TimezoneCli/[path]

**Checkpoint**: All user stories should now be independently functional

---

[Add more user story phases as needed, following the same pattern]

---

## Phase N: Polish & Cross-Cutting Concerns

**Purpose**: Improvements that affect multiple user stories

- [ ] TXXX [P] Documentation updates in docs/
- [ ] TXXX Code cleanup and refactoring
- [ ] TXXX Performance optimization across all stories
- [ ] TXXX [P] Additional automated tests for edge cases in tests/TimezoneCli.Tests/
- [ ] TXXX Security hardening
- [ ] TXXX Run quickstart.md validation
- [ ] TXXX Run `make test`
- [ ] TXXX Run `make lint`

---

## Dependencies & Execution Order

### Phase Dependencies

- **Setup (Phase 1)**: No dependencies - can start immediately
- **Foundational (Phase 2)**: Depends on Setup completion - BLOCKS all user stories
- **User Stories (Phase 3+)**: All depend on Foundational phase completion
  - User stories can then proceed in parallel (if staffed)
  - Or sequentially in priority order (P1 → P2 → P3)
- **Polish (Final Phase)**: Depends on all desired user stories being complete

### User Story Dependencies

- **User Story 1 (P1)**: Can start after Foundational (Phase 2) - No dependencies on other stories
- **User Story 2 (P2)**: Can start after Foundational (Phase 2) - May integrate with US1 but should be independently testable
- **User Story 3 (P3)**: Can start after Foundational (Phase 2) - May integrate with US1/US2 but should be independently testable

### Within Each User Story

- Tests MUST be written and fail before implementation where behavior changes are testable upfront
- Models before services
- Services before CLI boundary wiring
- Core implementation before integration
- Story complete before moving to next priority

### Parallel Opportunities

- All Setup tasks marked [P] can run in parallel
- All Foundational tasks marked [P] can run in parallel (within Phase 2)
- Once Foundational phase completes, all user stories can start in parallel (if team capacity allows)
- All tests for a user story marked [P] can run in parallel
- Models within a story marked [P] can run in parallel
- Different user stories can be worked on in parallel by different team members

---

## Parallel Example: User Story 1

```bash
# Launch all tests for User Story 1 together:
Task: "Domain behavior test in tests/TimezoneCli.Tests/[name]Tests.cs"
Task: "CLI input/output and exit-code test in tests/TimezoneCli.Tests/[name]CliTests.cs"

# Launch all domain updates for User Story 1 together:
Task: "Create or update domain model in src/TimezoneCli/[path]"
Task: "Create or update domain service in src/TimezoneCli/[path]"
```

---

## Implementation Strategy

### MVP First (User Story 1 Only)

1. Complete Phase 1: Setup
2. Complete Phase 2: Foundational (CRITICAL - blocks all stories)
3. Complete Phase 3: User Story 1
4. **STOP and VALIDATE**: Test User Story 1 independently
5. Deploy/demo if ready

### Incremental Delivery

1. Complete Setup + Foundational → Foundation ready
2. Add User Story 1 → Test independently → Deploy/Demo (MVP!)
3. Add User Story 2 → Test independently → Deploy/Demo
4. Add User Story 3 → Test independently → Deploy/Demo
5. Each story adds value without breaking previous stories

### Parallel Team Strategy

With multiple developers:

1. Team completes Setup + Foundational together
2. Once Foundational is done:
   - Developer A: User Story 1
   - Developer B: User Story 2
   - Developer C: User Story 3
3. Stories complete and integrate independently

---

## Notes

- [P] tasks = different files, no dependencies
- [Story] label maps task to specific user story for traceability
- Each user story MUST be independently completable and testable
- Verify tests fail before implementing
- Verify final work with `make test` and `make lint`
- Commit after each task or logical group
- Stop at any checkpoint to validate story independently
- Avoid: vague tasks, same file conflicts, cross-story dependencies that break independence
