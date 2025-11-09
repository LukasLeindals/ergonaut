using Microsoft.Extensions.Hosting;

namespace Ergonaut.UI.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureHttpRequestPipeline(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseHttpsRedirection();
            return app;
        }

        app.UseExceptionHandler("/Error", createScopeForErrors: true);
        app.UseHsts();
        app.UseHttpsRedirection();

        return app;
    }
}
