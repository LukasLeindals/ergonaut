namespace Ergonaut.Core.LogIngestion.PayloadParser;

/// <summary>
/// Describes the environment in which an ingestion request was received.
/// </summary>
public sealed record PayloadParserContext(

    /// <summary>
    /// The declared content type of the payload (e.g., "application/json", "application/x-protobuf").
    /// </summary>
    string ContentType,

    /// <summary>
    /// Optional identifier for the producer of the payload (service name, agent id, etc.).
    /// </summary>
    string? Source,

    /// <summary>
    /// Request headers or equivalent metadata supplied by the transport.
    /// </summary>
    IReadOnlyDictionary<string, string?> Headers
);
