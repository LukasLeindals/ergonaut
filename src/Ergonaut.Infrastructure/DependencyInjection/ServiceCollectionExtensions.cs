using Ergonaut.Infrastructure.Data;
using Ergonaut.Infrastructure.Repositories;
using Microsoft.Data.Sqlite;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace Ergonaut.Infrastructure.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration, IHostEnvironment environment)
    {
        // Configure the EF Core DbContext using a resilient SQLite connection string.
        services.AddDbContext<ErgonautDbContext>(options =>
            options.UseSqlite(BuildConnectionString(configuration, environment)));

        // Register repository abstractions; keep the application layer unaware of EF specifics.
        services.AddScoped<IProjectRepository, LocalProjectRepository>();
        services.AddScoped<ITaskRepository, LocalTaskRepository>();
        return services;
    }

    private static string BuildConnectionString(IConfiguration configuration, IHostEnvironment environment)
    {
        var raw = configuration.GetConnectionString("Ergonaut")
                  ?? throw new InvalidOperationException("Connection string 'Ergonaut' not found.");

        var builder = new SqliteConnectionStringBuilder(raw);
        var dataSource = builder.DataSource;

        if (!Path.IsPathRooted(dataSource))
        {
            // Resolve relative paths using a shared data root so background workers can reuse the same DB file.
            var basePath = ResolveDataRoot(configuration, environment);
            var absolute = Path.GetFullPath(Path.Combine(basePath, dataSource));
            Directory.CreateDirectory(Path.GetDirectoryName(absolute)!);
            builder.DataSource = absolute;
        }
        else
        {
            // Ensure the absolute directory exists before EF tries to create/open the file.
            Directory.CreateDirectory(Path.GetDirectoryName(dataSource)!);
        }

        return builder.ConnectionString;
    }

    private static string ResolveDataRoot(IConfiguration configuration, IHostEnvironment environment)
    {
        // Prefer configuration binding; fall back to environment variables; default to the host content root.
        var fromConfig = configuration["Ergonaut:DataRoot"];
        var fromEnv = Environment.GetEnvironmentVariable("ERGONAUT_DATA_ROOT");

        var candidate = string.IsNullOrWhiteSpace(fromConfig) ? fromEnv : fromConfig;
        if (string.IsNullOrWhiteSpace(candidate))
        {
            return environment.ContentRootPath;
        }

        var fullPath = Path.GetFullPath(candidate);
        Directory.CreateDirectory(fullPath);
        return fullPath;
    }
}
