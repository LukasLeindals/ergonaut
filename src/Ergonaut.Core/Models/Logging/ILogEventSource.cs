namespace Ergonaut.Core.Models.Logging;

public interface ILogEventSource
{
    string Name { get; }

    LogStreamOptions Options { get; }

    IAsyncEnumerable<ILogEvent> ObserveAsync(CancellationToken cancellationToken = default);
}
