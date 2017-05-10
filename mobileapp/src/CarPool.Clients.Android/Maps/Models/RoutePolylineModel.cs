using Newtonsoft.Json;

namespace CarPool.Clients.Droid.Maps.Gmaps.Models
{
    public class RoutePolylineModel
    {
        [JsonProperty("points")]
        public string Points { get; set; }
    }
}