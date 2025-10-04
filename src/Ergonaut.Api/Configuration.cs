namespace Ergonaut.Api.Configuration;

public sealed record JwtOptions(string Issuer, string Audience, string Secret);
