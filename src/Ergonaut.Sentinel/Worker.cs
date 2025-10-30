using Ergonaut.Core.LogIngestion;
using Ergonaut.App.Sentinel;
namespace Ergonaut.Sentinel;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly ILogEventSource _logEventSource;
    private readonly ILogEventFilter _logEventFilter;

    public Worker(ILogger<Worker> logger, ILogEventSource logEventSource, ILogEventFilter logEventFilter)
    {
        _logger = logger;
        _logEventSource = logEventSource;
        _logEventFilter = logEventFilter;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("Sentinel Worker running at: {time}", DateTimeOffset.Now);
        using var subscription = _logEventSource.Subscribe("Sentinel", stoppingToken);

        await foreach (var logEvent in subscription.Events.WithCancellation(stoppingToken))
        {
            _logger.LogInformation("Accepting log event");
            bool accept = _logEventFilter.Accept(logEvent, stoppingToken);
        }

    }
}
