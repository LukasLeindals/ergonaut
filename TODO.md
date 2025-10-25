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


## Improvements
- Introduce validators for `CreateProjectRequest` / `CreateWorkItemRequest`, extend `WorkItem` with correlation identifiers, and add unit tests covering duplicate-prevention.
- Expand NetArchTest coverage (UI ↛ Infrastructure, Infrastructure ↛ UI, Contracts ↛ Infrastructure) and keep the CI pipeline failing fast on architectural violations.
- Investigate possibility of testing against a test database in CI to catch issues with migrations and seed data.
