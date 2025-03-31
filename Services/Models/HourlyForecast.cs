using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DestinationsApp.Services.Models
{
    public class HourlyForecast
    {
        [JsonPropertyName("time")]
        public string Time { get; set; }

        [JsonPropertyName("temp_c")]
        public double TempC { get; set; }

        [JsonPropertyName("condition")]
        public Condition Condition { get; set; }

        [JsonPropertyName("chance_of_rain")]
        public int ChanceOfRain { get; set; }
    }

}
