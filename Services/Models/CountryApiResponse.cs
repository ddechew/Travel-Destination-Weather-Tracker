namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

class CountryApiResponse
{
    [JsonPropertyName("error")]
    public bool Error { get; set; }

    [JsonPropertyName("msg")]
    public string Msg { get; set; }

    [JsonPropertyName("data")]
    public List<CountryData> Data { get; set; }
}
