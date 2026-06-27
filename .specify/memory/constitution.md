<!--
Sync Impact Report
Version change: template -> 1.0.0
Modified principles:
- PRINCIPLE_1_NAME -> I. Containerized Reproducibility
- PRINCIPLE_2_NAME -> II. Make-Driven CLI Contract
- PRINCIPLE_3_NAME -> III. Testable Domain Boundaries
- PRINCIPLE_4_NAME -> IV. Explicit Failure Semantics
- PRINCIPLE_5_NAME -> V. Simplicity and Scope Control
Added sections:
- Host and Infrastructure Boundaries
- Development Workflow and Quality Gates
Removed sections:
- Placeholder template comments and undefined sections
Templates requiring updates:
- ✅ updated: .specify/templates/plan-template.md
- ✅ updated: .specify/templates/spec-template.md
- ✅ updated: .specify/templates/tasks-template.md
- ✅ updated: README.md
- ✅ updated: AGENTS.md
Follow-up TODOs: none
-->

# TimezoneCli Constitution

## Core Principles

### I. Containerized Reproducibility

The project MUST remain a minimal, reproducible .NET 8 CLI lab. Build, run,
test, lint, and development flows MUST execute through `make` targets backed by
Docker Compose. The Fedora host MUST NOT require a project .NET SDK or runtime.
This keeps the lab portable, auditable, and free from project-specific host
state.

### II. Make-Driven CLI Contract

`make` is the stable project interface. CLI behavior MUST stay consistent in
arguments, standard output, standard error, and exit-code semantics. Plans and
tasks MUST use existing `Makefile` targets before introducing ad hoc commands.
This creates one repeatable workflow for humans, agents, and automation.

### III. Testable Domain Boundaries

Domain logic MUST remain separate from console input and output. Completed
behavior MUST be covered by automated tests that can validate behavior without
terminal coupling. This preserves fast feedback and prevents presentation
details from hiding business-rule defects.

### IV. Explicit Failure Semantics

Invalid input MUST fail explicitly with clear user-facing messages and non-zero
exit codes. Error behavior MUST be covered by automated tests whenever it is
part of completed behavior. This makes CLI failures predictable for both people
and scripts.

### V. Simplicity and Scope Control

The default implementation path MUST be the simplest design that preserves
clarity, testability, and reproducibility. Features MUST NOT introduce
databases, background services, web stacks, or extra infrastructure unless the
active specification explicitly requires them. When principles conflict,
correctness, testability, and operational simplicity take precedence over speed
of delivery or feature expansion.

## Host and Infrastructure Boundaries

The host environment is reserved for cross-project tools only. Project-specific
runtimes, SDKs, databases, and services belong inside the containerized lab
unless an approved exception is documented. `Dockerfile`, `compose.yaml`,
`global.json`, project files, and `Makefile` define the supported execution
environment. Changes to target frameworks, package versions, solution structure,
volumes, or persistent data require explicit specification or plan justification.

## Development Workflow and Quality Gates

Every implementation plan MUST include a Constitution Check covering:
containerized execution, `make` usage, domain and console separation, automated
test coverage, explicit invalid-input behavior, CLI compatibility, and
infrastructure scope. Every task list MUST include automated tests for completed
behavior and final verification with `make test` and `make lint`. Work MUST NOT
be closed until both commands pass or a blocking issue is documented with the
exact command and failure.

## Governance

This constitution supersedes conflicting project practices and templates.
Specifications and implementation plans MUST justify any exception before code
is merged. Amendments require updating this file, adding a Sync Impact Report,
propagating affected templates or runtime guidance, and incrementing the
constitution version.

Versioning follows semantic versioning for governance changes: MAJOR for
backward-incompatible principle removals or redefinitions, MINOR for new
principles or materially expanded guidance, and PATCH for clarifications that do
not change obligations. Reviews MUST verify constitution compliance before
approval.

**Version**: 1.0.0 | **Ratified**: 2026-06-26 | **Last Amended**: 2026-06-26
