using Ergonaut.App.Services.ApiScoped;
using Ergonaut.UI.Components;
using Ergonaut.UI.Extensions;
using Ergonaut.App.Extensions;
using Microsoft.AspNetCore.DataProtection;

var builder = WebApplication.CreateBuilder(args);

// Add configuration sources
builder.Configuration.AddConfigurationSources(builder.Environment);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents(options =>
    {
        options.DetailedErrors = true;
    });




builder.Services.AddErgonautApiServices("Ergonaut.UI");


var keysPath = Path.Combine(builder.Environment.ContentRootPath, "..", "..", "data-protection");
Directory.CreateDirectory(keysPath);
builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo(keysPath))
    .SetApplicationName("ergonaut-ui");


var app = builder.Build();


app.ConfigureHttpRequestPipeline();
app.UseAntiforgery();



app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

await app.RunAsync();
