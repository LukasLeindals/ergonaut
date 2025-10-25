using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace Ergonaut.Core.Models.Logging;

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

    string? TraceId { get; }
    string? SpanId { get; }
    string? TraceFlags { get; }
    IReadOnlyDictionary<string, string?>? Tags { get; }

}
