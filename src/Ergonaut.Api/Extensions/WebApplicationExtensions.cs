using Microsoft.Extensions.Hosting;

namespace Ergonaut.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRedirection(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            return app;
        }

        app.UseHttpsRedirection();

        if (app.Environment.IsStaging())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        return app;
    }
}
