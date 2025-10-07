using Ergonaut.UI.Components;
using Ergonaut.App.Features.Projects;
using Ergonaut.UI.Features.Auth;
using Ergonaut.UI.Features.Projects;
using Ergonaut.UI.Features.WorkItems;
using Microsoft.Extensions.Options;
using Ergonaut.App.Features.WorkItems;
using Ergonaut.UI.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));


builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddApiServices();


var app = builder.Build();


app.ConfigureHttpRequestPipeline();
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
