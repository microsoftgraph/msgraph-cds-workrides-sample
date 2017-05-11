using CarPool.Clients.Core.Services.GeoCoder;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Helpers
{
    public static class MapHelper
    {
        
        public static Task<IEnumerable<Position>> GeoCodeAsync(string address)
        {
            IGeoCoder geocoder = new GoogleGeoCoder();
            return geocoder.GetPositionsForAddressAsync(address);
        }

        public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2, DistanceMeasure unit)
        {
            double theta = lon1 - lon2;
            double dist = Math.Sin(Deg2Rad(lat1)) * Math.Sin(Deg2Rad(lat2)) + Math.Cos(Deg2Rad(lat1)) * Math.Cos(Deg2Rad(lat2)) * Math.Cos(Deg2Rad(theta));
            dist = Math.Acos(dist);
            dist = Rad2Deg(dist);
            dist = dist * 60 * 1.1515;
            if (unit == DistanceMeasure.Kilometers)
            {
                dist = dist * 1.609344;
            }
            else if (unit == DistanceMeasure.Miles)
            {
                dist = dist * 0.8684;
            }
            return (dist);
        }

        public static double ConvertMetersToMiles(double meters)
        {
            return (meters / 1609.344);
        }

        private static double Deg2Rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        private static double Rad2Deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
    }

    public enum DistanceMeasure
    {
        Kilometers,
        Miles
    }
}