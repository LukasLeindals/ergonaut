namespace Ergonaut.App.LogIngestion;

public interface IEventProducer<T>
{
    /// <summary>
    /// Adds events to the ingestion pipeline.
    /// </summary>
    Task ProduceAsync(IEnumerable<T> events, CancellationToken cancellationToken = default);
}
