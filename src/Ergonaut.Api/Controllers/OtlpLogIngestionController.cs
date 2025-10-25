using System.Text;
using Ergonaut.App.LogIngestion;
using Ergonaut.Core.LogIngestion;
using Google.Protobuf;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/otlp/v1/logs")]
[AllowAnonymous]
public sealed class OtlpLogIngestionController : ControllerBase
{
    private static readonly JsonParser Parser = new(JsonParser.Settings.Default.WithIgnoreUnknownFields(true));
    private static readonly JsonFormatter Formatter = new(JsonFormatter.Settings.Default.WithPreserveProtoFieldNames(true));

    private readonly ILogIngestionService<ExportLogsServiceRequest> _ingestionService;
    private readonly ILogger<OtlpLogIngestionController> _logger;

    public OtlpLogIngestionController(ILogIngestionService<ExportLogsServiceRequest> ingestionService, ILogger<OtlpLogIngestionController> logger)
    {
        _ingestionService = ingestionService;
        _logger = logger;
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

        var contentFormat = ResolveFormat(Request.ContentType);

        ExportLogsServiceRequest request;
        try
        {
            request = await ParseRequestAsync(contentFormat, cancellationToken).ConfigureAwait(false);
        }
        catch (InvalidProtocolBufferException ex)
        {
            _logger.LogWarning(ex, "Failed to parse OTLP payload.");
            return BadRequest("Invalid OTLP payload.");
        }

        LogIngestionResult logIngestionResult = await _ingestionService.IngestAsync(request, cancellationToken).ConfigureAwait(false);

        _logger.LogInformation("Ingested {Count} log records from OTLP request.", logIngestionResult.IngestedEventCount);

        var response = new ExportLogsServiceResponse();
        return contentFormat switch
        {
            ContentFormat.Json => Content(Formatter.Format(response), "application/json", Encoding.UTF8),
            _ => File(response.ToByteArray(), "application/x-protobuf")
        };
    }

    private async Task<ExportLogsServiceRequest> ParseRequestAsync(ContentFormat format, CancellationToken cancellationToken)
    {
        if (format == ContentFormat.Json)
        {
            using var reader = new StreamReader(Request.Body, Encoding.UTF8, detectEncodingFromByteOrderMarks: false, leaveOpen: true);
            var json = await reader.ReadToEndAsync(cancellationToken).ConfigureAwait(false);
            return Parser.Parse<ExportLogsServiceRequest>(json);
        }

        await using var ms = new MemoryStream();
        await Request.Body.CopyToAsync(ms, cancellationToken).ConfigureAwait(false);
        ms.Position = 0;
        return ExportLogsServiceRequest.Parser.ParseFrom(ms);
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
