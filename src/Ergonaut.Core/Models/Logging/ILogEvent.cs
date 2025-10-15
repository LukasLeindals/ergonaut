using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

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
}
