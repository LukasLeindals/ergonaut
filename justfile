#!/usr/bin/env just --justfile

# List all available just recipes
help:
    @just -l

# Launch the Ergonaut UI in watch mode
ui:
    @dotnet watch --project src/Ergonaut.UI/Ergonaut.UI.csproj --launch-profile development

# Launch the Ergonaut API in watch mode using the HTTP profile expected by the UI
api: stop-api
    @dotnet watch --project src/Ergonaut.Api/Ergonaut.Api.csproj --launch-profile development

stop-api:
    kill -9 $(lsof -ti:5075) || echo "Ergonaut API not running"

sentinel:
    @dotnet run --project src/Ergonaut.Sentinel/Ergonaut.Sentinel.csproj

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
    dotnet test -clp:ErrorsOnly --logger:"console;verbosity=detailed"

run-log-emitter:
    cd samples/log_emitter && sh main.sh

example-sentinel-python:
    cd examples/sentinel-python && poetry run uvicorn src.api:app --reload

example-sentinel-python-ui:
    cd examples/sentinel-python && poetry run opentelemetry-instrument streamlit run src/ui.py

example-sentinel-python-docker:
    cd examples/sentinel-python && docker compose -f docker-compose.yaml up -d --remove-orphans

run-docker-development:
    @docker compose -f .image/docker-compose-development.yaml up --build -d --remove-orphans

run-docker: write-docker-env create-docker-networks
    export DOTNET_ENVIRONMENT=Staging && \
    docker compose \
    -f .image/docker-compose.yaml \
    up --build -d --remove-orphans

stop-docker:
    @docker compose -f .image/docker-compose.yaml down

build-docker project:
    docker build -f .image/{{ project }}/Dockerfile .

compose-docker target:
    if [ -z "{{ target }}" ]; then \
        echo "Usage: just compose-docker <target> (e.g. target=api)"; exit 1; \
    fi
    export DOTNET_ENVIRONMENT=Staging && \
    docker compose -f .image/docker-compose.yaml up -d --build {{ target }}

get-process port:
    @lsof -i :{{ port }} | grep LISTEN

list-process name="Ergonaut.Api":
    @ps aux | grep {{ name }}

add-service-credential service scopes="":
    @bash scripts/add-service-credential.sh "{{ service }}" "{{ scopes }}"

remove-service-credential service:
    @bash scripts/remove-service-credential.sh "{{ service }}"

show-user-secrets project:
    @echo "User-secrets for project {{ project }}:" && \
    dotnet user-secrets --project src/{{ project }}/{{ project }}.csproj list

set-tokens:
    @bash scripts/set-default-tokens.sh

# Create .image/.env.local from user-secrets for Docker compose
write-docker-env:
    @.image/write-docker-env.sh

create-docker-networks:
    @docker network create telemetry || echo "Docker network 'telemetry' already exists"
    @docker network create ergonaut || echo "Docker network 'ergonaut' already exists"
