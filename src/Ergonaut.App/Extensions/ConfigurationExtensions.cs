using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ergonaut.App.Extensions;

public static class ConfigurationExtensions
{

    public static IConfigurationManager AddConfigurationSources(this IConfigurationManager config, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(environment);

        var configRoot = Path.Combine(environment.ContentRootPath, "..", "..", "config");
        var sharedConfig = Path.Combine(configRoot, "appsettings.json");
        var sharedEnvironmentConfig = Path.Combine(configRoot, $"appsettings.{environment.EnvironmentName}.json");

        config.AddJsonFile(sharedConfig, optional: false, reloadOnChange: true)
            .AddJsonFile(sharedEnvironmentConfig, optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true);

        // Ensure user-secrets still override the shared config when running locally.
        if (environment.IsDevelopment())
        {
            // Use the entry assembly (the executable project) to locate the correct secrets id.
            var entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly is not null)
            {
                config.AddUserSecrets(entryAssembly, optional: true);
            }
        }

        // Environment variables can still override user-secrets when explicitly set.
        config.AddEnvironmentVariables();

        return config;
    }

    
}
