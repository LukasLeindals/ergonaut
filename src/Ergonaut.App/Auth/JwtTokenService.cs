using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Ergonaut.App.Auth;

public sealed class JwtTokenService : ITokenService
{
    private readonly AuthSettings _settings;
    private readonly JwtSecurityTokenHandler _handler = new();

    public JwtTokenService(IOptions<AuthSettings> options)
    {
        _settings = options.Value;
    }

    public string CreateAccessToken(IEnumerable<Claim> claims, int? ttlMinutes = null)
    {
        var signingKey = _settings.GetSigningKey();
        var now = DateTime.UtcNow;
        var expires = now.AddMinutes(ttlMinutes ?? _settings.AccessTokenTtlMinutes);

        var claimList = claims.ToList();
        claimList.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString("N")));
        claimList.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString(), ClaimValueTypes.Integer64));

        var token = new JwtSecurityToken(
            issuer: _settings.Issuer,
            audience: _settings.Audience,
            claims: claimList,
            notBefore: now,
            expires: expires,
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return _handler.WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(string subject)
    {
        Span<byte> bytes = stackalloc byte[32];
        RandomNumberGenerator.Fill(bytes);
        var token = Convert.ToBase64String(bytes);
        var expires = DateTimeOffset.UtcNow.AddDays(_settings.RefreshTokenTtlDays);
        return new RefreshToken(token, expires);
    }

}
