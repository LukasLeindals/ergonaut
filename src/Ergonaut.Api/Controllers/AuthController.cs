using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ergonaut.App.Auth;
using System.Collections.Concurrent;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthSettings _authSettings;
    private readonly ITokenService _tokenService;
    private static readonly ConcurrentDictionary<string, RefreshTokenEntry> RefreshTokens = new();

    public AuthController(IOptions<AuthSettings> authSettings, ITokenService tokenService)
    {
        _authSettings = authSettings.Value;
        _tokenService = tokenService;
    }

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> IssueToken([FromBody] TokenRequest request)
    {
        var credential = _authSettings.GetServiceCredential(request.Service);
        if (credential is null || string.IsNullOrWhiteSpace(credential.Token) || request.Token != credential.Token)
            return Unauthorized("Invalid service token.");

        var subject = request.Service;
        var claims = BuildClaims(subject, credential);
        var accessToken = _tokenService.CreateAccessToken(claims);
        var accessExpires = DateTime.UtcNow.AddMinutes(_authSettings.AccessTokenTtlMinutes);

        var refresh = _tokenService.CreateRefreshToken(subject);
        RefreshTokens[refresh.Token] = new RefreshTokenEntry(subject, refresh.ExpiresAtUtc);

        return Ok(new TokenResponse(accessToken, accessExpires, refresh.Token, refresh.ExpiresAtUtc));
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> Refresh([FromBody] RefreshTokenRequest request)
    {
        if (!RefreshTokens.TryGetValue(request.RefreshToken, out var stored))
            return Unauthorized("Invalid refresh token.");

        if (stored.ExpiresAtUtc <= DateTimeOffset.UtcNow)
        {
            RefreshTokens.TryRemove(request.RefreshToken, out _);
            return Unauthorized("Refresh token expired.");
        }

        // Rotate refresh token (single use)
        RefreshTokens.TryRemove(request.RefreshToken, out _);

        var credential = _authSettings.GetServiceCredential(stored.Subject);
        if (credential is null) return Unauthorized("Service credential missing.");

        var claims = BuildClaims(stored.Subject, credential);
        var accessToken = _tokenService.CreateAccessToken(claims);
        var accessExpires = DateTime.UtcNow.AddMinutes(_authSettings.AccessTokenTtlMinutes);

        var refresh = _tokenService.CreateRefreshToken(stored.Subject);
        RefreshTokens[refresh.Token] = new RefreshTokenEntry(stored.Subject, refresh.ExpiresAtUtc);

        return Ok(new TokenResponse(accessToken, accessExpires, refresh.Token, refresh.ExpiresAtUtc));
    }

    private static IEnumerable<Claim> BuildClaims(string subject, ServiceCredential credential)
    {
        var scopes = credential.Scopes ?? Array.Empty<string>();
        var claims = scopes.Select(s => new Claim("scope", s)).ToList();
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));
        return claims;
    }

    public sealed record TokenRequest(string Service, string Token);
    public sealed record RefreshTokenRequest(string RefreshToken);
    public sealed record TokenResponse(string AccessToken, DateTime ExpiresAt, string RefreshToken, DateTimeOffset RefreshExpiresAt);
    private sealed record RefreshTokenEntry(string Subject, DateTimeOffset ExpiresAtUtc);
}
