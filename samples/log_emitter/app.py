"""
Simple Streamlit app that emits an OTLP log record when the user clicks a button.
"""

from datetime import datetime
import logging
import os
from typing import Tuple
import requests

from pydantic import BaseModel
import streamlit as st
from opentelemetry import _logs
from opentelemetry.exporter.otlp.proto.http._log_exporter import OTLPLogExporter
from opentelemetry.sdk._logs import LoggerProvider, LoggingHandler
from opentelemetry.sdk._logs.export import BatchLogRecordProcessor
from opentelemetry.sdk.resources import Resource


class TokenResponse(BaseModel):
    """Model for the token response from Ergonaut authentication API."""

    accessToken: str
    expiresAt: str


def _signin_to_ergonaut():
    token_response = requests.post(
        "http://localhost:5075/api/v1/auth/token",
        json={
            "username": st.secrets["ergonaut"]["username"],
            "password": st.secrets["ergonaut"]["password"],
        },
        timeout=5,
    )
    if not token_response.ok:
        raise RuntimeError(
            f"Failed to sign in to Ergonaut: {token_response.status_code} {token_response.text}"
        )
    try:
        token = TokenResponse.model_validate(token_response.json())
    except Exception as e:
        raise RuntimeError(f"Failed to parse Ergonaut token response: {e}")
    os.environ["ERGONAUT_API_TOKEN"] = token.accessToken
    print("Signed in to Ergonaut successfully.")
    return token


def _build_logger() -> logging.Logger:

    resource = Resource.create(
        {
            "service.name": "ergonaut-streamlit-demo",
            "service.namespace": "ergonaut.samples",
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
    base_handler = logging.StreamHandler()

    logger = logging.getLogger("streamlit-demo")
    logger.setLevel(logging.INFO)
    logger.addHandler(handler)
    logger.addHandler(base_handler)
    logger.propagate = False

    return logger


@st.cache_resource(show_spinner=False)
def get_logger() -> logging.Logger:
    _signin_to_ergonaut()
    return _build_logger()


def main() -> None:
    st.set_page_config(page_title="Ergonaut OTLP Log Demo", page_icon="🛰️")
    st.title("Ergonaut OTLP Log Demo")
    st.write(
        "Click the button below to emit a structured log record via OpenTelemetry. "
        "Make sure the Ergonaut collector is running and forwarding to the app."
    )

    logger = get_logger()
    st.caption(f"Logs exported to: {os.environ['OTLP_COLLECTOR_ENDPOINT']}")

    message = st.text_input("A custom log message", value="A custom log message")
    warn_level = st.selectbox(
        "Log level", options=["WARNING", "ERROR", "INFO", "DEBUG"], index=0
    )
    extra_vars = st.text_input("Extra variables (key1=value1,key2=value2)", value=None)
    if extra_vars:
        extra_vars = dict(item.strip().split("=") for item in extra_vars.split(","))
    else:
        extra_vars = {}

    if st.button("Emit log event"):
        try:
            getattr(logger, warn_level.lower())(
                message.format(**extra_vars),
                extra=extra_vars | {"messageTemplate": message},
            )
            st.success(
                f"Log event sent at {datetime.now():%Y-%m-%d %H:%M:%S}. Check the collector output."
            )
        except Exception as e:  # pylint: disable=broad-except
            st.error(f"Failed to emit log event: {e}")


if __name__ == "__main__":
    main()
