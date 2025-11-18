using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Ergonaut.App.Auth;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthSettings _authSettings;
    private readonly ITokenService _tokenService;

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
        var accessToken = _tokenService.CreateAccessToken(claims, _authSettings.ServiceTokenTtlMinutes);
        var accessExpires = DateTime.UtcNow.AddMinutes(_authSettings.ServiceTokenTtlMinutes);

        return Ok(new TokenResponse(accessToken, accessExpires));
    }

    private static IEnumerable<Claim> BuildClaims(string subject, ServiceCredential credential)
    {
        var scopes = credential.Scopes ?? Array.Empty<string>();
        var claims = scopes.Select(s => new Claim("scope", s)).ToList();
        claims.Add(new Claim(JwtRegisteredClaimNames.Sub, subject));
        return claims;
    }

    public sealed record TokenRequest(string Service, string Token);
    public sealed record TokenResponse(string AccessToken, DateTime ExpiresAt);
}
