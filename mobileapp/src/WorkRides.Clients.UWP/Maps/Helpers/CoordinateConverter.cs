using Windows.Devices.Geolocation;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.UWP.Maps.Helpers
{
    public static class CoordinateConverter
    {
        public static Geopoint ConvertToNative(Position position)
        {
            return new Geopoint(new BasicGeoposition
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude
            });
        }

        public static Position ConvertToAbstraction(Geopoint position)
        {
            return new Position(position.Position.Latitude, position.Position.Longitude);
        }
    }
}