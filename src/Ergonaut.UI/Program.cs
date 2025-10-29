using Ergonaut.UI.Components;
using Ergonaut.UI.ApiServices;
using Ergonaut.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
var mainConfig = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "config", "appsettings.json");
builder.Configuration.AddJsonFile(mainConfig, optional: false, reloadOnChange: true)
    .AddJsonFile("appsettings.json", optional: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));


builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddErgonautApiServices();


var app = builder.Build();


app.ConfigureHttpRequestPipeline();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
