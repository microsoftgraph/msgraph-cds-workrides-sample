using Newtonsoft.Json;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Models.People
{
    public class RootGraphObject
    {
        [JsonProperty(PropertyName = "value")]
        public IList<GraphPeople> People { get; set; }
    }
}