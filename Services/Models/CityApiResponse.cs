namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

class CityApiResponse
{
    [JsonPropertyName("error")]
    public bool Error { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("data")]
    public List<string> Cities { get; set; }
}
