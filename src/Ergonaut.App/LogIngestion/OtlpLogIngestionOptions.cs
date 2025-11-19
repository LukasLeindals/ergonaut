namespace Ergonaut.App.LogIngestion;

public sealed class OtlpLogIngestionOptions
{
    /// <summary>
    /// Optional static API key for OTLP ingestion; if set, requests must present this key.
    /// </summary>
    public string? ApiKey { get; set; }
    public string? TraceViewerBaseUrl { get; set; }
}
