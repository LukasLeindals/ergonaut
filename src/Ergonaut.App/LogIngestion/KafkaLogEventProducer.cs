using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.EventIngestion;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;

namespace Ergonaut.App.LogIngestion;


public sealed class KafkaLogEventProducer : IEventProducer<ILogEvent>, IDisposable
{

    private readonly KafkaLogEventOptions _options;
    private ProducerConfig _producerConfig;
    private IProducer<Null, byte[]> _producer;
    private readonly ILogger<KafkaLogEventProducer> _logger;

    public KafkaLogEventProducer(KafkaLogEventOptions options, ILogger<KafkaLogEventProducer> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _producerConfig = new ProducerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            ClientId = "ErgonautLogEventProducer",
            EnableIdempotence = true, // ensure exactly-once delivery
            Acks = Acks.All // wait for all replicas to acknowledge
        };
        _producer = new ProducerBuilder<Null, byte[]>(_producerConfig)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka producer error: {Reason}", e.Reason))
            .Build();
    }

    public KafkaLogEventProducer(IOptions<KafkaLogEventOptions> options, ILogger<KafkaLogEventProducer> logger)
        : this(options?.Value ?? throw new ArgumentNullException(nameof(options)), logger)
    {

    }

    public async Task ProduceAsync(IEnumerable<ILogEvent> events, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Producing {EventCount} log event(s) to Kafka topic {Topic}", events.Count(), _options.Topic);

        foreach (ILogEvent logEvent in events)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var message = new Message<Null, byte[]>
            {
                Value = new KafkaLogEventEnvelope(logEvent).Wrap()
            };

            try
            {
                var deliveryResult = await _producer.ProduceAsync(_options.Topic, message, cancellationToken).ConfigureAwait(false);
            }
            catch (ProduceException<Null, byte[]> ex)
            {
                _logger.LogError(ex, "Failed to deliver log event: {Reason}", ex.Error.Reason);
                throw; // pass on to retry mechanisms upstream
            }
        }

    }

    public void Dispose()
    {
        _producer.Flush(TimeSpan.FromSeconds(10));
        _producer.Dispose();
    }
}
