using Ergonaut.Sentinel;
using Ergonaut.App.Extensions;
using Ergonaut.Sentinel.Startup;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration sources
builder.Configuration.AddConfigurationSources(builder.Environment);

// Add services to the container.
builder.Services.AddErgonautApiServices();
builder.Services.AddSentinel();
builder.Services.AddHostedService<Worker>();
builder.Services.AddHostedService<KafkaTopicInitializerHostedService>();

var host = builder.Build();
await host.RunAsync();
