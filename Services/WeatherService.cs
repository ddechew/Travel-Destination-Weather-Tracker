namespace DestinationsApp.Services;

using DestinationsApp.Services.Models;

using System.Text.Json;

public class WeatherService
{
    private readonly string ApiKey;
    private readonly HttpClient _httpClient = new();

    public WeatherService()
    {
        ApiKey = ConfigurationService.GetWeatherValue("API_KEY");
    }

    public async Task<WeatherResult?> GetWeatherAsync(string city)
    {
        var url = $"https://api.weatherapi.com/v1/forecast.json?key={ApiKey}&q={city}&days=7&aqi=no&alerts=no";
        var response = await _httpClient.GetAsync(url);

        if (!response.IsSuccessStatusCode)
            return null;

        var json = await response.Content.ReadAsStringAsync();
        var result = JsonSerializer.Deserialize<WeatherResult>(json, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        return result;
    }
}
