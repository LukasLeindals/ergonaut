"""
Simple Streamlit app that emits an OTLP log record when the user clicks a button.
"""

import os
from urllib.parse import urljoin
import requests
import streamlit as st

from src.models import EmitLogRequest, EmitLogResponse, LOG_LEVELS


API_URL = "http://localhost:8000"


def main() -> None:
    st.set_page_config(page_title="Ergonaut OTLP Log Demo", page_icon="🛰️")
    st.title("Ergonaut OTLP Log Demo")
    st.write(
        "Click the button below to emit a structured log record via OpenTelemetry. "
        "Make sure the Ergonaut collector is running and forwarding to the app."
    )

    message = st.text_input("A custom log message", value="A custom log message")
    warn_level = st.selectbox("Log level", options=[None] + LOG_LEVELS, index=0)
    extra_vars = st.text_input("Extra variables (key1=value1,key2=value2)", value=None)
    if extra_vars:
        extra_vars = dict(item.strip().split("=") for item in extra_vars.split(","))
    else:
        extra_vars = {}

    if st.button("Emit log event"):
        try:
            emit_response = requests.post(
                url=urljoin(API_URL, "emit_log"),
                data=EmitLogRequest(
                    message_template=message,
                    level=warn_level,
                    extra=extra_vars,
                ).model_dump_json(),
                timeout=5,
            )
            emit_response.raise_for_status()
            emit_data = EmitLogResponse.model_validate_json(emit_response.text)
            st.success(
                f"Log event sent at {emit_data.emitted_at:%Y-%m-%d %H:%M:%S}. Check the collector output."
            )
            st.session_state["log_history"] = st.session_state.get(
                "log_history", []
            ) + [
                f"[{emit_data.emitted_level} {emit_data.emitted_at:%Y-%m-%d %H:%M:%S}] {message.format(**extra_vars)}"
            ]
        except Exception as e:  # pylint: disable=broad-except
            st.error(f"Failed to emit log event: {e}")

    if "log_history" in st.session_state:
        st.subheader("Log history")
        for log_entry in reversed(st.session_state["log_history"]):
            st.text(log_entry)


if __name__ == "__main__":
    main()
