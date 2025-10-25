using System;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.LogIngestion.PayloadParser;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.App.LogIngestion;

/// <summary>
/// OTLP-specific implementation of <see cref="ILogIngestionPipeline"/> that composes parsing, transformation, and dispatch.
/// </summary>
public sealed class OtlpLogIngestionPipeline : ILogIngestionPipeline
{
    private readonly IPayloadParser<ExportLogsServiceRequest> _parser;
    private readonly ILogEventSink _sink;
    private readonly ILogger<OtlpLogIngestionPipeline> _logger;

    public OtlpLogIngestionPipeline(
        IPayloadParser<ExportLogsServiceRequest> parser,
        ILogEventSink sink,
        ILogger<OtlpLogIngestionPipeline> logger)
    {
        _parser = parser ?? throw new ArgumentNullException(nameof(parser));
        _sink = sink ?? throw new ArgumentNullException(nameof(sink));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
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

        var transformation = OtlpLogEventAdapter.Transform(parseResult.Payload, cancellationToken);

        if (transformation.Events.Count > 0)
        {
            await _sink.PublishAsync(transformation.Events, cancellationToken).ConfigureAwait(false);
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
