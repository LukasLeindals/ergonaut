using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.LogIngestion.PayloadParser;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/otlp/v1/logs")]
[AllowAnonymous]
public sealed class OtlpLogIngestionController : ControllerBase
{
    private const string SOURCE = "HTTP OTLP Log Ingestion Endpoint";
    private static readonly JsonFormatter Formatter = new(JsonFormatter.Settings.Default.WithPreserveProtoFieldNames(true));

    private readonly ILogIngestionPipeline _pipeline;
    private readonly ILogger<OtlpLogIngestionController> _logger;

    public OtlpLogIngestionController(ILogIngestionPipeline pipeline, ILogger<OtlpLogIngestionController> logger)
    {
        _pipeline = pipeline ?? throw new ArgumentNullException(nameof(pipeline));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    [HttpPost]
    [Consumes("application/x-protobuf", "application/json")]
    [Produces("application/x-protobuf", "application/json")]
    public async Task<IActionResult> IngestAsync(CancellationToken cancellationToken)
    {
        if (Request.ContentLength is 0)
        {
            return BadRequest("Payload is required.");
        }

        var payloadBytes = await ReadBodyAsync(cancellationToken).ConfigureAwait(false);

        var context = new PayloadParserContext(
            ContentType: Request.ContentType ?? throw new InvalidOperationException("Content-Type header is missing."),
            Source: SOURCE,
            Headers: GetHeaders(Request.Headers));

        var result = await _pipeline.IngestAsync(payloadBytes, context, cancellationToken).ConfigureAwait(false);

        if (!result.IsSuccess)
        {
            _logger.LogWarning("Rejected OTLP payload. Reasons: {Reasons}", string.Join(", ", result.Warnings));
            return BadRequest("Invalid OTLP payload.");
        }

        _logger.LogInformation(
            "Ingested {Count} OTLP log record(s) and dropped {Dropped}.",
            result.Events.Count,
            result.DroppedEventCount);

        var response = new ExportLogsServiceResponse();
        return ResolveFormat(Request.ContentType) switch
        {
            ContentFormat.Json => Content(Formatter.Format(response), "application/json", Encoding.UTF8),
            _ => File(response.ToByteArray(), "application/x-protobuf")
        };
    }

    private async Task<ReadOnlyMemory<byte>> ReadBodyAsync(CancellationToken cancellationToken)
    {
        var estimatedSize = Request.ContentLength is > 0 and <= int.MaxValue
            ? (int)Request.ContentLength.Value
            : 0;

        await using var buffer = estimatedSize > 0 ? new MemoryStream(estimatedSize) : new MemoryStream();
        await Request.Body.CopyToAsync(buffer, cancellationToken).ConfigureAwait(false);
        return buffer.ToArray();
    }

    private static ContentFormat ResolveFormat(string? contentType) =>
        contentType is not null && contentType.Contains("json", StringComparison.OrdinalIgnoreCase)
            ? ContentFormat.Json
            : ContentFormat.Protobuf;

    private static IReadOnlyDictionary<string, string?> GetHeaders(IHeaderDictionary headers)
    {
        var filteredHeaders = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        foreach (var header in headers)
        {
            filteredHeaders[header.Key] = header.Value.ToString();
        }

        return filteredHeaders;
    }

    private enum ContentFormat
    {
        Protobuf,
        Json
    }
}
