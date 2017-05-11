using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Services.GeoCoder
{
    class XamarinGeoCoder : IGeoCoder
    {
        /// <summary>
        /// https://developer.xamarin.com/recipes/cross-platform/xamarin-forms/maps/geocode
        /// </summary>
        /// <param name="address">Address to geocode</param>
        /// <returns>List of geolocation approximations</returns>
        public Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address)
        {
            var geocoder = new Geocoder();
            return geocoder.GetPositionsForAddressAsync(address);
        }
    }
}
