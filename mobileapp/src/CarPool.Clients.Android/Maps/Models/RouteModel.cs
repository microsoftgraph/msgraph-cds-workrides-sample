using Newtonsoft.Json;
using System.Collections.Generic;

namespace CarPool.Clients.Droid.Maps.Gmaps.Models
{
    public class RouteModel
    {
        public string Summary { get; set; }

        [JsonProperty("legs")]
        public List<RouteStepModel> Steps { get; set; }

        [JsonProperty("overview_polyline")]
        public RoutePolylineModel Polyline { get; set; }
    }
}