using System.Collections.Concurrent;
using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Ergonaut.Core.Models.Logging;

namespace Ergonaut.App.Sentinel;

/// <summary>
/// In-memory fan-out hub that bridges ILogEventSource producers to subscribers.
/// </summary>
public sealed class LogEventHub
{
    private readonly ConcurrentDictionary<Guid, Channel<ILogEvent>> _subscribers = new();

    /// <summary>
    /// Publishes log events to all subscribers.
    /// </summary>
    public async Task PublishAsync(ILogEvent logEvent, CancellationToken cancellationToken = default)
    {
        foreach (var channel in _subscribers.Values)
        {
            if (!channel.Writer.TryWrite(logEvent))
            {
                await channel.Writer.WriteAsync(logEvent, cancellationToken).ConfigureAwait(false);
            }
        }
    }

    /// <summary>
    /// Subscribes to log events. The returned async enumerable will yield log events as they arrive.
    /// </summary>
    public IAsyncEnumerable<ILogEvent> SubscribeAsync(CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateUnbounded<ILogEvent>(new UnboundedChannelOptions
        {
            SingleReader = true,
            AllowSynchronousContinuations = false
        });

        var subscriptionId = Guid.NewGuid();
        _subscribers[subscriptionId] = channel;

        return ReadAllAsync(subscriptionId, channel, cancellationToken);
    }

    /// <summary>
    /// Reads all log events from the specified channel until cancellation is requested.
    /// Cleans up the subscription when done.
    /// </summary>
    private async IAsyncEnumerable<ILogEvent> ReadAllAsync(
        Guid subscriptionId,
        Channel<ILogEvent> channel,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        try
        {
            await foreach (var entry in channel.Reader.ReadAllAsync(cancellationToken))
            {
                yield return entry;
            }
        }
        finally
        {
            _subscribers.TryRemove(subscriptionId, out _);
            channel.Writer.TryComplete();
        }
    }
}
