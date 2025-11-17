namespace Ergonaut.App.Auth;

public sealed record RefreshToken(string Token, DateTimeOffset ExpiresAtUtc);
