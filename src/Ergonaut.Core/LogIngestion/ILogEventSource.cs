namespace Ergonaut.Core.LogIngestion;

public interface ILogEventSource
{
    LogEventSubscription Subscribe(string? subscriberName = null, CancellationToken cancellationToken = default);
}
