using Ergonaut.Sentinel;
using Ergonaut.App.Extensions;
using Ergonaut.Infrastructure.Extensions;

var builder = Host.CreateApplicationBuilder(args);

// Add configuration sources
var mainConfig = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "config", "appsettings.json");
builder.Configuration.AddJsonFile(mainConfig, optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
builder.Services.AddSentinel();
builder.Services.AddHostedService<Worker>();
builder.Services.AddInfrastructure(builder.Configuration, builder.Environment);

var host = builder.Build();
await host.RunAsync();
