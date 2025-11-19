namespace Ergonaut.App.LogIngestion.PayloadParser;

/// <summary>
/// Represents the outcome of parsing a raw log payload.
/// </summary>
public sealed record PayloadParseResult<TPayload>
{
    private PayloadParseResult(bool isSuccess, TPayload? payload, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Payload = payload;
        Errors = errors;
    }

    public bool IsSuccess { get; }

    public TPayload? Payload { get; }

    public IReadOnlyList<string> Errors { get; }

    public static PayloadParseResult<TPayload> Success(TPayload payload) =>
        new(true, payload, Array.Empty<string>());

    public static PayloadParseResult<TPayload> Failure(params string[] errors) =>
        new(false, payload: default, errors: errors);

    public static PayloadParseResult<TPayload> Failure(IEnumerable<string> errors) =>
        new(false, payload: default, errors: errors.ToList());
}
