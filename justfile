#!/usr/bin/env just --justfile

# List all available just recipes
help:
    @just -l

# Launch the Ergonaut UI in watch mode
ui:
    @cd src/Ergonaut.UI && dotnet watch

# Launch the Ergonaut API in watch mode using the HTTP profile expected by the UI
api: update-db
    @dotnet watch --project src/Ergonaut.Api/Ergonaut.Api.csproj --launch-profile http

# Update the local SQLite database using the latest migrations
update-db:
    @echo "Applying any pending database migrations..."
    @dotnet ef database update \
    --project src/Ergonaut.Infrastructure/Ergonaut.Infrastructure.csproj \
    --startup-project src/Ergonaut.Api/Ergonaut.Api.csproj | grep "Done"
