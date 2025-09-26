using Ergonaut.Infrastructure.Data;
using Ergonaut.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;



namespace Ergonaut.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<ErgonautDbContext>(options =>
            options.UseSqlite(BuildConnectionString(configuration)));

        services.AddScoped<IProjectRepository, ProjectRepository>();
        return services;
    }

    private static string BuildConnectionString(IConfiguration configuration)
    {
        var raw = configuration.GetConnectionString("Ergonaut") ?? throw new InvalidOperationException("Connection string 'Ergonaut' not found.");

        var builder = new SqliteConnectionStringBuilder(raw);
        var dataSource = builder.DataSource;

        if (!Path.IsPathRooted(dataSource))
        {
            var absolute = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, dataSource));
            Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
            builder.DataSource = absolute;
        }
        else
        {
            Directory.CreateDirectory(Path.GetDirectoryName(dataSource)!);
        }

        return builder.ConnectionString;
    }
}
