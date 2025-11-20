import logging
import os

from opentelemetry import _logs
from opentelemetry.exporter.otlp.proto.http._log_exporter import OTLPLogExporter
from opentelemetry.sdk._logs import LoggerProvider, LoggingHandler
from opentelemetry.sdk._logs.export import BatchLogRecordProcessor
from opentelemetry.sdk.resources import Resource


def build_logger() -> logging.Logger:
    """Builds a basic logger."""
    base_handler = logging.StreamHandler()

    logger = logging.getLogger("sentinel-python-example")
    logger.setLevel(logging.INFO)
    logger.addHandler(base_handler)
    logger.propagate = False

    return logger


def add_otel_handler(logger: logging.Logger) -> logging.Logger:
    """Adds an OpenTelemetry handler to the logger."""
    resource = Resource.create(
        {
            "service.name": "sentinel-python-example",
            "service.namespace": "ergonaut.examples",
            "service.instance.id": os.getenv("HOSTNAME", "local"),
        }
    )

    # Only set the provider once; reuse if already initialized
    provider = _logs.get_logger_provider()
    if not isinstance(provider, LoggerProvider):
        provider = LoggerProvider(resource=resource)
        _logs.set_logger_provider(provider)

    exporter = OTLPLogExporter()
    provider.add_log_record_processor(BatchLogRecordProcessor(exporter))

    handler = LoggingHandler(level=logging.INFO, logger_provider=provider)

    logger.addHandler(handler)

    return logger
