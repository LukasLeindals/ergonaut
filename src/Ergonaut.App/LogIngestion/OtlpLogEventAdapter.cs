using System;
using System.Collections.Generic;
using System.Threading;
using Ergonaut.Core.LogIngestion;
using Google.Protobuf.Collections;
using Microsoft.Extensions.Logging;
using OpenTelemetry.Proto.Common.V1;
using OpenTelemetry.Proto.Logs.V1;
using OpenTelemetry.Proto.Resource.V1;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.App.LogIngestion;

/// <summary>
/// Translates Open Telemetry Protocol (OTLP) log records into domain log events.
/// </summary>
public sealed class OtlpLogEventAdapter
{
    public static LogIngestionResult Transform(ExportLogsServiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var events = new List<ILogEvent>();
        var warnings = new List<string>();
        var dropped = 0;

        foreach (ResourceLogs resourceLogs in request.ResourceLogs)
        {
            cancellationToken.ThrowIfCancellationRequested();

            foreach (ScopeLogs scopeLogs in resourceLogs.ScopeLogs)
            {
                foreach (LogRecord logRecord in scopeLogs.LogRecords)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    if (!TryConvert(out var logEvent, logRecord, resourceLogs.Resource))
                    {
                        dropped++;
                        var severity = logRecord.SeverityText ?? logRecord.SeverityNumber.ToString();
                        warnings.Add($"Dropped OTLP log record (severity: {severity}).");
                        continue;
                    }

                    events.Add(logEvent);
                }
            }
        }

        return LogIngestionResult.Success(events, dropped, warnings);
    }

    public static bool TryConvert(out ILogEvent logEvent, LogRecord logRecord, Resource? resource)
    {
        if (logRecord is null)
            throw new ArgumentNullException(nameof(logRecord));

        var message = ExtractMessage(logRecord);
        if (string.IsNullOrWhiteSpace(message))
        {
            logEvent = null!;
            return false;
        }

        var source = SelectSource(logRecord, ResolveServiceName(resource));
        var timestamp = ResolveTimestamp(logRecord);
        var level = MapSeverity(logRecord.SeverityNumber);
        string? messageTemplate = ExtractMessageTemplate(logRecord);

        logEvent = new LogEvent(message, source, timestamp, level, messageTemplate);
        return true;
    }

    private static string ResolveServiceName(Resource? resource)
    {
        if (resource is null)
        {
            return "unknown-service";
        }

        var serviceName = TryGetAttribute(resource.Attributes, "service.name");
        if (string.IsNullOrWhiteSpace(serviceName))
        {
            return "unknown-service";
        }

        var serviceNamespace = TryGetAttribute(resource.Attributes, "service.namespace");
        return string.IsNullOrWhiteSpace(serviceNamespace)
            ? serviceName!
            : $"{serviceNamespace}.{serviceName}";
    }

    private static string SelectSource(LogRecord logRecord, string defaultSource)
    {
        var loggerName = TryGetAttribute(logRecord.Attributes, "logger.name");
        if (!string.IsNullOrWhiteSpace(loggerName))
        {
            return loggerName!;
        }

        return defaultSource;
    }

    private static string ExtractMessage(LogRecord logRecord)
    {
        if (logRecord.Body is null)
        {
            return logRecord.SeverityText;
        }

        return logRecord.Body.ValueCase switch
        {
            AnyValue.ValueOneofCase.StringValue => logRecord.Body.StringValue,
            AnyValue.ValueOneofCase.BoolValue => logRecord.Body.BoolValue.ToString(),
            AnyValue.ValueOneofCase.IntValue => logRecord.Body.IntValue.ToString(),
            AnyValue.ValueOneofCase.DoubleValue => logRecord.Body.DoubleValue.ToString("G"),
            AnyValue.ValueOneofCase.BytesValue => Convert.ToBase64String(logRecord.Body.BytesValue.ToByteArray()),
            _ => logRecord.SeverityText ?? string.Empty
        };
    }

    private static string? ExtractMessageTemplate(LogRecord logRecord)
    {
        foreach (string sep in new[] { "_", "." })
        {
            var template = TryGetAttribute(logRecord.Attributes, $"messagetemplate", key => key.ToLowerInvariant().Replace(sep, ""));
            if (!string.IsNullOrWhiteSpace(template))
            {
                return template;
            }
        }
        return null;
    }

    private static DateTimeOffset ResolveTimestamp(LogRecord logRecord)
    {
        if (logRecord.TimeUnixNano > 0)
        {
            return FromUnixNanoseconds(logRecord.TimeUnixNano);
        }

        return DateTimeOffset.UtcNow;
    }

    private static DateTimeOffset FromUnixNanoseconds(ulong unixNano)
    {
        var seconds = (long)(unixNano / 1_000_000_000);
        var nanoseconds = (long)(unixNano % 1_000_000_000);
        return DateTimeOffset.FromUnixTimeSeconds(seconds).AddTicks(nanoseconds / 100);
    }

    private static LogLevel MapSeverity(SeverityNumber severity) =>
        severity switch
        {
            SeverityNumber.Trace or SeverityNumber.Trace2 or SeverityNumber.Trace3 or SeverityNumber.Trace4 => LogLevel.Trace,
            SeverityNumber.Debug or SeverityNumber.Debug2 or SeverityNumber.Debug3 or SeverityNumber.Debug4 => LogLevel.Debug,
            SeverityNumber.Info or SeverityNumber.Info2 or SeverityNumber.Info3 or SeverityNumber.Info4 => LogLevel.Information,
            SeverityNumber.Warn or SeverityNumber.Warn2 or SeverityNumber.Warn3 or SeverityNumber.Warn4 => LogLevel.Warning,
            SeverityNumber.Error or SeverityNumber.Error2 or SeverityNumber.Error3 or SeverityNumber.Error4 => LogLevel.Error,
            SeverityNumber.Fatal or SeverityNumber.Fatal2 or SeverityNumber.Fatal3 or SeverityNumber.Fatal4 => LogLevel.Critical,
            _ => LogLevel.Information
        };

    private static string? TryGetAttribute(RepeatedField<KeyValue> attributes, string key, Func<string, string>? transform = null)
    {
        foreach (var attribute in attributes)
        {
            string attributeKey;
            if (transform != null)
                attributeKey = transform(attribute.Key);
            else
                attributeKey = attribute.Key;

            if (string.Equals(attributeKey, key, StringComparison.OrdinalIgnoreCase))
            {
                return attribute.Value.ValueCase switch
                {
                    AnyValue.ValueOneofCase.StringValue => attribute.Value.StringValue,
                    AnyValue.ValueOneofCase.IntValue => attribute.Value.IntValue.ToString(),
                    AnyValue.ValueOneofCase.BoolValue => attribute.Value.BoolValue.ToString(),
                    AnyValue.ValueOneofCase.DoubleValue => attribute.Value.DoubleValue.ToString("G"),
                    _ => null
                };
            }
        }

        return null;
    }
}
