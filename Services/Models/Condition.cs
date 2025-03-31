using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DestinationsApp.Services.Models
{
    public class Condition
    {
        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("icon")]
        public string Icon { get; set; }

        [JsonIgnore]
        public string FullIcon => $"https:{Icon}";
    }
}
