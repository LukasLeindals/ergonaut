"""Models for Sentinel Python API."""

from typing import Optional, Literal
from datetime import datetime
from pydantic import BaseModel


LOG_LEVEL = Literal["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"]
LOG_LEVELS = ["DEBUG", "INFO", "WARNING", "ERROR", "CRITICAL"]


class EmitLogRequest(BaseModel):
    """Request model for emitting a log."""

    message_template: str
    level: Optional[LOG_LEVEL] = None
    extra: dict = {}


class EmitLogResponse(BaseModel):
    """Response model for emitted log."""

    formatted_message: str
    emitted_level: LOG_LEVEL
    emitted_at: datetime
