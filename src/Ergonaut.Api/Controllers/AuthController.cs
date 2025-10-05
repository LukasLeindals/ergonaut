using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Ergonaut.Api.Configuration;

namespace Ergonaut.Api.Controllers;

[ApiController]
[Route("api/v1/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly JwtOptions _jwtOptions;

    public AuthController(JwtOptions jwtOptions) => _jwtOptions = jwtOptions;

    [HttpPost("token")]
    [AllowAnonymous]
    public ActionResult<TokenResponse> IssueToken([FromBody] TokenRequest request)
    {
        if (request.Username != "dev" || request.Password != "dev")
            return Unauthorized();

        var now = DateTime.UtcNow;
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Username),
            new Claim("scope", "projects:read"),
            new Claim("scope", "projects:write"),
            new Claim("scope", "tasks:read"),
            new Claim("scope", "tasks:write"),
        };

        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.Secret));
        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(30),
            signingCredentials: new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256));

        return Ok(new TokenResponse(new JwtSecurityTokenHandler().WriteToken(jwt), now.AddMinutes(30)));
    }

    public sealed record TokenRequest(string Username, string Password);
    public sealed record TokenResponse(string AccessToken, DateTime ExpiresAt);
}
