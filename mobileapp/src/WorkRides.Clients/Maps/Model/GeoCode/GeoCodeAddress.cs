using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Maps.Model.GeoCode
{
    public class GeoCodeAddress
    {
        [JsonProperty("formatted_address")]
        public string FormattedAddress { get; set; }

        [JsonProperty("place_id")]
        public string PlaceId { get; set; }

        [JsonProperty("address_components")]
        public IEnumerable<GeoCodeAddressComponent> AddressComponents { get; set; }

        [JsonProperty("geometry")]
        public GeoCodeGeometry Geometry {get;set;}
    }
}