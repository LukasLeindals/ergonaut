
namespace Ergonaut.Core.LogIngestion;

public sealed class LogEventSubscription : IDisposable
{
    public IAsyncEnumerable<ILogEvent> Events { get; }
    private readonly Action _disposeAction;

    public LogEventSubscription(IAsyncEnumerable<ILogEvent> events, Action disposeAction)
    {
        Events = events ?? throw new ArgumentNullException(nameof(events));
        _disposeAction = disposeAction ?? throw new ArgumentNullException(nameof(disposeAction));
    }

    public void Dispose()
    {
        _disposeAction();
    }
}
