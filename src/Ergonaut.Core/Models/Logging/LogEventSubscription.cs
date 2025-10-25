
namespace Ergonaut.Core.Models.Logging;

public sealed record LogEventSubscription(
    IAsyncEnumerable<ILogEvent> Events,
    Action Dispose
);
