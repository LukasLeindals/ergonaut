using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ergonaut.Core.LogIngestion;

public interface ILogEvent
{
    [Required]
    string Message { get; }

    [Required]
    string Source { get; }

    DateTimeOffset Timestamp { get; }

    string? MessageTemplate { get; }

    LogLevel Level { get; }

    IReadOnlyDictionary<string, JsonElement?>? Metadata { get; }

    IReadOnlyDictionary<string, string?>? Tags { get; }

    // From log record
    string? TraceId { get; }
    string? SpanId { get; }

    IReadOnlyDictionary<string, Object> Attributes { get; }

    IReadOnlyDictionary<string, Object> ResourceAttributes { get; }
    IReadOnlyDictionary<string, Object> ScopeAttributes { get; }
}
