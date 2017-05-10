using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Maps.Routes;
using CarPool.Clients.iOS.Maps.Helpers;
using CarPool.Clients.iOS.Maps.Pushpins;
using CoreLocation;
using MapKit;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;
using System;

namespace CarPool.Clients.iOS.Maps.Routes
{
    public class RouteManager : AbstractRouteManager
    {
        private readonly MKMapView _nativeMap;

        private MKPolyline _currentUserRoute;

        public RouteManager(MKMapView nativeMap, CustomMap formsMap, AnnotationManager annotationManager)
            : base(formsMap, annotationManager)
        {
            _nativeMap = nativeMap;
        }

        public override void ClearAllUserRoutes()
        {
            var allRoutes = _nativeMap.Overlays?.Where(o => o.GetType() != typeof(MKPolygon))
                                                .ToArray();

            if (allRoutes?.Any() == true)
            {
                _nativeMap.RemoveOverlays(allRoutes);
            }

            _currentUserRoute = null;
        }

        public override IEnumerable<Position> GetCurrentUserRoute()
        {
            var positions = _currentUserRoute?.Points.Select(r =>
            {
                var coord = MKMapPoint.ToCoordinate(r);
                return CoordinateConverter.ConvertToAbstraction(coord);
            });

            return positions;
        }

        public override async Task<IEnumerable<Position>> CalculateRoute(RouteModel route)
        {
            List<Position> result = new List<Position>();

            var nativeFrom = CoordinateConverter.ConvertToNative(route.From);
            var nativeTo = CoordinateConverter.ConvertToNative(route.To);

            var actualOriginPoint = nativeFrom;
            
            // ios waypoints not nativelly supported
            if (route.WayPoints != null && route.WayPoints.Any())
            {
                foreach (var waypoint in route.WayPoints)
                {
                    var actualDestinationPoint = CoordinateConverter.ConvertToNative(waypoint);
                    result.AddRange(await RequestMKMapRoutePoints(route, actualOriginPoint, actualDestinationPoint));
                    actualOriginPoint = actualDestinationPoint;
                }
            }

            result.AddRange(await RequestMKMapRoutePoints(route, actualOriginPoint, nativeTo));
            return result;
        }

        private async Task<IEnumerable<Position>> RequestMKMapRoutePoints(RouteModel route, CLLocationCoordinate2D from, CLLocationCoordinate2D to)
        {
            IEnumerable<Position> result = Enumerable.Empty<Position>();

            var userPlaceMark = new MKPlacemark(from, new Foundation.NSDictionary());
            var incidencePlaceMark = new MKPlacemark(to, new Foundation.NSDictionary());

            var sourceItem = new MKMapItem(userPlaceMark);
            var destItem = new MKMapItem(incidencePlaceMark);

            var request = new MapKit.MKDirectionsRequest
            {
                Source = sourceItem,
                Destination = destItem,
                TransportType = MKDirectionsTransportType.Automobile
            };

            var directions = new MKDirections(request);

            MKPolyline polyRoute = null;

            directions.CalculateDirections((response, error) =>
            {
                if (error != null)
                {
                    System.Diagnostics.Debug.WriteLine(error.LocalizedDescription);
                }
                else
                {
                    if (response.Routes.Any())
                    {
                        var firstRoute = response.Routes.First();
                        polyRoute = firstRoute.Polyline;
                        route.Distance += firstRoute.Distance;
                        route.Duration += firstRoute.ExpectedTravelTime / 60;
                    }
                }
            });

            do
            {
                await Task.Delay(100);
            }
            while (directions.Calculating);

            if (polyRoute != null)
            {
                result = polyRoute.Points.Select(s =>
                {
                    CLLocationCoordinate2D coordinate = MKMapPoint.ToCoordinate(s);
                    return CoordinateConverter.ConvertToAbstraction(coordinate);
                });
            }

            return result;
        }

        protected override void DrawCalculatedRouteInMap(RouteModel route)
        {
            var drawingManager = new MapDrawingManager(FormsMap, route.Color);
            _nativeMap.OverlayRenderer = drawingManager.GetOverlayRenderer;

            var nativeCoordinates = route.RoutePoints.Select(p => CoordinateConverter.ConvertToNative(p));
            _currentUserRoute = MKPolyline.FromCoordinates(nativeCoordinates.ToArray());

            _nativeMap.AddOverlay(_currentUserRoute);
        }
    }
}