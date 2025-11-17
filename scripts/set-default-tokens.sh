set_api_tokens () {
    KEY=$(openssl rand -base64 32)
    echo "Setting Auth:SigningKey for Ergonaut.Api via user-secrets..."
    dotnet user-secrets --project src/Ergonaut.Api/Ergonaut.Api.csproj set "Auth:SigningKey" "$KEY" >/dev/null
}

set_service_token () {
    service="${1:-}"
    if [[ -z "$service" ]]; then
        echo "Usage: set_service_token <service>" >&2
        exit 1
    fi
    TOKEN=$(openssl rand -base64 32)
    echo "Setting service token for '${service}' via user-secrets..."
    dotnet user-secrets --project src/Ergonaut.UI/Ergonaut.UI.csproj set "Api:Auth:ServiceToken" "$TOKEN" >/dev/null
}

set_api_tokens
set_service_token "Ergonaut.UI"
set_service_token "Ergonaut.Sentinel"
echo "Done."
