#!/usr/bin/env bash
set -euo pipefail

SERVICE="${1:-}"

if [[ -z "${SERVICE}" ]]; then
  echo "Usage: remove-service-credential.sh <service>" >&2
  exit 1
fi

ROOT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")/.." && pwd)"

# List matching keys and remove them
dotnet user-secrets --project "$ROOT_DIR/src/Ergonaut.Api/Ergonaut.Api.csproj" list \
  | awk -F' = ' -v svc="$SERVICE" '/^Auth:ServiceCredentials:/ {print $1}' \
  | grep "^Auth:ServiceCredentials:${SERVICE}:" \
  | while IFS= read -r key; do
      dotnet user-secrets --project "$ROOT_DIR/src/Ergonaut.Api/Ergonaut.Api.csproj" remove "$key" >/dev/null
    done

echo "Removed service credential entries for '${SERVICE}' from Ergonaut.Api user-secrets."
