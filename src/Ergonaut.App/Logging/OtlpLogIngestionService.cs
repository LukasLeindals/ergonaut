using Ergonaut.Core.Models.Logging;
using OpenTelemetry.Proto.Collector.Logs.V1;

namespace Ergonaut.App.Logging;

public sealed class OtlpLogIngestionService
{
    private readonly ILogEventSink _sink;

    public OtlpLogIngestionService(ILogEventSink sink)
    {
        _sink = sink ?? throw new ArgumentNullException(nameof(sink));
    }

    /// <summary>
    /// Ingests log events from an OTLP export request.
    /// </summary>
    /// <param name="request">The OTLP log export request.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The number of ingested log events.</returns>
    public async Task<int> IngestAsync(ExportLogsServiceRequest request, CancellationToken cancellationToken = default)
    {
        if (request is null)
            throw new ArgumentNullException(nameof(request));

        var collected = new List<ILogEvent>();

        foreach (var resourceLogs in request.ResourceLogs)
        {
            foreach (var scopeLogs in resourceLogs.ScopeLogs)
            {
                foreach (var logRecord in scopeLogs.LogRecords)
                {
                    if (!OtlpLogEventAdapter.TryConvert(out ILogEvent logEvent, logRecord, resourceLogs.Resource))
                    {
                        continue;
                    }

                    collected.Add(logEvent);
                }
            }
        }

        if (collected.Count > 0)
        {
            await _sink.PublishAsync(collected, cancellationToken).ConfigureAwait(false);
        }

        return collected.Count;
    }




}
