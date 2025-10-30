using Ergonaut.App.LogIngestion;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.LogIngestion.PayloadParser;
using Ergonaut.App.Sentinel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

    public static IServiceCollection AddLogIngestion(this IServiceCollection services)
    {
        services.TryAddSingleton<LogEventHub>();
        services.TryAddSingleton<ILogEventSink>(sp => sp.GetRequiredService<LogEventHub>());
        services.TryAddSingleton<ILogEventSource>(sp => sp.GetRequiredService<LogEventHub>());

        services.AddSingleton<IPayloadParser<OpenTelemetry.Proto.Collector.Logs.V1.ExportLogsServiceRequest>, OtlpLogPayloadParser>();
        services.AddScoped<ILogIngestionPipeline, OtlpLogIngestionPipeline>();

        return services;
    }

    public static IServiceCollection AddSentinel(this IServiceCollection services)
    {

        // Log ingestion
        services.AddLogIngestion();

        // Sentinel specific services
        services.AddSingleton<ILogEventFilter, SentinelLogEventFilter>();

        return services;
    }

}
