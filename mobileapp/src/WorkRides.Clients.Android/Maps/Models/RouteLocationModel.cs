using Newtonsoft.Json;

namespace CarPool.Clients.Droid.Maps.Gmaps.Models
{
    public class RouteLocationModel
    {
        [JsonProperty("lat")]
        public double Latitude { get; set; }

        [JsonProperty("lng")]
        public double Longitude { get; set; }
    }
}