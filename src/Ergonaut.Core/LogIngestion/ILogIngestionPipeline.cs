using Ergonaut.Core.LogIngestion.PayloadParser;
namespace Ergonaut.Core.LogIngestion;

public interface ILogIngestionPipeline
{
    /// <summary>
    /// Ingests raw log payloads, transforming them into domain events and publishing to downstream sinks.
    /// </summary>
    /// <param name="payload">Opaque payload (JSON, protobuf, etc.). The pipeline delegates decoding to an <see cref="ILogEventParser" />.</param>
    /// <param name="context">Request metadata (content type, auth scope, tenant id, headers). Treat this as immutable hints.</param>
    /// <param name="cancellationToken">Stops ingestion mid-flight; pipeline should cancel parsing, transformation, and publishing promptly.</param>
    /// <returns>A result describing how many events were published and any non-fatal issues.</returns>
    Task<LogIngestionResult> IngestAsync(
        ReadOnlyMemory<byte> payload,
        PayloadParserContext context,
        CancellationToken cancellationToken = default
    );
}
