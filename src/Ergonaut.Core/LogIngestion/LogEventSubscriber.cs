using System.Threading.Channels;

namespace Ergonaut.Core.LogIngestion;
public sealed class LogEventSubscriber : IDisposable
{
    private readonly Channel<ILogEvent> _channel;
    private readonly CancellationToken _token;

    public string Name { get; }

    public LogEventSubscriber(string name, Channel<ILogEvent> channel, CancellationToken token)
    {
        Name = name;
        _channel = channel;
        _token = token;
    }

    private ChannelReader<ILogEvent> Reader => _channel.Reader;

    public bool TryWrite(ILogEvent logEvent)
    {
        if (_token.IsCancellationRequested)
            return false;

        // Fast path: buffer has space, so enqueue without awaiting.
        if (_channel.Writer.TryWrite(logEvent))
        {
            ResetFailures();
            return true;
        }

        // Buffer is saturated; signal the caller to drop this subscriber.
        RecordFailure();
        return false;
    }

    public void Dispose()
    {
        _channel.Writer.TryComplete();
    }

    public int FailureCount { get; private set; }
    private void ResetFailures() => FailureCount = 0;
    private void RecordFailure() => FailureCount++;
}
