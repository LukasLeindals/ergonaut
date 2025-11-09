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
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        return config;
    }

    
}
