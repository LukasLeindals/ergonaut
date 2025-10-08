# TODO by Project


## Ergonaut.UI
- Plan how the UI should surface log-generated work items (auto-refresh, badges, filters) once background automation is active.
- Update README and other docs that still reference `Ergonaut.Sentinel` to match the current solution layout or the planned worker implementation.

## Ergonaut.App / Infrastructure
- Introduce automated tests, starting with `ProjectScopedWorkItemService` and log-ingestion scenarios, to guard domain rules.

## Sentinel / Automation
- Decide how to host the upcoming log-monitor/Sentinel worker (inline background service vs dedicated project) and document the integration pattern.

## Architecture Improvements
- **Tighten dependency flow**
  - Create an `AddApplicationServices` extension so `Ergonaut.Api` only references application abstractions.
  - Move repository registration into infrastructure composition and expose only interfaces to the API/UI.
- **Organize by feature modules**
  - Restructure controllers, services, and UI components into feature folders (Projects, WorkItems, Sentinel).
  - Update namespace conventions and DI wiring to match the new layout.
- **Extract UI bootstrapping helpers**
  - Add a `services.AddErgonautApiClients()` extension encapsulating HttpClient + token handler setup.
  - Simplify `Ergonaut.UI/Program.cs` to call composable setup methods only.
- **Define automation interfaces early**
  - Introduce `ILogEventSource` / `ILogTaskOrchestrator` contracts in the application layer.
  - Sketch stub implementations so automation tests can start before the worker exists.
- **Strengthen validation pipeline**
  - Add application-level validators (FluentValidation or manual) for work item/project commands.
  - Extend `WorkItem` with correlation identifiers or guards to prevent duplicate log-derived work items.
- **Enforce architectural rules with tests**
  - Add NetArchTest (or similar) checks to keep UI/API from referencing infrastructure directly.
  - Wire the arch tests into CI so violations fail builds immediately.
