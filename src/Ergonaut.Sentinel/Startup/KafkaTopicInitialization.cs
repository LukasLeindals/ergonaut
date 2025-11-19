using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Confluent.Kafka;
using Confluent.Kafka.Admin;
using Ergonaut.App.LogIngestion.Kafka;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Ergonaut.Sentinel.Startup;

internal sealed class KafkaTopicInitializerHostedService : IHostedService
{
    private readonly ILogger<KafkaTopicInitializerHostedService> _logger;
    private readonly KafkaLogEventOptions _options;

    public KafkaTopicInitializerHostedService(
        ILogger<KafkaTopicInitializerHostedService> logger,
        IOptions<KafkaLogEventOptions> options)
    {
        _logger = logger;
        _options = options.Value;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var adminConfig = new AdminClientConfig { BootstrapServers = _options.BootstrapServers };
        using var admin = new AdminClientBuilder(adminConfig).Build();

        try
        {
            var createTopicsTask = admin.CreateTopicsAsync(
                new[]
                {
                    new TopicSpecification
                    {
                        Name = _options.Topic,
                        NumPartitions = 3,
                        ReplicationFactor = 1
                    }
                });

            await createTopicsTask.WaitAsync(cancellationToken);

            _logger.LogInformation("Created Kafka topic '{Topic}'.", _options.Topic);
        }
        catch (CreateTopicsException ex) when (ex.Results.All(r => r.Error.Code == ErrorCode.TopicAlreadyExists))
        {
            _logger.LogDebug("Kafka topic '{Topic}' already exists.", _options.Topic);
        }
        catch (CreateTopicsException ex)
        {
            _logger.LogError(ex, "Failed to create Kafka topic '{Topic}'.", _options.Topic);
            throw;
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
