# TODO

TODOs for the project.

Milestone TODOs:
- Should be given a subheading for larger groups of tasks that can be performed independently.
  - Avoid creating groups with only one task unless it is a major task.
  - Groups should be independent of each other, i.e. the tasks can/should be done in any order.
- Within a milestone heading, tasks should be ordered by priority and dependency.


## Structure & Architecture
- Refactor LogIngestion to separate adapters, processors, and exporters more clearly.
- Re-evaluate placement of certain services (e.g., WorkItemCreator) in the project structure.
- Investigate oppportunity for using MVVM or similar patterns in the app structure.

## Authentication & Authorization
- Clean up authentication and authorization.
  - Long-lived tokens.
  - No hardcoded secrets.

## Sentinel MVP

- Add application stack traces.
- Configuration options for Sentinel (e.g., time window for deduplication, log event severity threshold for work item creation).

## Checks
- Is async where possible and done correctly.

## Sentinel Future
- Datadog integration.
- LLM layer for better work item description generation and deduplication.

