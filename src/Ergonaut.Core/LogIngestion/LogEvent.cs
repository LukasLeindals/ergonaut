using System.Text.Json;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;

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
        IReadOnlyDictionary<string, string?>? attributes = null,
        IReadOnlyDictionary<string, string?>? resourceAttributes = null,
        IReadOnlyDictionary<string, string?>? scopeAttributes = null
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

        Attributes = attributes ?? new Dictionary<string, string?>();
        ResourceAttributes = resourceAttributes ?? new Dictionary<string, string?>();
        ScopeAttributes = scopeAttributes ?? new Dictionary<string, string?>();
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
    public IReadOnlyDictionary<string, string?> Attributes { get; init; }
    public string? TraceId { get; }
    public string? SpanId { get; }

    // From resource
    public IReadOnlyDictionary<string, string?> ResourceAttributes { get; init; }

    // From scope
    public IReadOnlyDictionary<string, string?> ScopeAttributes { get; init; }

    public string? GetFingerprint() => GetFingerprint(true);

    public string? GetFingerprint(bool includeTimestamp)
    {
        string fingerPrint = $"{Source}__{Message}__{TraceId}__{SpanId}";

        if (includeTimestamp)
            fingerPrint += $"__{Timestamp}";

        using var sha256 = SHA256.Create();
        return Convert.ToHexString(sha256.ComputeHash(Encoding.UTF8.GetBytes(fingerPrint)));
    }
}
