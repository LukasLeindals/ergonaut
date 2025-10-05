using Ergonaut.UI.Components;
using Ergonaut.Infrastructure.DependencyInjection;
using Ergonaut.App.Features.Projects;
using Ergonaut.UI.Features.Auth;
using Ergonaut.UI.Features.Projects;
using Ergonaut.UI.Features.Tasks;
using Microsoft.Extensions.Options;
using Ergonaut.App.Features.Tasks;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.Configure<ApiOptions>(builder.Configuration.GetSection("Api"));


builder.Services.AddTransient<ApiTokenHandler>();
builder.Services.AddHttpClient<IProjectService, ApiProjectService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        throw new InvalidOperationException("API base URL is not configured.");
    }

    client.BaseAddress = new Uri(options.BaseUrl);
}).AddHttpMessageHandler<ApiTokenHandler>();
builder.Services.AddHttpClient<ITaskService, ApiTaskService>((sp, client) =>
{
    var options = sp.GetRequiredService<IOptions<ApiOptions>>().Value;
    if (string.IsNullOrWhiteSpace(options.BaseUrl))
    {
        throw new InvalidOperationException("API base URL is not configured.");
    }

    client.BaseAddress = new Uri(options.BaseUrl);
}).AddHttpMessageHandler<ApiTokenHandler>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseHttpsRedirection();
}



app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
