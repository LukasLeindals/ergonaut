
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ergonaut.Core.LogIngestion;

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
        IReadOnlyDictionary<string, string?>? tags = null,
        IReadOnlyDictionary<string, object>? attributes = null,
        IReadOnlyDictionary<string, object>? resourceAttributes = null,
        IReadOnlyDictionary<string, object>? scopeAttributes = null
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
        Tags = tags;

        Attributes = attributes ?? new Dictionary<string, Object>();
        ResourceAttributes = resourceAttributes ?? new Dictionary<string, Object>();
        ScopeAttributes = scopeAttributes ?? new Dictionary<string, Object>();
        TraceId = traceId;
        SpanId = spanId;

    }

    public string Message { get; }

    public string Source { get; }

    public DateTimeOffset Timestamp { get; }

    public string? MessageTemplate { get; }

    public LogLevel Level { get; }

    public IReadOnlyDictionary<string, JsonElement?>? Metadata { get; }

    public IReadOnlyDictionary<string, string?>? Tags { get; }

    // From log record
    public IReadOnlyDictionary<string, Object> Attributes { get; set; } = new Dictionary<string, Object>();
    public string? TraceId { get; }
    public string? SpanId { get; }

    // From resource
    public IReadOnlyDictionary<string, Object> ResourceAttributes { get; set; } = new Dictionary<string, Object>();

    // From scope
    public IReadOnlyDictionary<string, Object> ScopeAttributes { get; set; } = new Dictionary<string, Object>();
}
