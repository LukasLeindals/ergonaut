using System.Linq;
using System.Threading;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Ergonaut.Core.Models.Logging;



namespace Ergonaut.App.Logging;


public sealed class LogEventHub : ILogEventSink, ILogEventSource, IDisposable
{
    private const int ALLOWED_FAILURES = 5;
    private static readonly TimeSpan ShutdownWaitTimeout = TimeSpan.FromSeconds(5);
    private readonly int _perSubscriberCapacity = 1_000;
    private readonly Channel<ILogEvent> _inbox;
    private readonly Lock _sync = new(); // Guards subscriber list so add/remove stay thread-safe.

    private readonly List<LogEventSubscriber> _subscribers = new();
    private readonly CancellationTokenSource _cts = new();
    private readonly Task _dispatcherTask;
    private readonly ILogger<LogEventHub> _logger;
    public LogEventHub(ILogger<LogEventHub> logger)
    {

        _inbox = Channel.CreateUnbounded<ILogEvent>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false
        });

        _logger = logger;
        _dispatcherTask = Task.Run(() => DispatchAsync(_cts.Token));
    }

    /// <summary>
    /// Publishes a collection of log events to the hub.
    /// </summary>
    public async Task PublishAsync(IEnumerable<ILogEvent> events, CancellationToken cancellationToken = default)
    {
        if (events is null)
            throw new ArgumentNullException(nameof(events));

        foreach (var logEvent in events)
        {
            cancellationToken.ThrowIfCancellationRequested();
            await _inbox.Writer.WriteAsync(logEvent, cancellationToken).ConfigureAwait(false);
        }
    }

    public LogEventSubscription Subscribe(string? subscriberName = null, CancellationToken cancellationToken = default)
    {
        var channel = Channel.CreateBounded<ILogEvent>(_perSubscriberCapacity);

        var subscriber = new LogEventSubscriber(subscriberName ?? "unknown", channel, cancellationToken);
        using (_sync.EnterScope())
        {
            _subscribers.Add(subscriber);
        }

        _logger.LogDebug("Registered log subscriber {SubscriberName}.", subscriber.Name);

        var events = channel.Reader.ReadAllAsync(cancellationToken);
        return new LogEventSubscription(events, () => { RemoveSubscriber(subscriber); });
    }

    /// <summary>
    /// Continuously forwards incoming log events to all active subscribers.
    /// </summary>
    private async Task DispatchAsync(CancellationToken cancellationToken)
    {
        await foreach (var logEvent in _inbox.Reader.ReadAllAsync(cancellationToken))
        {
            LogEventSubscriber[] currentSubscribers;

            using (_sync.EnterScope())
            {
                if (_subscribers.Count == 0)
                {
                    continue;
                }

                currentSubscribers = _subscribers.ToArray();
            }

            foreach (var subscriber in currentSubscribers)
            {
                if (!subscriber.TryWrite(logEvent) && subscriber.FailureCount > ALLOWED_FAILURES)
                {
                    _logger.LogWarning("Dropping log subscriber {SubscriberName} due to repeated backpressure.", subscriber.Name);
                    RemoveSubscriber(subscriber);
                }
            }
        }
    }

    private void RemoveSubscriber(LogEventSubscriber subscriber)
    {
        using (_sync.EnterScope())
        {
            if (_subscribers.Remove(subscriber))
            {
                _logger.LogDebug("Removed log subscriber {SubscriberName}.", subscriber.Name);
                subscriber.Dispose();
            }
        }
    }

    public void Dispose()
    {
        _inbox.Writer.TryComplete();
        _cts.Cancel();

        var dispatcherCompleted = false;
        try
        {
            dispatcherCompleted = _dispatcherTask.Wait(ShutdownWaitTimeout);
        }
        catch (AggregateException ex)
        {
            if (!ex.InnerExceptions.Any(e => !(e is OperationCanceledException)))
            {
                _logger.LogError(ex, "Log event dispatcher terminated unexpectedly.");
            }
            dispatcherCompleted = true;
        }

        if (!dispatcherCompleted)
        {
            _logger.LogWarning("Log event dispatcher did not shut down within {Timeout}.", ShutdownWaitTimeout);
        }

        using (_sync.EnterScope())
        {
            foreach (var subscriber in _subscribers)
            {
                subscriber.Dispose();
            }

            _subscribers.Clear();
        }

        _cts.Dispose();
    }
}
