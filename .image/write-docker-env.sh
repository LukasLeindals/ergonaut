#!/usr/bin/env bash
set -euo pipefail

# Generate per-service env files from dotnet user-secrets

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"
FILENAME=".env"

ui_token=$(dotnet user-secrets --project "$ROOT_DIR/src/Ergonaut.UI/Ergonaut.UI.csproj" list | awk -F' = ' '/Api:Auth:ServiceToken/ {print $2}')
signing_key=$(dotnet user-secrets --project "$ROOT_DIR/src/Ergonaut.Api/Ergonaut.Api.csproj" list | awk -F' = ' '/Auth:SigningKey/ {print $2}')
api_secrets=$(dotnet user-secrets --project "$ROOT_DIR/src/Ergonaut.Api/Ergonaut.Api.csproj" list)

api_ui_token=$(printf "%s\n" "$api_secrets" | awk -F' = ' '/Auth:ServiceCredentials:Ergonaut.UI:Token/ {print $2}')
api_sentinel_token=$(printf "%s\n" "$api_secrets" | awk -F' = ' '/Auth:ServiceCredentials:Ergonaut.Sentinel:Token/ {print $2}')

# Convert all Auth:ServiceCredentials keys to env format (colon -> double underscore)
api_creds_env=$(printf "%s\n" "$api_secrets" | awk -F' = ' '
  /^Auth:ServiceCredentials:/ {
    key=$1; val=$2;
    gsub(":", "__", key);
    print key"="val;
  }')

if [[ -z "$ui_token" || -z "$signing_key" || -z "$api_ui_token" || -z "$api_sentinel_token" ]]; then
  echo "Missing secrets. Run 'just set-tokens' first." >&2
  exit 1
fi

echo "Writing $FILENAME file for API image..."
mkdir -p "$ROOT_DIR/.image/api"
cat > "$ROOT_DIR/.image/api/$FILENAME" <<EOF
Auth__SigningKey=$signing_key
EOF
printf "%s\n" "$api_creds_env" >> "$ROOT_DIR/.image/api/$FILENAME"

echo "Writing $FILENAME file for UI image..."
mkdir -p "$ROOT_DIR/.image/ui"
cat > "$ROOT_DIR/.image/ui/$FILENAME" <<EOF
DOTNET_ENVIRONMENT=Staging
Api__Auth__ServiceToken=$ui_token
EOF

echo "Writing $FILENAME file for Sentinel image..."
mkdir -p "$ROOT_DIR/.image/sentinel"
cat > "$ROOT_DIR/.image/sentinel/$FILENAME" <<EOF
DOTNET_ENVIRONMENT=Staging
Api__Auth__ServiceToken=$api_sentinel_token
EOF

echo "Wrote:"
echo "  .image/api/$FILENAME"
echo "  .image/ui/$FILENAME"
echo "  .image/sentinel/$FILENAME"
