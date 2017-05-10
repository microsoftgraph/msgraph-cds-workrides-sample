using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Carpool.Clients.Services.Data;
using CarPool.Clients.Droid.Maps.Gmaps.Models;
using CarPool.Clients.Droid.Maps.Helpers;
using CarPool.Clients.Droid.Maps.Pushpins;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Routes;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.Android;

namespace CarPool.Clients.Droid.Maps.Routes
{
    public class RouteManager : AbstractRouteManager
    {
        private readonly GoogleMap _nativeMap;
        private Polyline _currentUserRoute;

        public RouteManager(GoogleMap nativeMap, CustomMap formsMap, MarkerManager pushpinManager) 
            : base(formsMap, pushpinManager)
        {
            _nativeMap = nativeMap;
        }

        public override async Task<IEnumerable<Position>> CalculateRoute(CarPool.Clients.Core.Maps.Model.RouteModel route)
        {
            PolylineOptions polyline = await RequestRoutePolyline(route);

            var positions = polyline?.Points.Select(CoordinateConverter.ConvertToAbstraction) ?? Enumerable.Empty<Position>();

            return positions;
        }

        public override void ClearAllUserRoutes()
        {
            _currentUserRoute?.Remove();
            _currentUserRoute = null;
        }

        public override IEnumerable<Position> GetCurrentUserRoute()
        {
            return _currentUserRoute?.Points.Select(CoordinateConverter.ConvertToAbstraction)
                ?? Enumerable.Empty<Position>();
        }

        protected override void DrawCalculatedRouteInMap(CarPool.Clients.Core.Maps.Model.RouteModel route)
        {
            var polyLine = new PolylineOptions();
            
            foreach(var position in route.RoutePoints)
            {
                LatLng nativePosition = CoordinateConverter.ConvertToNative(position);
                polyLine.Add(nativePosition);
            }

            polyLine.InvokeColor(route.Color.ToAndroid());

            _currentUserRoute = _nativeMap.AddPolyline(polyLine);
        }

        private async Task<PolylineOptions> RequestRoutePolyline(CarPool.Clients.Core.Maps.Model.RouteModel route)
        {
            // Origin of route
            string originQueryParam = $"origin={route.From.Latitude.ToString(CultureInfo.InvariantCulture)},{route.From.Longitude.ToString(CultureInfo.InvariantCulture)}";

            // Destination of route
            string destinationQueryParam = $"destination={route.To.Latitude.ToString(CultureInfo.InvariantCulture)},{route.To.Longitude.ToString(CultureInfo.InvariantCulture)}";

            // route waypoints
            if (route.WayPoints != null && route.WayPoints.Any())
            {
                destinationQueryParam += "&waypoints=optimize:true";
                foreach (var point in route.WayPoints)
                {
                    destinationQueryParam += $"|{point.Latitude.ToString(CultureInfo.InvariantCulture)},{point.Longitude.ToString(CultureInfo.InvariantCulture)}";
                }
            }

            // Sensor enabled
            string sensor = "sensor=false";

            // Auth
            // TODO sacar
            string key = $"key={AppSettings.GoogleMapsAPIKey}";

            RootRouteModel routeData = default(RootRouteModel);
            try
            {
                // Building the parameters to the web service
                string parameters = string.Join("&", new[] { originQueryParam, destinationQueryParam, sensor, key });

                UriBuilder uri = new UriBuilder("https://maps.googleapis.com/maps/api/directions/json");
                uri.Query = parameters;

                var requestProvider = new RequestProvider();
                routeData = await requestProvider.GetAsync<RootRouteModel>(uri.ToString());
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error calling routes api from google: {ex}");
            }

            PolylineOptions polylineOptions = new PolylineOptions();

            if (routeData?.Routes?.Any() == true)
            {
                var serviceRoute = routeData.Routes.FirstOrDefault();

                var polylinePoints = serviceRoute.Polyline.Points;

                double totalDistance = 0, totalTime = 0;
                var points = DecodePolyline(polylinePoints);

                foreach (var point in points)
                {
                    polylineOptions.Add(point);
                }

                foreach (var step in serviceRoute.Steps)
                {
                    totalDistance += step?.Distance?.Value ?? 0;
                    totalTime += step?.Duration?.Value ?? 0;
                }

                route.Distance = totalDistance;
                route.Duration = totalTime / 60;
            }

            return polylineOptions;
        }

        /// <summary>
        /// More information: https://agileapp.co/xamarin-forms-maps-polyline-route-highlighted-google-api
        /// </summary>
        public static List<LatLng> DecodePolyline(string encodedPoints)
        {
            if (string.IsNullOrEmpty(encodedPoints))
                throw new ArgumentNullException("encodedPoints");

            char[] polylineChars = encodedPoints.ToCharArray();
            int index = 0;

            int currentLat = 0;
            int currentLng = 0;
            int next5bits;
            int sum;
            int shifter;

            List<LatLng> polylinesPosition = new List<LatLng>();

            while (index < polylineChars.Length)
            {
                // Calculate next latitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length)
                    break;

                currentLat += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                // Calculate next longitude
                sum = 0;
                shifter = 0;
                do
                {
                    next5bits = (int)polylineChars[index++] - 63;
                    sum |= (next5bits & 31) << shifter;
                    shifter += 5;
                } while (next5bits >= 32 && index < polylineChars.Length);

                if (index >= polylineChars.Length && next5bits >= 32)
                    break;

                currentLng += (sum & 1) == 1 ? ~(sum >> 1) : (sum >> 1);

                polylinesPosition.Add(new LatLng(Convert.ToDouble(currentLat) / 1E5, Convert.ToDouble(currentLng) / 1E5));
            }

            return (polylinesPosition);
        }
    }
}