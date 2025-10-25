using System.Threading.Channels;
using Ergonaut.Core.Models.Logging;

namespace Ergonaut.App.Logging;
public sealed class LogEventSubscriber : IDisposable
{
    private readonly Channel<ILogEvent> _channel;
    private readonly CancellationToken _token;

    internal string Name { get; }

    internal LogEventSubscriber(string name, Channel<ILogEvent> channel, CancellationToken token)
    {
        Name = name;
        _channel = channel;
        _token = token;
    }

    internal ChannelReader<ILogEvent> Reader => _channel.Reader;

    internal bool TryWrite(ILogEvent logEvent)
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

    internal int FailureCount { get; private set; }
    private void ResetFailures() => FailureCount = 0;
    internal void RecordFailure() => FailureCount++;
}
