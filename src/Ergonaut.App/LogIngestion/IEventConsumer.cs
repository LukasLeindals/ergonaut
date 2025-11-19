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
}
