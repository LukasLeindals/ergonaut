namespace Ergonaut.App.Services.ApiScoped;

public sealed class ApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public AuthOptions Auth { get; set; } = new();

    public sealed class AuthOptions
    {
        public string Endpoint { get; set; } = "api/v1/auth/token";
        public string ServiceToken { get; set; } = string.Empty;
    }
}
