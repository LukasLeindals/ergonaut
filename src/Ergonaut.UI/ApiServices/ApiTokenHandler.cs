using System.Net.Http.Headers;
using Microsoft.Extensions.Options;

namespace Ergonaut.UI.ApiServices;

internal sealed class ApiTokenHandler : DelegatingHandler
{
    private readonly ApiOptions _options;
    private readonly SemaphoreSlim _lock = new(1, 1);
    private string? _token;
    private DateTimeOffset _expiresAt;

    public ApiTokenHandler(IOptions<ApiOptions> options)
    {
        _options = options.Value;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        await EnsureTokenAsync(cancellationToken);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", _token);
        return await base.SendAsync(request, cancellationToken);
    }

    private async Task EnsureTokenAsync(CancellationToken ct)
    {
        if (!NeedsRefresh()) return;

        await _lock.WaitAsync(ct);
        try
        {
            if (!NeedsRefresh()) return;

            if (string.IsNullOrWhiteSpace(_options.BaseUrl))
            {
                throw new InvalidOperationException("API base URL is not configured.");
            }

            var baseUri = new Uri(_options.BaseUrl);
            using var client = new HttpClient { BaseAddress = baseUri };
            var response = await client.PostAsJsonAsync(_options.Auth.Endpoint, new
            {
                username = _options.Auth.Username,
                password = _options.Auth.Password
            }, ct);

            response.EnsureSuccessStatusCode();

            var payload = await response.Content.ReadFromJsonAsync<TokenResponse>(cancellationToken: ct)
                          ?? throw new InvalidOperationException("Token payload missing.");

            _token = payload.AccessToken;
            _expiresAt = payload.ExpiresAt.AddSeconds(-30); // renew slightly early
        }
        finally
        {
            _lock.Release();
        }
    }

    private bool NeedsRefresh() =>
        string.IsNullOrEmpty(_token) || DateTimeOffset.UtcNow >= _expiresAt;

    private sealed record TokenResponse(string AccessToken, DateTimeOffset ExpiresAt);
}

internal sealed class ApiOptions
{
    public string BaseUrl { get; set; } = string.Empty;
    public AuthOptions Auth { get; set; } = new();

    public sealed class AuthOptions
    {
        public string Endpoint { get; set; } = "api/v1/auth/token";
        public string Username { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
