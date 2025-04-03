namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

class HourWeather
{
    [JsonPropertyName("time")]
    public string Time { get; set; }

    [JsonPropertyName("temp_c")]
    public double TempC { get; set; }

    [JsonPropertyName("condition")]
    public Condition Condition { get; set; }
}
