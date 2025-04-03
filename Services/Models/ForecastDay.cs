namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

public class ForecastDay
{
    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("day")]
    public DayInfo Day { get; set; }

    [JsonPropertyName("hour")]
    public List<HourlyForecast> Hour { get; set; } 

    [JsonIgnore]
    public string DisplayDate { get; set; }
}
