using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ergonaut.App.LogIngestion;

public interface IEventConsumer<T>
{
    /// <summary>
    /// Starts consuming events from the specified topic.
    /// </summary>
    /// <param name="topic"></param>
    /// <param name="handleEvent">A function to handle the consumed events.</param>
    /// <param name="cancellationToken"></param>
    Task StartConsuming(string topic, Func<T, CancellationToken, ValueTask> handleEvent, CancellationToken cancellationToken = default);

    /// <summary>
    /// Provides an async stream of events for consumers that prefer pull-based processing.
    /// </summary>
    IAsyncEnumerable<T> ConsumeAsync(string topic, CancellationToken cancellationToken = default);
}
