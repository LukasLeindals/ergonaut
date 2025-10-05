# Ergonaut
Ergonaut is a modular .NET solution for intelligent project and task orchestration. It keeps domain concerns isolated, exposes reusable application services, and provides both HTTP and Blazor experiences while preparing for log-driven automation.

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
- **Ergonaut.Core** – Owns the domain model (`LocalTask`, `LocalProject`, enums) and normalization helpers.
- **Ergonaut.App** – Implements use cases. Services such as `LocalTaskService` and the new `IProjectScopedTaskService` enforce project-level rules before calling repositories.
- **Ergonaut.Infrastructure** – Hosts Entity Framework Core, migrations, and concrete repository classes. A DI extension wires SQLite paths relative to the host.
- **Ergonaut.Api** – Exposes REST endpoints, handles JWT authentication/authorization, and delegates all business logic to the application layer. Includes project-scoped task endpoints at `api/v1/{projectId}/tasks`.
- **Ergonaut.UI** – Blazor Server front end that calls the API via typed HttpClient adapters, manages component state, and authenticates through an API token handler.

## Project-Scoped Task Flow
1. The UI resolves `IProjectScopedTaskService` for a selected project and calls `GET api/v1/{projectId}/tasks` to list tasks.
2. `ProjectScopedTasksController` forwards the request to the project-scoped service, which validates the project and loads data via repositories.
3. Creating a task posts to the same route; on success the API responds with `201 Created` and the new task payload.
4. Repositories persist `LocalTask` entities through `ErgonautDbContext`, keeping domain types at the center.

## Upcoming Automation ("Sentinel")
Work is underway to introduce a log-monitor worker that will:
- Ingest external log entries and evaluate rules.
- Use `IProjectScopedTaskService` to create tasks in a project-safe manner.
- Surface automation activity in the UI (see `TODO.md` for preparatory tasks).

## Local Development
- **Database**: SQLite file lives under `data/sqlite/ergonaut.db`. Migrations are managed in `Ergonaut.Infrastructure`.
- **API**: Run `dotnet run --project src/Ergonaut.Api` to expose the HTTP service on the configured port (defaults to `http://localhost:5075`).
- **UI**: Run `dotnet run --project src/Ergonaut.UI` for the Blazor Server experience. The UI automatically requests a JWT using dev credentials (`dev` / `dev`).
- **Authentication**: `AuthController` issues short-lived JWTs. Update `appsettings.json` for non-dev secrets before deploying.

## Documentation & Diagrams
Architecture diagrams reside in `docs/uml/` (PlantUML, Draw.io). Keep the diagrams in sync with code changes—especially the `components.puml` component map.

## Contributing
- Review `AGENTS.md` for collaboration principles (mentor mindset, design-first, safe-by-default).
- Use feature branches and keep commits focused on a single concern.
- Add tests in `tests/` whenever behaviour changes—log automation work should include integration coverage for project-scoped services.

---
Questions or ideas? Open an issue or add to `TODO.md` so the team can triage and plan the next iteration.
