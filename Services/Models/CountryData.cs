using DestinationsApp.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace DestinationsApp.Services.Models
{
    class CountryData
    {

        [JsonPropertyName("country")]
        public string Country { get; set; }

        [JsonPropertyName("iso2")]
        public string iso2 { get; set; }

        [JsonIgnore]
        public string Flag => HelperMethods.GetFlagEmoji(this.iso2);

        public override string ToString()
        {
            return $"{Country}";
        }
    }
}
