#!/usr/bin/env just --justfile

# List all available just recipes
help:
    @just -l

# Launch the Ergonaut UI in watch mode
ui:
    @cd src/Ergonaut.UI && dotnet watch

# Launch the Ergonaut API in watch mode using the HTTP profile expected by the UI
api:
    @dotnet watch --project src/Ergonaut.Api/Ergonaut.Api.csproj --launch-profile http

# Update the local SQLite database using the latest migrations
update-db:
    @echo "Applying any pending database migrations..."
    @dotnet ef database update \
    --project src/Ergonaut.Infrastructure/Ergonaut.Infrastructure.csproj \
    --startup-project src/Ergonaut.Api/Ergonaut.Api.csproj

# Create a new EF Core migration after changing the model (entities, DbContext configuration, etc.). Requires a name (similar to git commit message).
add-migration name:
    @if [ -z "{{ name }}" ]; then \
        echo "Usage: just add-migration <name>"; exit 1; \
    fi
    @dotnet ef migrations add "{{ name }}" \
    --project src/Ergonaut.Infrastructure/Ergonaut.Infrastructure.csproj \
    --startup-project src/Ergonaut.Api/Ergonaut.Api.csproj

test:
    dotnet test --no-build -clp:ErrorsOnly --logger:"console;verbosity=detailed"
