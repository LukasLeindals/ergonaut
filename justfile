#!/usr/bin/env just --justfile

# List all available just recipes
help:
    @just -l

# Launch the Ergonaut UI in watch mode
ui:
    @cd src/Ergonaut.UI && dotnet watch

# Launch the Ergonaut API in watch mode using the HTTP profile expected by the UI
api: stop-api
    @dotnet watch --project src/Ergonaut.Api/Ergonaut.Api.csproj --launch-profile http

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
    cd samples/log_emitter && poetry run streamlit run app.py

start-otelcol:
    @echo "Starting OpenTelemetry Collector..."
    @if pid=$(pgrep -f "otelcol --config config/collector.yaml"); then \
        echo "otelcol already running with pid ${pid}"; \
    else \
        otelcol --config config/collector.yaml; \
    fi

stop-otelcol:
    @pkill -TERM -f "otelcol --config config/collector.yaml" || echo "otelcol not running"

install-otelcol:
    curl -LO https://github.com/open-telemetry/opentelemetry-collector-releases/releases/download/v0.101.0/otelcol_0.101.0_darwin_arm64.tar.gz
    tar -xzf otelcol_0.101.0_darwin_arm64.tar.gz
    sudo mv otelcol /usr/local/bin/   # or another directory on your PATH

run-docker-development:
    @docker compose -f .image/docker-compose-development.yaml up --build -d --remove-orphans

run-docker:
    export DOTNET_ENVIRONMENT=Staging && \
    docker compose \
    -f .image/docker-compose.yaml \
    up --build -d --remove-orphans

stop-docker:
    @docker compose -f .image/docker-compose.yaml down

build-docker project:
    docker build -f .image/{{ project }}/Dockerfile .

compose-docker target:
    export DOTNET_ENVIRONMENT=Staging && \
    docker compose -f .image/docker-compose.yaml up -d --build {{ target }}

get-process port:
    @lsof -i :{{ port }} | grep LISTEN

list-process name="Ergonaut.Api":
    @ps aux | grep {{ name }}
