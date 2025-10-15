namespace Ergonaut.Core.Models.Logging;

public sealed record LogStreamOptions
{
    public LogLevel MinimumLogLevel { get; init; } = LogLevel.Information;
}
