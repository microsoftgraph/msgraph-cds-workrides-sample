using Android.Gms.Maps.Model;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Droid.Maps.Helpers
{
    public static class CoordinateConverter
    {
        public static LatLng ConvertToNative(Position position)
        {
            return new LatLng(position.Latitude, position.Longitude);
        }

        public static Position ConvertToAbstraction(LatLng position)
        {
            return new Position(position.Latitude,
                                position.Longitude);
        }
    }
}