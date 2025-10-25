using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ergonaut.Core.Models.Logging;

/// <summary>
/// Immutable log event record used across ingestion pipelines.
/// </summary>
public sealed record LogEvent : ILogEvent
{
    public LogEvent(
        string message,
        string source,
        DateTimeOffset timestamp,
        LogLevel level,
        string? messageTemplate = null,
        IReadOnlyDictionary<string, JsonElement?>? metadata = null,
        string? traceId = null,
        string? spanId = null,
        string? traceFlags = null,
        IReadOnlyDictionary<string, string?>? tags = null
        )
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            throw new ArgumentException("Message must be provided.", nameof(message));
        }

        if (string.IsNullOrWhiteSpace(source))
        {
            throw new ArgumentException("Source must be provided.", nameof(source));
        }

        Message = message;
        Source = source;
        Timestamp = timestamp;
        Level = level;
        MessageTemplate = messageTemplate;
        Metadata = metadata;
        TraceId = traceId;
        SpanId = spanId;
        TraceFlags = traceFlags;
        Tags = tags;
    }

    public string Message { get; }

    public string Source { get; }

    public DateTimeOffset Timestamp { get; }

    public string? MessageTemplate { get; }

    public LogLevel Level { get; }

    public IReadOnlyDictionary<string, JsonElement?>? Metadata { get; }

    public string? TraceId { get; }
    public string? SpanId { get; }
    public string? TraceFlags { get; }
    public IReadOnlyDictionary<string, string?>? Tags { get; }
}
