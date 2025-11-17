#!/usr/bin/env bash
set -euo pipefail

echo "🔍 Running OpenTelemetry Bootstrap to detect required instrumentations..."
echo

# Run bootstrap through poetry to ensure correct environment is used
REQUIREMENTS=$(poetry run opentelemetry-bootstrap -a requirements || true)

if [[ -z "$REQUIREMENTS" ]]; then
    echo "No OpenTelemetry instrumentation recommendations found."
    exit 0
fi

echo "📋 Detected the following required packages:"
echo "$REQUIREMENTS" | sed 's/^/  /'

echo
echo "📦 Extracting package names..."

# Filter out comments & markers
PACKAGES=$(echo "$REQUIREMENTS" \
    | grep -v '^#' \
    | sed 's/;.*$//' \
    | awk '{print $1}' \
    | tr '\n' ' ')

if [[ -z "$PACKAGES" ]]; then
    echo "No valid packages detected."
    exit 0
fi

echo "Packages to install:"
echo "  $PACKAGES"
echo

echo "🚀 Installing with Poetry..."
poetry add $PACKAGES

echo
echo "✅ Done! Your pyproject.toml and poetry.lock are now updated."
