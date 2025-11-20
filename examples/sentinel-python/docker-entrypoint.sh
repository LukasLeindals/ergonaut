#!/usr/bin/env bash
set -euo pipefail

API_PORT=${UVICORN_PORT:-8000}
API_HOST=${UVICORN_HOST:-0.0.0.0}

echo "Starting FastAPI backend on ${API_HOST}:${API_PORT}..."
exec opentelemetry-instrument \
    uvicorn src.api:app \
    --host "$API_HOST" \
    --port "$API_PORT"
