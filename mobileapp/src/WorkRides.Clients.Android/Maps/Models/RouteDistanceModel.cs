using Newtonsoft.Json;

namespace CarPool.Clients.Droid.Maps.Gmaps.Models
{
    public class RouteDistanceModel
    {
        [JsonProperty("text")]
        public string Text { get; set; }

        [JsonProperty("value")]
        public double Value { get; set; }
    }
}