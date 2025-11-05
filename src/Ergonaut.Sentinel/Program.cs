using Ergonaut.Sentinel;
using Ergonaut.App.Extensions;
using Ergonaut.App.Sentinel;
using Ergonaut.Sentinel.Services;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration sources
var mainConfig = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "config", "appsettings.json");
builder.Configuration.AddJsonFile(mainConfig, optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
builder.Services.AddScoped<ISentinelProjectService, SentinelProjectService>();
builder.Services.AddScoped<ISentinelWorkItemService, SentinelWorkItemService>();
builder.Services.AddSentinel();
builder.Services.AddHostedService<Worker>();

var host = builder.Build();
await host.RunAsync();
