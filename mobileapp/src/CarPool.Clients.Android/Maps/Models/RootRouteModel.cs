using System.Collections.Generic;
using Newtonsoft.Json;

namespace CarPool.Clients.Droid.Maps.Gmaps.Models
{
    public class RootRouteModel
    {
        [JsonProperty("routes")]
        public List<RouteModel> Routes { get; set; }

        public string Status { get; set; }
    }
}