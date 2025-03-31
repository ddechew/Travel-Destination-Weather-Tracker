using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DestinationsApp.Services.Models
{
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
}
