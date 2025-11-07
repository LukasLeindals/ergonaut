namespace Ergonaut.App.LogIngestion;

using Ergonaut.Core.EventIngestion;
using Ergonaut.Core.LogIngestion;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Confluent.Kafka;
using System.Threading;

public sealed class KafkaLogEventConsumer : IEventConsumer<ILogEvent>, IDisposable
{
    private readonly KafkaLogEventOptions _options;
    private readonly ILogger<KafkaLogEventConsumer> _logger;

    private IConsumer<Null, byte[]>? _consumer;

    public KafkaLogEventConsumer(KafkaLogEventOptions options, ILogger<KafkaLogEventConsumer> logger)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        var config = new ConsumerConfig
        {
            BootstrapServers = _options.BootstrapServers,
            GroupId = _options.GroupId,
            AutoOffsetReset = AutoOffsetReset.Earliest
        };
        _consumer = new ConsumerBuilder<Null, byte[]>(config)
            .SetErrorHandler((_, e) => _logger.LogError("Kafka consumer error: {Reason}", e.Reason))
            .Build();
    }

    public KafkaLogEventConsumer(IOptions<KafkaLogEventOptions> options, ILogger<KafkaLogEventConsumer> logger)
        : this(options?.Value ?? throw new ArgumentNullException(nameof(options)), logger)
    {

    }

    public async Task StartConsuming(string topic, Func<ILogEvent, CancellationToken, ValueTask> handleEvent, CancellationToken cancellationToken)
    {
        var consumer = _consumer ?? throw new ObjectDisposedException(nameof(KafkaLogEventConsumer));
        consumer.Subscribe(topic);
        _logger.LogInformation("Started consuming Kafka topic: {Topic}", topic);

        try
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var consumeResult = consumer.Consume(cancellationToken);
                    if (consumeResult != null)
                    {
                        if (!KafkaLogEventEnvelope.TryUnwrap(consumeResult.Message.Value, out var envelope))
                        {
                            _logger.LogError("Failed to unwrap Kafka log event envelope.");
                            continue;
                        }

                        ILogEvent logEvent = envelope!.Data;
                        await handleEvent(logEvent, cancellationToken);
                    }
                }
                catch (ConsumeException ex)
                {
                    _logger.LogError(ex, "Error consuming Kafka message: {Error}", ex.Error.Reason);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error during Kafka consumption: {Message}", ex.Message);
                }
            }
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Kafka consumption cancelled.");
        }
        finally
        {
            CloseConsumer();
            _logger.LogInformation("Kafka consumer closed.");
        }
    }

    public void Dispose()
    {
        CloseConsumer();
    }

    private void CloseConsumer()
    {
        var consumer = Interlocked.Exchange(ref _consumer, null);
        if (consumer is null)
        {
            return;
        }

        try
        {
            consumer.Close();
        }
        catch (ObjectDisposedException)
        {
            // already closed by another path
        }
        finally
        {
            consumer.Dispose();
        }
    }
}
