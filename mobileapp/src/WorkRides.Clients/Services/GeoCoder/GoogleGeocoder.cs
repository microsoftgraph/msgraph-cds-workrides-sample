using Carpool.Clients.Services.Data;
using CarPool.Clients.Core.Maps.Model.GeoCode;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Services.GeoCoder
{
    public class GoogleGeoCoder : IGeoCoder
    {
        public async Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address)
        {
            if (!string.IsNullOrEmpty(address))
            {
                // Origin of route
                string addressQueryParam = $"address={WebUtility.UrlEncode(address)}";

                // Auth
                string key = $"key={AppSettings.GoogleMapsGeocodeAPIKey}";

                GeoCodeResult geoCodeData = default(GeoCodeResult);
                try
                {
                    // Building the parameters to the web service
                    string parameters = string.Join("&", new[] { addressQueryParam, key });

                    UriBuilder uri = new UriBuilder("https://maps.googleapis.com/maps/api/geocode/json");
                    uri.Query = parameters;

                    var requestProvider = new RequestProvider();
                    geoCodeData = await requestProvider.GetAsync<GeoCodeResult>(uri.ToString());
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error calling routes api from google: {ex}");
                    return null;
                }

                if ("OK".Equals(geoCodeData?.Status, StringComparison.CurrentCultureIgnoreCase) &&
                    geoCodeData?.Results != null &&
                    geoCodeData.Results.Any())
                {
                    Collection<Position> result = new Collection<Position>();

                    foreach (var geoCodePosition in geoCodeData.Results)
                    {
                        if (geoCodePosition?.Geometry?.Location != null)
                        {
                            result.Add(new Position(
                                geoCodePosition.Geometry.Location.Latitude,
                                geoCodePosition.Geometry.Location.Longitude));
                        }
                    }

                    return result;
                }
            }

            return null;
        }
    }
}
