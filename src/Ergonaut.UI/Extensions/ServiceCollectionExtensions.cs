using Microsoft.Extensions.Options;
using Ergonaut.UI.ApiServices;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;

namespace Ergonaut.UI.Extensions;


public static class ServiceCollectionExtensions
{
    private static void SetErgonautBaseUrl(IServiceProvider sp, HttpClient client)
    {
        var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            throw new InvalidOperationException("API base URL is not configured.");
        }

        client.BaseAddress = new Uri(options.BaseUrl);
    }

    private static void ConfigureErgonautApiClient(IServiceProvider sp, HttpClient client)
    {
        SetErgonautBaseUrl(sp, client);
    }


    public static IServiceCollection AddErgonautApiServices(this IServiceCollection services)
    {

        services.AddHttpClient<IProjectService, ApiProjectService>(ConfigureErgonautApiClient).AddHttpMessageHandler<ApiTokenHandler>();
        services.AddHttpClient<IWorkItemService, ApiWorkItemService>(ConfigureErgonautApiClient).AddHttpMessageHandler<ApiTokenHandler>();

        return services;

    }

}
