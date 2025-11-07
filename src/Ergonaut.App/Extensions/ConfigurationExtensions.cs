using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

namespace Ergonaut.App.Extensions;

public static class ConfigurationExtensions
{

    public static IConfigurationManager AddConfigurationSources(this IConfigurationManager config, IHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(config);
        ArgumentNullException.ThrowIfNull(environment);

        var mainConfig = Path.Combine(environment.ContentRootPath, "..", "..", "config", "appsettings.json");
        config.AddJsonFile(mainConfig, optional: false, reloadOnChange: true)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true)
            .AddEnvironmentVariables();

        return config;
    }

    
}
