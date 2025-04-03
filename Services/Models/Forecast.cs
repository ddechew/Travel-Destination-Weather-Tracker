namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

public class Forecast
{
    [JsonPropertyName("forecastday")]
    public List<ForecastDay> ForecastDays { get; set; }
}
