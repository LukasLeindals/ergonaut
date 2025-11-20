# Sentinel Python Example

FastAPI + Streamlit demo that emits structured logs via OTLP to your collector.

## Prereqs
- Docker (with BuildKit)
- OpenTelemetry collector reachable at `otelcol:4318` on the `telemetry` network

## Run order (all via `just`, from repo root)
1) Start the Api and Sentinel service through docker:
   ```bash
   just run-docker
   ```
2) Start the Python API in Docker (mapped to `http://localhost:8000`):
   ```bash
   just example-sentinel-python run-api
   ```
3) (Optional) Launch the Streamlit UI against that API:
   ```bash
   just example-sentinel-python run-ui
   ```

Notes
- Ensure the `telemetry` Docker network and collector are running before step 2 (the compose file expects `otelcol:4318`).
