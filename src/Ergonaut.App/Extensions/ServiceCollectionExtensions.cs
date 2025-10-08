using Microsoft.Extensions.DependencyInjection;
using Ergonaut.App.Services;


namespace Ergonaut.App.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectScopedWorkItemService, ProjectScopedWorkItemService>();
        services.AddScoped<IWorkItemService>(sp => sp.GetRequiredService<IProjectScopedWorkItemService>());

        return services;
    }

}
