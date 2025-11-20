"""Example FastAPI application for Sentinel Python API."""

from datetime import datetime
import random
from fastapi import FastAPI

from src.models import EmitLogRequest, EmitLogResponse, LOG_LEVELS
from src.logger import build_logger, add_otel_handler

app = FastAPI()

app.state.logger = build_logger()
app.state.logger = add_otel_handler(app.state.logger)


@app.post("/emit_log", response_model=EmitLogResponse)
def emit_log(request: EmitLogRequest):
    """Emit a log record with the specified message and level."""
    message = request.message_template.format(**request.extra)
    level = request.level or random.choice(LOG_LEVELS)

    log_method = getattr(app.state.logger, level.lower())
    log_method(
        message, extra=request.extra | {"message_template": request.message_template}
    )

    return EmitLogResponse(
        formatted_message=message,
        emitted_level=level,
        emitted_at=datetime.now(),
    )
