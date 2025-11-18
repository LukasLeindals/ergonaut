using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Ergonaut.App.Auth;

public sealed record ServiceCredential(string? Token, string[]? Scopes);

public class AuthSettings
{
    [Required] public string Issuer { get; set; } = string.Empty;
    [Required] public string Audience { get; set; } = string.Empty;
    public string? SigningKey { get; set; }
    public string? SigningKeyPath { get; set; } // Path to PEM-encoded RSA private key.
    [Range(1, 1440)] public int AccessTokenTtlMinutes { get; set; } = 30;
    [Range(1, 1440)] public int ServiceTokenTtlMinutes { get; set; } = 1440; // ~24h
    // Keyed by service name (e.g., "sentinel", "log-emitter")
    public Dictionary<string, ServiceCredential> ServiceCredentials { get; set; } = new();

    public SymmetricSecurityKey GetSigningKey()
    {
        var signingKey = SigningKey;
        if (string.IsNullOrEmpty(signingKey) && !string.IsNullOrEmpty(SigningKeyPath))
            signingKey = File.ReadAllText(SigningKeyPath);

        if (string.IsNullOrEmpty(signingKey))
            throw new InvalidOperationException("Missing signing key for JWT authentication");

        return new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signingKey));
    }

    public ServiceCredential? GetServiceCredential(string service)
    {
        if (string.IsNullOrWhiteSpace(service)) return null;
        return ServiceCredentials.GetValueOrDefault(service);
    }
}
