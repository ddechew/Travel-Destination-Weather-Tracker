﻿namespace DestinationsApp.Services.Models;

using System.Text.Json.Serialization;

public class DayInfo
{
    [JsonPropertyName("maxtemp_c")]
    public double MaxTempC { get; set; }

    [JsonPropertyName("mintemp_c")]
    public double MinTempC { get; set; }

    [JsonPropertyName("avgtemp_c")]
    public double AvgTempC { get; set; }

    [JsonPropertyName("condition")]
    public Condition Condition { get; set; }

    [JsonPropertyName("daily_chance_of_rain")]
    public int ChanceOfRain { get; set; }

}
