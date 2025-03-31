using Microsoft.Extensions.Configuration;

namespace DestinationsApp.Services;

public static class ConfigurationService
{
    private static IConfigurationRoot _configuration;

    static ConfigurationService()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

        _configuration = builder.Build();
    }

    public static string GetAmadeusValue(string key)
    {
        return _configuration[$"Amadeus:{key}"];
    }

    public static string GetWeatherValue(string key)
    {
        return _configuration[$"Weather:{key}"];
    }
}
