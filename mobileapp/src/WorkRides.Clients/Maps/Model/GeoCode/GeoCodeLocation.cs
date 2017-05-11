using Newtonsoft.Json;

namespace CarPool.Clients.Core.Maps.Model.GeoCode
{
    public class GeoCodeLocation
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}