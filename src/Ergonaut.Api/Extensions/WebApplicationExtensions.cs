

namespace Ergonaut.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication ConfigureRedirection(this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        else
        {
            app.UseHttpsRedirection();
        }
        return app;
    }
}
