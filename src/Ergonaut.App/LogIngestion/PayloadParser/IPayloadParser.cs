namespace Ergonaut.App.LogIngestion.PayloadParser;

/// <summary>
/// Parses raw payloads into protocol-specific log representations.
/// </summary>
public interface IPayloadParser<TPayload>
{
    /// <summary>
    /// Attempts to parse the supplied payload into a structured request object.
    /// </summary>
    /// <param name="payload">Raw payload bytes.</param>
    /// <param name="context">Contextual metadata describing the request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>A result describing whether parsing succeeded.</returns>
    Task<PayloadParseResult<TPayload>> ParseAsync(
        ReadOnlyMemory<byte> payload,
        PayloadParserContext context,
        CancellationToken cancellationToken = default
    );
}
