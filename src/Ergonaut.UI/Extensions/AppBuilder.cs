

namespace Ergonaut.UI.Extensions;

public static class AppBuilderExtensions
{
    public static WebApplication ConfigureHttpRequestPipeline(this WebApplication app)
    {
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
        return app;
    }
}
