namespace Ergonaut.Core.Models.Logging;

public interface ILogEventSink
{
    /// <summary>
    /// Publishes log events to the sink.
    /// </summary>
    Task PublishAsync(IEnumerable<ILogEvent> events, CancellationToken cancellationToken = default);
}
