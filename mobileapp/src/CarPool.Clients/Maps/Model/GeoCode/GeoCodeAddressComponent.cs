using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Maps.Model.GeoCode
{
    public class GeoCodeAddressComponent
    {
        [JsonProperty("long_name")]
        public string LongName { get; set; }

        [JsonProperty("short_name")]
        public string ShortName { get; set; }

        [JsonProperty("types")]
        public IEnumerable<string> Types { get; set; }
    }
}