using System.Text.Json.Serialization;
using DestinationsApp.Services.Models;

public class CurrentWeather
{
    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("condition")]
    public DestinationsApp.Services.Models.Condition Condition { get; set; }

    [JsonPropertyName("humidity")]
    public int Humidity { get; set; }

    [JsonPropertyName("wind_kph")]
    public double WindKph { get; set; }
}