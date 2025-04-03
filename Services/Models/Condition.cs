namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

public class Condition
{
    [JsonPropertyName("text")]
    public string Text { get; set; }

    [JsonPropertyName("icon")]
    public string Icon { get; set; }

    [JsonIgnore]
    public string FullIcon => $"https:{Icon}";
}
