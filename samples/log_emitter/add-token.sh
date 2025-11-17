#!/usr/bin/env bash
set -euo pipefail

SERVICE="${1:-log_emitter}"
SECRETS_FILE="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)/.streamlit/secrets.toml"

TOKEN=$(just add-service-credential "$SERVICE" 2>/dev/null | tail -n1)

mkdir -p "$(dirname "$SECRETS_FILE")"
cat > "$SECRETS_FILE" <<EOF
[ergonaut]
service="$SERVICE"
token="$TOKEN"
EOF

echo "Updated $SECRETS_FILE with service '$SERVICE'."
echo "$TOKEN"
