export OTLP_COLLECTOR_ENDPOINT="http://localhost:4318"

export OTEL_SERVICE_NAME="ergonaut-streamlit-demo"

export OTEL_EXPORTER_OTLP_PROTOCOL="http/protobuf"
export OTEL_EXPORTER_OTLP_HEADERS="Authorization=Bearer ${ERGONAUT_API_TOKEN}"

export OTEL_TRACES_EXPORTER="otlp"
export OTEL_LOGS_EXPORTER="otlp"

export OTEL_PYTHON_LOG_CORRELATION="true"

poetry run opentelemetry-instrument streamlit run app.py
