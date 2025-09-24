#!/usr/bin/env just --justfile

# List all available just recipes
help:
    @just -l

# Launch the Ergonaut UI in watch mode
ui:
    @cd src/Ergonaut.UI && dotnet watch

# Update the local SQLite database using the latest migrations
update-db:
    dotnet ef database update --project src/Ergonaut.Infrastructure --startup-project data
