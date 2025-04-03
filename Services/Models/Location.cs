namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

public class Location
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("country")]
    public string Country { get; set; }
}
