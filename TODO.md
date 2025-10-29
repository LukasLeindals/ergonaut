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
- Clean up authentication and authorization.

## Sentinel MVP

- Settle on design for creating work items from log events.
- Implement creation of work items from log events.


## Sentinel Part II
- Deduplicate work item creation for identical log events occurring within a configurable time window.
- Configuration options for Sentinel (e.g., time window for deduplication, log event severity threshold for work item creation).

## Sentinel Future
- Datadog integration.
- LLM layer for better work item description generation and deduplication.

## Improvements
- Introduce validators for `CreateProjectRequest` / `CreateWorkItemRequest`, extend `WorkItem` with correlation identifiers, and add unit tests covering duplicate-prevention.
- Expand NetArchTest coverage (UI ↛ Infrastructure, Infrastructure ↛ UI, Contracts ↛ Infrastructure) and keep the CI pipeline failing fast on architectural violations.
- Investigate possibility of testing against a test database in CI to catch issues with migrations and seed data.
