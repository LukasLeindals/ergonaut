# Architecture Improvement Plan

This plan outlines the refactors needed to tighten Ergonaut’s modular boundaries ahead of the Sentinel automation work.

## 1. Dependency Flow
- Extract an `AddApplicationServices` extension inside `Ergonaut.App` that registers services/factories without referencing infrastructure.
- Update `Ergonaut.Api/Program.cs` to call the new extension and remove direct references to concrete repositories.
- Keep `AddInfrastructure` responsible for EF Core wiring; ensure only `Ergonaut.Api` and future hosts opt into the infrastructure layer.
- Add NetArchTest rules to enforce `Ergonaut.Api` → `Ergonaut.App` → `Ergonaut.Core` hierarchy (no direct `Api` → `Infrastructure` references).

## 2. DTO Strategy
- Decide whether UI can consume `Ergonaut.App` DTOs directly or if a dedicated “Contracts” assembly is warranted.
- If sharing `App` DTOs, expose them via a new `Ergonaut.App.Contracts` namespace to clarify their reuse intent.
- Remove duplicate UI models (`ProjectInfo`, etc.) and update mapping code to use the shared DTOs.
- Document the DTO policy in README or `/docs` so new features follow the same pattern.

## 3. Feature-Folder Organization
- Introduce feature-based folders inside each layer (`Features/Projects`, `Features/WorkItems`, `Features/Sentinel`).
- Align namespaces, e.g., `Ergonaut.Api.Features.WorkItems.ProjectWorkItemsController`.
- Update DI registrations to reflect the new namespace paths.
- Keep automated tests organized in matching feature folders to simplify discoverability.

## 4. Shared DI Helpers
- Create `Ergonaut.Api/DependencyInjection/ServiceCollectionExtensions` with methods like `AddErgonautAuthentication`, `AddErgonautSwagger`.
- In `Ergonaut.UI`, add `AddErgonautApiClients` to encapsulate HttpClient + token handler registration.
- Ensure both API and UI hosts become thin composition roots that orchestrate helper extensions only.

## 5. Automation Interfaces
- Define interfaces such as `ILogEventSource`, `ILogWorkItemOrchestrator`, and `ILogIngestionPipeline` in `Ergonaut.App`.
- Provide in-memory or stub implementations for tests before Sentinel is built.
- Update TODOs/docs to reference the new abstractions as the recommended integration points.

## 6. Validation Pipeline
- Add command validators (FluentValidation or custom) for `CreateProjectRequest` and `CreateWorkItemRequest`.
- Extend `LocalWorkItem` to include optional correlation identifiers so log-driven work items can be de-duplicated.
- Write unit tests ensuring duplicate detection prevents unintended work item creation.

## 7. Architectural Tests
- Add a `Ergonaut.ArchTests` project using NetArchTest.
- Write tests enforcing:
  - UI must not reference infrastructure.
  - Infrastructure must not reference UI.
  - DTO assemblies remain dependency-free from infrastructure concerns.
- Integrate the arch test project into CI so violations break the build.

## Execution Ordering
1. Dependency flow and DI helpers (simplify composition roots first).
2. DTO strategy (reduce duplication before restructuring features).
3. Feature-folder reorganization.
4. Automation interfaces + validation pipeline.
5. Architectural tests to lock in the new boundaries.

Track progress by updating `TODO.md` as each milestone completes.
