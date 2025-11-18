#!/usr/bin/env bash

set -euo pipefail

set_api_tokens () {
    SIGNING_KEY=$(openssl rand -base64 32)
    LOG_KEY=$(openssl rand -base64 32)
    echo "Setting Auth:SigningKey for Ergonaut.Api via user-secrets..."
    dotnet user-secrets --project src/Ergonaut.Api/Ergonaut.Api.csproj set "Auth:SigningKey" "$SIGNING_KEY" >/dev/null
    echo "Setting LogIngestion:ApiKey for Ergonaut.Api via user-secrets..."
    dotnet user-secrets --project src/Ergonaut.Api/Ergonaut.Api.csproj set "LogIngestion:ApiKey" "$LOG_KEY" >/dev/null
}

set_service_token () {
    service="${1:-}"
    if [[ -z "$service" ]]; then
        echo "Usage: set_service_token <service>" >&2
        exit 1
    fi
    project="src/${service}/${service}.csproj"
    if [[ ! -f "$project" ]]; then
        echo "Project file not found for service '${service}' at ${project}" >&2
        exit 1
    fi
    TOKEN=$(openssl rand -base64 32)
    echo "Setting service token for '${service}' via user-secrets..."
    dotnet user-secrets --project "$project" set "Api:Auth:ServiceToken" "$TOKEN" >/dev/null
    dotnet user-secrets --project src/Ergonaut.Api/Ergonaut.Api.csproj set "Auth:ServiceCredentials:${service}:Token" "$TOKEN" >/dev/null
}

set_api_tokens
set_service_token "Ergonaut.UI"
set_service_token "Ergonaut.Sentinel"
echo "Done."
