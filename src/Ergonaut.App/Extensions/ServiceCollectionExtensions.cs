using Ergonaut.App.LogIngestion;
using Ergonaut.App.Services;
using Ergonaut.App.Services.ProjectScoped;
using Ergonaut.Core.EventIngestion;
using Ergonaut.Core.LogIngestion;
using Ergonaut.Core.LogIngestion.PayloadParser;
using Ergonaut.App.Sentinel;
using Ergonaut.App.Auth;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;


namespace Ergonaut.App.Extensions;

public static class ServiceCollectionExtensions
{

    public static IServiceCollection AddErgonautAuth(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions<AuthSettings>()
            .Bind(configuration.GetSection("Auth"))
            .ValidateDataAnnotations()
            .Validate(s => !string.IsNullOrWhiteSpace(s.SigningKey) || !string.IsNullOrWhiteSpace(s.SigningKeyPath), "Need a signing key")
            .ValidateOnStart();

        return services;
    }
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddScoped<IProjectService, ProjectService>();
        services.AddScoped<IWorkItemService, WorkItemService>();

        return services;
    }

    public static IServiceCollection AddLogIngestion(this IServiceCollection services)
    {
        services.AddOptions<KafkaLogEventOptions>().BindConfiguration("LogIngestion:Kafka").ValidateDataAnnotations().ValidateOnStart();
        services.AddOptions<LogIngestionOptions>().BindConfiguration("LogIngestion").ValidateDataAnnotations().ValidateOnStart();
        services.AddSingleton<IEventProducer<ILogEvent>, KafkaLogEventProducer>();
        services.AddSingleton<IEventConsumer<ILogEvent>, KafkaLogEventConsumer>();

        services.AddSingleton<IPayloadParser<OpenTelemetry.Proto.Collector.Logs.V1.ExportLogsServiceRequest>, OtlpLogPayloadParser>();
        services.AddScoped<ILogIngestionPipeline, OtlpLogIngestionPipeline>();

        return services;
    }

    public static IServiceCollection AddSentinel(this IServiceCollection services)
    {

        // Log ingestion
        services.AddLogIngestion();

        // Sentinel configuration
        services.AddOptions<SentinelConfig>().BindConfiguration("Sentinel").ValidateDataAnnotations().ValidateOnStart();

        // Sentinel specific services
        services.AddSingleton<ILogEventFilter, SentinelLogEventFilter>();
        services.AddScoped<IWorkItemCreator, WorkItemCreator>();

        return services;
    }

}
