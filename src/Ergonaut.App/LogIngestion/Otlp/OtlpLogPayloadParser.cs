using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.App.LogIngestion.PayloadParser;
using Google.Protobuf;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.App.LogIngestion.Otlp;

/// <summary>
/// Parses OTLP log payloads delivered over HTTP in either protobuf or JSON encoding.
/// </summary>
public sealed class OtlpLogPayloadParser : IPayloadParser<ExportLogsServiceRequest>
{
    private static readonly JsonParser JsonParser = new(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));

    public Task<PayloadParseResult<ExportLogsServiceRequest>> ParseAsync(
        ReadOnlyMemory<byte> payload,
        PayloadParserContext context,
        CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (payload.IsEmpty)
        {
            return Task.FromResult(PayloadParseResult<ExportLogsServiceRequest>.Failure("Payload is empty."));
        }

        var format = ResolveFormat(context.ContentType);

        try
        {
            var request = format == ContentFormat.Json
                ? JsonParser.Parse<ExportLogsServiceRequest>(Encoding.UTF8.GetString(payload.Span))
                : ExportLogsServiceRequest.Parser.ParseFrom(payload.Span);

            return Task.FromResult(PayloadParseResult<ExportLogsServiceRequest>.Success(request));
        }
        catch (InvalidProtocolBufferException ex)
        {
            return Task.FromResult(PayloadParseResult<ExportLogsServiceRequest>.Failure($"Invalid OTLP payload: {ex.Message}"));
        }
    }

    private static ContentFormat ResolveFormat(string? contentType) =>
        contentType is not null && contentType.Contains("json", StringComparison.OrdinalIgnoreCase)
            ? ContentFormat.Json
            : ContentFormat.Protobuf;

    private enum ContentFormat
    {
        Protobuf,
        Json
    }
}
