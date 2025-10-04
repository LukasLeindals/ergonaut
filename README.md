# Ergonaut
Ergonaut: A modular C# framework for intelligent task generation, planning, and management.

## Project Structure
```
Ergonaut/
│── src/
│   ├── Ergonaut.Core/            # Core domain models & abstractions
│   │   ├── Models/               # Task, Project, LogEvent entities
│   │   ├── Interfaces/           # Repository & service contracts
│   │   └── Utils/                # Shared helpers & extensions
│   │
│   ├── Ergonaut.Sentinel/        # Log-driven task generator (MVP focus)
│   │   ├── Api/                  # ASP.NET Core Web API controllers
│   │   ├── Services/             # Business logic & rules engine
│   │   ├── Persistence/          # EF Core DbContext, SQLite provider
│   │   └── Mapping/              # DTOs & entity mappers
│   │
│   └── Ergonaut.UI/              # Blazor WebAssembly frontend
│       ├── Pages/                # Razor pages
│       ├── Components/           # Reusable UI components
│       └── Services/             # API clients & state management
│
│── tests/                        # Automated tests
│   ├── Ergonaut.Core.Tests/
│   ├── Ergonaut.Sentinel.Tests/
│   └── Ergonaut.UI.Tests/
│
│── docs/                         # Project documentation
│── build/                        # CI/CD, build, and packaging scripts
│── tools/                        # Developer tools, migrations, seeds
│── .editorconfig                 # Code style configuration
│── README.md                     # Project overview

```
