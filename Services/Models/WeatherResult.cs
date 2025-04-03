namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

public class WeatherResult
{
    [JsonPropertyName("location")]
    public Location Location { get; set; }

    [JsonPropertyName("current")]
    public CurrentWeather Current { get; set; }

    [JsonPropertyName("forecast")]
    public Forecast Forecast { get; set; }
}
