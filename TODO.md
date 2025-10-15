# TODO

TODOs for the project.

Milestone TODOs:
- Should be given a subheading for larger groups of tasks that can be performed independently.
  - Avoid creating groups with only one task unless it is a major task.
  - Groups should be independent of each other, i.e. the tasks can/should be done in any order.
- Within a milestone heading, tasks should be ordered by priority and dependency.

## General
- Allow updates to projects and work items.
- Add all fields to request models.

## Sentinel Milestone – MVP Online
### Worker Foundations
- Document the inline vs dedicated project decision and integration plan for the initial Sentinel worker.
- Implement in-memory versions of `ILogEventSource`, `ILogWorkItemOrchestrator`, and `ILogIngestionPipeline` inside `Ergonaut.App` so downstream components have stable contracts.
- Create the minimal infrastructure adapters (storage, message bus, etc.) against the new interfaces so the first loop can run end-to-end.

### Quality & Guardrails
- Add smoke tests covering log-ingestion happy paths and guard rails to validate the end-to-end loop once adapters exist.

### Visibility & Documentation
- Capture how automation-created work items surface in the UI (refresh cadence, badges, filters) to align UX with the automation path.
- Update README/docs that still mention `Ergonaut.Sentinel` to match the MVP worker naming, hosting model, and workflow.

## Improvements
- Introduce validators for `CreateProjectRequest` / `CreateWorkItemRequest`, extend `WorkItem` with correlation identifiers, and add unit tests covering duplicate-prevention.
- Expand NetArchTest coverage (UI ↛ Infrastructure, Infrastructure ↛ UI, Contracts ↛ Infrastructure) and keep the CI pipeline failing fast on architectural violations.
- Investigate possibility of testing against a test database in CI to catch issues with migrations and seed data.
