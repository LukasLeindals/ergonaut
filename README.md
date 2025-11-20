# Ergonaut
Modular .NET stack for tracking projects and tasks with a clean domain core, REST API, and Blazor Server UI.

## What is Ergonaut?
Ergonaut keeps domain rules in `Ergonaut.Core`, funnels all use cases through `Ergonaut.App`, and exposes them via `Ergonaut.Api` (JWT) and `Ergonaut.UI` (Blazor). Data defaults to SQLite in `data/sqlite/ergonaut.db`.

## What is Sentinel?
Sentinel is an automation worker that watches your logs/telemetry and creates tasks when rules match. It calls the same application services as the UI and API, so automation stays project-safe and goes through the domain model.


## Add Sentinel to Your Project
1) Start the Dockerized application services (or host them yourself):
```bash
just run-docker
```
    - This will:
        - Start the database in a Docker volume.
        - Create Auth tokens and create the relevant `.env` files.
        - Run the API on `http://localhost:5075`.
        - Run the UI on `http://localhost:5242`.
        - Run an OTLP collector.
        - Run a Sentinel worker.
1) Point your log/telemetry pipeline to an OTLP collector the Sentinel worker can reach.
    - It is a good idea to ensure `messageTemplate` is included in log records as an attribute as this is used to deduplicate similar logs.
2) View automation results in the UI or via the API.


See `examples/sentinel-python` for a sample Sentinel worker in Python, where the `logger.py` file shows how to add a logging handler that emits OTLP logs.

## Repo Overview
- `src/Ergonaut.Core` – domain entities and value objects.
- `src/Ergonaut.App` – application services/DTOs.
- `src/Ergonaut.Infrastructure` – EF Core DbContext + repositories.
- `src/Ergonaut.Api` – REST layer with JWT auth.
- `src/Ergonaut.UI` – Blazor Server front end.
- `docs/uml` – component and sequence diagrams.
