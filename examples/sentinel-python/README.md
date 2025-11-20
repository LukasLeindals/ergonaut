# Sentinel Python Example

## Setup

To set up the environment for this example, follow these steps:

1. Install the required dependencies:

```bash
cd examples/sentinel-python
poetry install
sh otel-bootstrap.sh 
```

## Run with Docker

Build and run the FastAPI service inside a container without installing Poetry locally:

```bash
cd examples/sentinel-python
docker compose up --build
```

The API listens on `http://localhost:8000/emit_log` by default. Override the telemetry target by exporting `OTEL_EXPORTER_OTLP_ENDPOINT` before running `docker compose`. (If you run the Streamlit UI separately, it continues to read `OTLP_COLLECTOR_ENDPOINT`.)
