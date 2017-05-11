using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Maps.Routes;
using CarPool.Clients.UWP.Maps.Helpers;
using CarPool.Clients.UWP.Maps.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Windows.Devices.Geolocation;
using Windows.Services.Maps;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using CarPool.Clients.UWP.Maps.Extensions.Pushpins;

namespace CarPool.Clients.UWP.Maps.Routes
{
    public class RouteManager : AbstractRouteManager
    {
        private readonly MapControl _nativeMap;
        private MapPolyline _currentUserRoute;

        public RouteManager(MapControl nativeMap, CustomMap formsMap, PushpinManager pushpinManager)
            : base(formsMap, pushpinManager)
        {
            _nativeMap = nativeMap;
        }

        public override async Task<IEnumerable<Position>> CalculateRoute(RouteModel route)
        {
            Geopoint nativeFrom = CoordinateConverter.ConvertToNative(route.From);
            Geopoint nativeTo = CoordinateConverter.ConvertToNative(route.To);

            // add route starts
            List<Geopoint> routeWaypoints = new List<Geopoint>() { nativeFrom};

            // add route waypoints if any
            if (route.WayPoints != null && route.WayPoints.Any())
            {
                foreach (var point in route.WayPoints)
                {
                    routeWaypoints.Add(CoordinateConverter.ConvertToNative(point));
                }
            }

            // add route ends
            routeWaypoints.Add(nativeTo);

            MapRouteFinderResult routeResult = await MapRouteFinder.GetDrivingRouteFromWaypointsAsync(
                routeWaypoints,
                MapRouteOptimization.Time,
                MapRouteRestrictions.None);

            List<Position> result = new List<Position>();
            IReadOnlyList<BasicGeoposition> routePositions = routeResult?.Route?.Path?.Positions;
            route.Distance = routeResult?.Route?.LengthInMeters ?? 0;
            route.Duration = routeResult?.Route?.EstimatedDuration.TotalMinutes ?? 0;

            if (routePositions?.Any() == true)
            {
                foreach (BasicGeoposition position in routePositions)
                {
                    result.Add(new Position(position.Latitude, position.Longitude));
                }
            }

            return result;
        }

        public override void ClearAllUserRoutes()
        {
            var allRoutes = _nativeMap.MapElements?.OfType<MapPolyline>()
                                                   .ToArray();

            if (allRoutes?.Any() == true)
            {
                foreach (MapPolyline route in allRoutes)
                {
                    _nativeMap.MapElements.Remove(route);
                }
            }

            _currentUserRoute = null;
        }

        protected override void DrawCalculatedRouteInMap(RouteModel route)
        {
            var nativePositions = route.RoutePoints.Select(CoordinateConverter.ConvertToNative)
                                                   .Select(p => p.Position)
                                                   .ToArray();

            var polyline = new MapPolyline();
            polyline.StrokeColor = route.Color.ToMediaColor();
            polyline.StrokeThickness = 8;
            polyline.Path = new Geopath(nativePositions);
            _nativeMap.MapElements.Add(polyline);

            _currentUserRoute = polyline;
        }

        public override IEnumerable<Position> GetCurrentUserRoute()
        {
            return _currentUserRoute?.Path?.Positions.Select(p => new Position(p.Latitude, p.Longitude)) ?? Enumerable.Empty<Position>();
        }
    }
}
