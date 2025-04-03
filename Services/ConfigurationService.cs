using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace DestinationsApp.Services;

public static class ConfigurationService
{
    private static IConfigurationRoot _configuration;

    static ConfigurationService()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "DestinationsApp.appsettings.json"; 

        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream == null)
            throw new FileNotFoundException($"Embedded resource '{resourceName}' not found.");

        _configuration = new ConfigurationBuilder()
            .AddJsonStream(stream)
            .Build();
    }

    public static string GetAmadeusValue(string key) =>
        _configuration[$"Amadeus:{key}"];

    public static string GetWeatherValue(string key) =>
        _configuration[$"Weather:{key}"];
}
