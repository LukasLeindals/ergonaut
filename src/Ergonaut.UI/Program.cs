using Ergonaut.UI.Components;
using Ergonaut.UI.ApiServices;
using Ergonaut.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

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
