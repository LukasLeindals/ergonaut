#!/usr/bin/env bash
set -euo pipefail

OS="$(uname | tr '[:upper:]' '[:lower:]')"
if [[ "$OS" != "darwin" ]]; then
    echo "This script is intended to run on macOS (darwin) only." >&2
    exit 1
fi

ACTION="${1:-}"

if [[ "$ACTION" == "install" ]]; then
    echo "Installing OpenTelemetry Collector..."
    curl -LO https://github.com/open-telemetry/opentelemetry-collector-releases/releases/download/v0.101.0/otelcol_0.101.0_darwin_arm64.tar.gz
    tar -xzf otelcol_0.101.0_darwin_arm64.tar.gz
    sudo mv otelcol /usr/local/bin/   # or another directory on your PATH
    exit 0
fi

if [[ "$ACTION" == "run" ]]; then
    echo "Running OpenTelemetry Collector..."
    if pid=$(pgrep -f "otelcol --config config/collector.yaml"); then
        echo "Otelcol already running with pid ${pid}"
    else
        otelcol --config config/collector.yaml
    fi
    exit 0
fi

if [[ "$ACTION" == "stop" ]]; then
    echo "Stopping OpenTelemetry Collector..."
    pkill -TERM -f "otelcol --config config/collector.yaml" || echo "Otelcol not running"
    exit 0
fi

echo "Usage: otelcol.sh <install|run|stop>" >&2
exit 1
