namespace Ergonaut.App.Services.ApiScoped;

public sealed class ApiServiceIdentity
{
    public string Service { get; }

    public ApiServiceIdentity(string service)
    {
        if (string.IsNullOrWhiteSpace(service))
            throw new ArgumentException("Service name must be provided.", nameof(service));

        Service = service;
    }
}
