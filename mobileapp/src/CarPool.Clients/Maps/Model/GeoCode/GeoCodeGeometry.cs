using Newtonsoft.Json;

namespace CarPool.Clients.Core.Maps.Model.GeoCode
{
    public class GeoCodeGeometry
    {
        [JsonProperty("location")]
        public GeoCodeLocation Location { get; set; }
    }
}