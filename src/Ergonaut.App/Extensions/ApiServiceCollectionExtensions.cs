
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.App.Services.ApiScoped;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Configuration;


namespace Ergonaut.App.Extensions;

public static class ApiServiceCollectionExtensions
{

    private static void ConfigureErgonautApiClient(IServiceProvider sp, HttpClient client)
    {
        var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;

        // Set the base URL for the API client
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            throw new InvalidOperationException("API base URL is not configured.");
        }

        client.BaseAddress = new Uri(options.BaseUrl);
    }
    public static IServiceCollection AddErgonautApiServices(this IServiceCollection services)
    {
        services.Configure<ApiOptions>(services.BuildServiceProvider().GetRequiredService<IConfiguration>().GetSection("Api"));

        services.AddTransient<ApiTokenHandler>();
        services.AddHttpClient<IProjectService, ApiProjectService>(ConfigureErgonautApiClient).AddHttpMessageHandler<ApiTokenHandler>();
        services.AddHttpClient<IWorkItemService, ApiWorkItemService>(ConfigureErgonautApiClient).AddHttpMessageHandler<ApiTokenHandler>();

        return services;

    }

}
