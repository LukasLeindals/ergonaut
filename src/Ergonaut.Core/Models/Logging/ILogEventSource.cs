namespace Ergonaut.Core.Models.Logging;

public interface ILogEventSource
{
    LogEventSubscription Subscribe(string? subscriberName = null, CancellationToken cancellationToken = default);
}
