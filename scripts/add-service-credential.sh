#!/usr/bin/env bash
set -euo pipefail

SERVICE="${1:-}"
SCOPES="${2:-}"

if [[ -z "${SERVICE}" ]]; then
  echo "Usage: add-service-credential.sh <service> [scopes_csv]" >&2
  exit 1
fi

# Enforce env-safe service names (alnum + underscore only)
if ! [[ "${SERVICE}" =~ ^[A-Za-z0-9_]+$ ]]; then
  echo "Service name '${SERVICE}' is invalid. Use only letters, numbers, and underscores (no hyphens)." >&2
  exit 1
fi

TOKEN="${TOKEN:-$(openssl rand -base64 32)}"

echo "Adding service credential for '${SERVICE}' into Ergonaut.Api user-secrets..."
dotnet user-secrets --project src/Ergonaut.Api/Ergonaut.Api.csproj set \
  "Auth:ServiceCredentials:${SERVICE}:Token" "${TOKEN}" >/dev/null

if [[ -n "${SCOPES}" ]]; then
  IFS=',' read -ra PARTS <<< "${SCOPES}"
  i=0
  for scope in "${PARTS[@]}"; do
    scope_trimmed="$(echo "${scope}" | tr -d '[:space:]')"
    if [[ -n "${scope_trimmed}" ]]; then
      dotnet user-secrets --project src/Ergonaut.Api/Ergonaut.Api.csproj set \
        "Auth:ServiceCredentials:${SERVICE}:Scopes:${i}" "${scope_trimmed}" >/dev/null
      ((i++))
    fi
  done
fi

echo "Done. Token (copy to the external client):"
echo "${TOKEN}"
