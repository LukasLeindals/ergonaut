namespace Ergonaut.Core.LogIngestion;

public record LogIngestionResult(
    int IngestedEventCount,
    int FailedEventCount
);
