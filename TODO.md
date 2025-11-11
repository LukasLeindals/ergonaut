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

## Authentication & Authorization
- Clean up authentication and authorization.
  - Long-lived tokens.
  - No hardcoded secrets.

## Sentinel MVP

- Update work items to contain more information from log events for better deduplication and context.
- Add simple deduplication based on log template.
  - Posibly also add a configurable otlp attribute to use for filtering.
- Deduplicate work item creation for identical log events occurring within a configurable time window.
- Configuration options for Sentinel (e.g., time window for deduplication, log event severity threshold for work item creation).

## Sentinel Future
- Datadog integration.
- LLM layer for better work item description generation and deduplication.

