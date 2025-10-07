using System.Text;
using Microsoft.Extensions.Options;
using Ergonaut.App.Features.Projects;
using Ergonaut.UI.Features.Auth;
using Ergonaut.UI.Features.Projects;
using Ergonaut.UI.Features.WorkItems;

using Ergonaut.App.Features.WorkItems;

namespace Ergonaut.UI.Extensions;


public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddPolicies(this IServiceCollection services)
    {
        services.AddAuthorization(options =>
        {
            options.AddPolicy("ProjectsRead", policy => policy.RequireClaim("scope", "projects:read"));
            options.AddPolicy("ProjectsWrite", policy => policy.RequireClaim("scope", "projects:write"));
            options.AddPolicy("WorkItemsRead", policy => policy.RequireClaim("scope", "workitems:read"));
            options.AddPolicy("WorkItemsWrite", policy => policy.RequireClaim("scope", "workitems:write"));
        });

        return services;
    }


    private static void SetBaseUrl(IServiceProvider sp, HttpClient client)
    {
        var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            throw new InvalidOperationException("API base URL is not configured.");
        }

        client.BaseAddress = new Uri(options.BaseUrl);
    }

    private static void ConfigureApiClient(IServiceProvider sp, HttpClient client)
    {
        SetBaseUrl(sp, client);
    }


    public static IServiceCollection AddApiServices(this IServiceCollection services)
    {

        services.AddHttpClient<IProjectService, ApiProjectService>(ConfigureApiClient).AddHttpMessageHandler<ApiTokenHandler>();
        services.AddHttpClient<IProjectScopedWorkItemService, ApiWorkItemService>(ConfigureApiClient).AddHttpMessageHandler<ApiTokenHandler>();

        return services;

    }

}
