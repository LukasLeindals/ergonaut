using Ergonaut.App.Logging;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.Core.Models.Logging;
using Microsoft.Extensions.DependencyInjection;

namespace Ergonaut.App.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IWorkItemService, WorkItemService>();

        services.AddLogIngestion();

        return services;
    }

    private static IServiceCollection AddLogIngestion(this IServiceCollection services)
    {
        services.AddSingleton<LogEventHub>();
        services.AddSingleton<ILogEventSink>(sp => sp.GetRequiredService<LogEventHub>());
        services.AddSingleton<ILogEventSource>(sp => sp.GetRequiredService<LogEventHub>());

        services.AddSingleton<OtlpLogIngestionService>();

        return services;
    }

}
