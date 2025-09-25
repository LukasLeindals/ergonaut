#!/usr/bin/env just --justfile

# List all available just recipes
help:
    @just -l

# Launch the Ergonaut UI in watch mode
ui: update-db
    @cd src/Ergonaut.UI && dotnet watch

# Update the local SQLite database using the latest migrations
update-db:
    @echo "Applying any pending database migrations..."
    @dotnet ef database update \
    --project src/Ergonaut.Infrastructure/Ergonaut.Infrastructure.csproj \
    --startup-project src/Ergonaut.UI/Ergonaut.UI.csproj \
    | grep "Applying migration" || true
