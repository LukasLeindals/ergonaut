using Microsoft.Extensions.DependencyInjection;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;

namespace Ergonaut.App.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IWorkItemService, WorkItemService>();

        return services;
    }

}
