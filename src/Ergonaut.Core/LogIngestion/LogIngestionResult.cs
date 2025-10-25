using System;
using System.Collections.Generic;

namespace Ergonaut.Core.LogIngestion;

/// <summary>
/// Summarizes the outcome of a log ingestion attempt.
/// </summary>
public sealed record LogIngestionResult
{
    private LogIngestionResult(bool isSuccess, IReadOnlyList<ILogEvent> events, int droppedEventCount, IReadOnlyList<string> warnings)
    {
        IsSuccess = isSuccess;
        Events = events;
        DroppedEventCount = droppedEventCount;
        Warnings = warnings;
    }

    public bool IsSuccess { get; }

    /// <summary>
    /// Events produced during ingestion. Empty for failures.
    /// </summary>
    public IReadOnlyList<ILogEvent> Events { get; }

    public int DroppedEventCount { get; }

    public IReadOnlyList<string> Warnings { get; }

    public static LogIngestionResult Success(IReadOnlyList<ILogEvent> events, int droppedEventCount = 0, IReadOnlyList<string>? warnings = null)
    {
        var producedEvents = events ?? Array.Empty<ILogEvent>();
        var warningList = warnings ?? Array.Empty<string>();
        return new(true, producedEvents, droppedEventCount, warningList);
    }

    public static LogIngestionResult Failure(IReadOnlyList<string>? warnings)
    {
        var warningList = warnings ?? Array.Empty<string>();
        return new(false, Array.Empty<ILogEvent>(), 0, warningList);
    }

    public static LogIngestionResult Failure(params string[] warnings) =>
        Failure(warnings.Length == 0 ? Array.Empty<string>() : warnings);
}
