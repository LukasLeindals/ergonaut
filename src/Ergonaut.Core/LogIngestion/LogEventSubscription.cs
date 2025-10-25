
namespace Ergonaut.Core.LogIngestion;

public sealed record LogEventSubscription(
    IAsyncEnumerable<ILogEvent> Events,
    Action Dispose
);
