using Ergonaut.Core.EventIngestion;
using Ergonaut.Core.LogIngestion;
using Ergonaut.App.Sentinel;
using Microsoft.Extensions.Options;
using Ergonaut.App.LogIngestion;
namespace Ergonaut.Sentinel;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IEventConsumer<ILogEvent> _logEventConsumer;
    private readonly ILogEventFilter _logEventFilter;
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly KafkaLogEventOptions _kafkaOptions;

    public Worker(ILogger<Worker> logger, IEventConsumer<ILogEvent> logEventConsumer, ILogEventFilter logEventFilter, IServiceScopeFactory scopeFactory, IOptions<KafkaLogEventOptions> kafkaOptions)
    {
        _logger = logger;
        _logEventConsumer = logEventConsumer;
        _logEventFilter = logEventFilter;
        _scopeFactory = scopeFactory ?? throw new ArgumentNullException(nameof(scopeFactory));
        _kafkaOptions = kafkaOptions.Value;
    }
    public async ValueTask HandleEvent(ILogEvent logEvent, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling log event using Sentinel filter");
        bool accept = await _logEventFilter.Accept(logEvent, cancellationToken);
        if (accept)
        {
            _logger.LogInformation("Processing log event with message: {Message}", logEvent.Message);
            using var scope = _scopeFactory.CreateScope();
            var workItemCreator = scope.ServiceProvider.GetRequiredService<IWorkItemCreator>();
            await workItemCreator.CreateWorkItem(logEvent, cancellationToken);
        }
        else
        {
            _logger.LogInformation("Ignoring log event with message: {Message}", logEvent.Message);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {

        _logger.LogInformation("Sentinel Worker consuming from Kafka topic '{topic}' running at: {time}", _kafkaOptions.Topic, DateTimeOffset.Now);
        await _logEventConsumer.StartConsuming(_kafkaOptions.Topic, HandleEvent, stoppingToken);

    }
}
