using CoreLocation;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.iOS.Maps.Helpers
{
    public static class CoordinateConverter
    {
        public static CLLocationCoordinate2D ConvertToNative(Position position)
        {
            return new CLLocationCoordinate2D()
            {
                Latitude = position.Latitude,
                Longitude = position.Longitude
            };
        }

        public static Position ConvertToAbstraction(CLLocationCoordinate2D position)
        {
            return new Position(position.Latitude, position.Longitude);
        }
    }
}