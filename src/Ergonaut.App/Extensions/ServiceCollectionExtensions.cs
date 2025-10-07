using Microsoft.Extensions.DependencyInjection;
using Ergonaut.App.Features.Projects;
using Ergonaut.App.Features.WorkItems;


namespace Ergonaut.App.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddAppServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IProjectFactory, ProjectFactory>();
        services.AddScoped<IProjectScopedWorkItemService, ProjectScopedWorkItemService>();
        services.AddScoped<IWorkItemService>(sp => sp.GetRequiredService<IProjectScopedWorkItemService>());

        return services;
    }

}
