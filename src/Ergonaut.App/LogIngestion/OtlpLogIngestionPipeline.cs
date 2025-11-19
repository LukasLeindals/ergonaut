using Ergonaut.Core.EventIngestion;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.LogIngestion.PayloadParser;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.App.LogIngestion;

/// <summary>
/// OTLP-specific implementation of <see cref="ILogIngestionPipeline"/> that composes parsing, transformation, and dispatch.
/// </summary>
public sealed class OtlpLogIngestionPipeline : ILogIngestionPipeline
{
    private readonly IPayloadParser<ExportLogsServiceRequest> _parser;
    private readonly IEventProducer<ILogEvent> _eventProducer;
    private readonly ILogger<OtlpLogIngestionPipeline> _logger;

    private readonly OtlpLogIngestionOptions _options;

    public OtlpLogIngestionPipeline(
        IPayloadParser<ExportLogsServiceRequest> parser,
        IEventProducer<ILogEvent> eventProducer,
        ILogger<OtlpLogIngestionPipeline> logger,
        IOptions<OtlpLogIngestionOptions> options)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _eventProducer = eventProducer ?? throw new ArgumentNullException(nameof(eventProducer));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
    }

    public async Task<LogIngestionResult> IngestAsync(
        ReadOnlyMemory<byte> payload,
        PayloadParserContext context,
        CancellationToken cancellationToken = default)
    {
        if (payload.IsEmpty)
        {
            return LogIngestionResult.Failure("Payload is empty.");
        }

        var parseResult = await _parser.ParseAsync(payload, context, cancellationToken).ConfigureAwait(false);
        if (!parseResult.IsSuccess || parseResult.Payload is null)
        {
            if (parseResult.Errors.Count > 0)
            {
                _logger.LogWarning("Failed to parse OTLP payload. Reason(s): {Errors}", string.Join(", ", parseResult.Errors));
            }

            return LogIngestionResult.Failure(parseResult.Errors);
        }

        var transformation = OtlpLogEventAdapter.Transform(parseResult.Payload, _options, cancellationToken);

        if (transformation.Events.Count > 0)
        {
            await _eventProducer.ProduceAsync(transformation.Events, cancellationToken).ConfigureAwait(false);
        }

        if (transformation.DroppedEventCount > 0)
        {
            _logger.LogWarning(
                "Dropped {Dropped} OTLP log record(s) during transformation (source: {Source}).",
                transformation.DroppedEventCount,
                context.Source ?? "unknown");
        }

        return transformation;
    }
}
