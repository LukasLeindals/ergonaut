"""
Simple Streamlit app that emits an OTLP log record when the user clicks a button.
"""

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

DEFAULT_ENDPOINT = "http://localhost:4318/v1/logs"


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


def _build_logger() -> Tuple[logging.Logger, str]:
    endpoint = os.getenv("OTLP_COLLECTOR_ENDPOINT", DEFAULT_ENDPOINT)

    resource = Resource.create(
        {
            "service.name": "ergonaut-streamlit-demo",
            "service.namespace": "ergonaut.samples",
            "service.instance.id": os.getenv("HOSTNAME", "local"),
        }
    )

    provider = LoggerProvider(resource=resource)
    _logs.set_logger_provider(provider)

    exporter = OTLPLogExporter(
        endpoint=endpoint,
        headers=(("Authorization", f"Bearer {os.environ['ERGONAUT_API_TOKEN']}"),),
    )
    provider.add_log_record_processor(BatchLogRecordProcessor(exporter))

    handler = LoggingHandler(level=logging.INFO, logger_provider=provider)
    base_handler = logging.StreamHandler()

    logger = logging.getLogger("streamlit-demo")
    logger.setLevel(logging.INFO)
    logger.addHandler(handler)
    logger.addHandler(base_handler)
    logger.propagate = False

    return logger, endpoint


@st.cache_resource(show_spinner=False)
def get_logger() -> Tuple[logging.Logger, str]:
    _signin_to_ergonaut()
    return _build_logger()


def main() -> None:
    st.set_page_config(page_title="Ergonaut OTLP Log Demo", page_icon="🛰️")
    st.title("Ergonaut OTLP Log Demo")
    st.write(
        "Click the button below to emit a structured log record via OpenTelemetry. "
        "Make sure the Ergonaut collector is running and forwarding to the app."
    )

    logger, endpoint = get_logger()
    st.caption(f"Logs exported to: {endpoint}")

    if st.button("Emit log event"):
        logger.warning("User triggered log emission from Streamlit demo.")
        st.success("Log event sent! Check the collector output.")


if __name__ == "__main__":
    main()
