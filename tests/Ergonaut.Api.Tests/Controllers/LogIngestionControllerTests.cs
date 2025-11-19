
using System.Text;
using Ergonaut.Api.Controllers;
using Ergonaut.App.Extensions;
using Ergonaut.App.LogIngestion;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.EventIngestion;
using Google.Protobuf;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Configuration;
using OpenTelemetry.Proto.Collector.Logs.V1;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Logs.V1;
using OpenTelemetry.Proto.Resource.V1;

namespace Ergonaut.Api.Tests.Controllers;

public sealed class LogIngestionControllerTests
{
    [Fact(DisplayName = "IngestAsync publishes log events for a JSON OTLP payload")]
    public async Task IngestAsync_Publishes_LogEvents_For_Json()
    {
        var timestamp = new DateTimeOffset(2025, 1, 15, 12, 30, 0, TimeSpan.Zero);
        var exportRequest = BuildExportRequest(timestamp);

        var formatter = new JsonFormatter(JsonFormatter.Settings.Default.WithPreserveProtoFieldNames(true));
        var payload = Encoding.UTF8.GetBytes(formatter.Format(exportRequest));

        var sink = new RecordingLogEventSink();
        var controller = CreateController(payload, "application/json", sink);

        var result = await controller.IngestAsync("Bearer test-key", null, CancellationToken.None);

        var contentResult = Assert.IsType<ContentResult>(result);
        Assert.StartsWith("application/json", contentResult.ContentType, StringComparison.OrdinalIgnoreCase);
        Assert.NotNull(contentResult.Content);

        var logEvent = Assert.Single(sink.Events);
        Assert.Equal("Test log message", logEvent.Message);
        Assert.Equal("Telemetry.Logger", logEvent.Source);
        Assert.Equal(LogLevel.Information, logEvent.Level);
        Assert.Equal(timestamp, logEvent.Timestamp);
    }

    [Fact(DisplayName = "IngestAsync publishes log events for a protobuf OTLP payload")]
    public async Task IngestAsync_Publishes_LogEvents_For_Protobuf()
    {
        var timestamp = new DateTimeOffset(2025, 1, 15, 12, 45, 0, TimeSpan.Zero);
        var exportRequest = BuildExportRequest(timestamp);
        var payload = exportRequest.ToByteArray();
        var options = new OtlpLogIngestionOptions
        {
            TraceViewerBaseUrl = "https://traces.example.com"
        };

        var parsedRequest = ExportLogsServiceRequest.Parser.ParseFrom(payload);

        // Ensure the adapter can translate the parsed payload directly.
        Assert.True(
            OtlpLogEventAdapter.TryConvert(
                out var convertedEvent,
                options,
                parsedRequest.ResourceLogs[0].ScopeLogs[0].LogRecords[0],
                parsedRequest.ResourceLogs[0],
                parsedRequest.ResourceLogs[0].ScopeLogs[0]));
        Assert.NotNull(convertedEvent);

        var sink = new RecordingLogEventSink();
        var controller = CreateController(payload, "application/x-protobuf", sink);

        var result = await controller.IngestAsync("Bearer test-key", null, CancellationToken.None);

        var fileResult = Assert.IsType<FileContentResult>(result);
        Assert.Equal("application/x-protobuf", fileResult.ContentType);
        Assert.Empty(fileResult.FileContents);

        var logEvent = Assert.Single(sink.Events);
        Assert.Equal(timestamp, logEvent.Timestamp);
    }

    [Fact(DisplayName = "DI resolves OtlpLogIngestionController when application services are registered")]
    public void DependencyInjection_Resolves_Controller()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["LogIngestion:Kafka:BootstrapServers"] = "test",
                ["LogIngestion:Kafka:Topic"] = "test-topic",
                ["LogIngestion:Kafka:GroupId"] = "test-group"
            })
            .Build();

        var services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(configuration);
        services.AddLogging();
        services.AddApplicationServices();
        services.AddLogIngestion();
        services.AddSingleton<IEventProducer<ILogEvent>, RecordingLogEventSink>(); // avoid needing Kafka for tests
        services.AddTransient<OtlpLogIngestionController>();
        services.AddOptions();

        using var provider = services.BuildServiceProvider(validateScopes: true);
        using var scope = provider.CreateScope();

        Assert.NotNull(scope.ServiceProvider.GetRequiredService<OtlpLogIngestionController>());
    }

    private static OtlpLogIngestionController CreateController(byte[] payload, string contentType, RecordingLogEventSink sink)
    {
        var bodyStream = new MemoryStream();
        bodyStream.Write(payload, 0, payload.Length);
        bodyStream.Position = 0;

        var options = Microsoft.Extensions.Options.Options.Create(new OtlpLogIngestionOptions
        {
            TraceViewerBaseUrl = "https://traces.example.com",
            ApiKey = "test-key"
        });

        var controller = new OtlpLogIngestionController(CreatePipeline(sink, options), NullLogger<OtlpLogIngestionController>.Instance, options);

        var httpContext = new DefaultHttpContext();
        httpContext.Request.ContentType = contentType;
        httpContext.Request.ContentLength = payload.LongLength;
        httpContext.Request.Body = bodyStream;
        httpContext.Request.Headers.Authorization = "Bearer test-key";

        controller.ControllerContext = new ControllerContext
        {
            HttpContext = httpContext
        };

        controller.HttpContext.Request.Body.Position = 0;

        return controller;
    }

    private static OtlpLogIngestionPipeline CreatePipeline(IEventProducer<ILogEvent> producer, Microsoft.Extensions.Options.IOptions<OtlpLogIngestionOptions> options)
    {
        var parser = new OtlpLogPayloadParser();
        return new OtlpLogIngestionPipeline(parser, producer, NullLogger<OtlpLogIngestionPipeline>.Instance, options);
    }

    private static ExportLogsServiceRequest BuildExportRequest(DateTimeOffset timestamp)
    {
        var resourceLogs = new ResourceLogs
        {
            Resource = new Resource
            {
                Attributes =
                {
                    new KeyValue { Key = "service.name", Value = new AnyValue { StringValue = "orders" } },
                    new KeyValue { Key = "service.namespace", Value = new AnyValue { StringValue = "checkout" } }
                }
            }
        };

        var logRecord = new LogRecord
        {
            Body = new AnyValue { StringValue = "Test log message" },
            SeverityNumber = SeverityNumber.Info,
            SeverityText = "Information",
            TimeUnixNano = (ulong)timestamp.ToUnixTimeMilliseconds() * 1_000_000
        };

        logRecord.Attributes.Add(new KeyValue
        {
            Key = "logger.name",
            Value = new AnyValue { StringValue = "Telemetry.Logger" }
        });

        var scopeLogs = new ScopeLogs
        {
            Scope = new InstrumentationScope { Name = "ergonaut.test" }
        };
        scopeLogs.LogRecords.Add(logRecord);
        resourceLogs.ScopeLogs.Add(scopeLogs);

        var request = new ExportLogsServiceRequest();
        request.ResourceLogs.Add(resourceLogs);

        return request;
    }

    private sealed class RecordingLogEventSink : IEventProducer<ILogEvent>
    {
        public List<ILogEvent> Events { get; } = new();

        public Task ProduceAsync(IEnumerable<ILogEvent> events, CancellationToken cancellationToken = default)
        {
            Events.AddRange(events);
            return Task.CompletedTask;
        }
    }
}
