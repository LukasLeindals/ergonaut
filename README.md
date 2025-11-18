# Ergonaut
Ergonaut is a modular .NET solution for intelligent project and task (work item) orchestration. It keeps domain concerns isolated, exposes reusable application services, and provides both HTTP and Blazor experiences while preparing for log-driven automation.

## Usage

The ergonaut project is designed to be hosted locally or deployed to a server.

## Solution Layout
```
Ergonaut/
├── src/
│   ├── Ergonaut.Core/           # Domain entities, value objects, and enums
│   ├── Ergonaut.App/            # Application services, DTOs, factories
│   ├── Ergonaut.Infrastructure/ # EF Core DbContext + repository implementations
│   ├── Ergonaut.Api/            # ASP.NET Core API (JWT auth, REST endpoints)
│   └── Ergonaut.UI/             # Blazor Server UI + API client adapters
├── data/                        # Local persistence (SQLite)
├── docs/                        # Architecture notes, UML diagrams
└── TODO.md                      # Current engineering backlog
```

## Layer Responsibilities
- **Ergonaut.Core** – Owns the domain model (`WorkItem`, `Project`, enums) and normalization helpers.
- **Ergonaut.App** – Implements use cases. Services such as `WorkItemService` and the new `IWorkItemService` enforce project-level rules before calling repositories.
- **Ergonaut.Infrastructure** – Hosts Entity Framework Core, migrations, and concrete repository classes. A DI extension wires SQLite paths relative to the host.
- **Ergonaut.Api** – Exposes REST endpoints, handles JWT authentication/authorization, and delegates all business logic to the application layer. Includes project-scoped work item endpoints at `api/v1/{projectId}/work-items`.
- **Ergonaut.UI** – Blazor Server front end that calls the API via typed HttpClient adapters, manages component state, and authenticates through an API token handler.

## Shared DTO Policy
- Application DTOs live in `Ergonaut.App.Models` and act as the reusable contract for both API responses and UI consumption.
- Services return these models directly; downstream callers should not wrap or map them unless projecting into view-specific shapes.
- There is no separate contracts assembly today—new features should continue extending the existing `Models` namespace to keep serialization types centralized.

## Project Work Item Flow
1. The UI resolves `IWorkItemService` for a selected project and calls `GET api/v1/{projectId}/work-items` to list work items.
2. `Controllers/ProjectScoped/WorkItemsController` forwards the request to the project-safe service, which validates the project and loads data via repositories.
3. Creating a work item posts to the same route; on success the API responds with `201 Created` and the new work item payload.
4. Repositories persist `WorkItem` entities through `ErgonautDbContext`, keeping domain types at the center.

## Upcoming Automation ("Sentinel")
Work is underway to introduce a log-monitor worker that will:
- Ingest external log entries and evaluate rules.
- Use `IWorkItemService` to create work items in a project-safe manner.
- Surface automation activity in the UI (see `TODO.md` for preparatory work).

## Local Development
- **Database**: SQLite file lives under `data/sqlite/ergonaut.db`. Migrations are managed in `Ergonaut.Infrastructure`. Override the data root for other hosts (e.g., Sentinel worker) via the `Ergonaut:DataRoot` configuration key or `ERGONAUT_DATA_ROOT` environment variable.
- **API**: Run `dotnet run --project src/Ergonaut.Api` to expose the HTTP service on the configured port (defaults to `http://localhost:5075`).
- **UI**: Run `dotnet run --project src/Ergonaut.UI` for the Blazor Server experience. The UI automatically requests a JWT using dev credentials (`dev` / `dev`).
- **Authentication**: `AuthController` issues short-lived JWTs. Update `appsettings.json` for non-dev secrets before deploying.

## Documentation & Diagrams
Architecture diagrams reside in `docs/uml/` (PlantUML, Draw.io). Keep the diagrams in sync with code changes—especially the `components.puml` component map.
