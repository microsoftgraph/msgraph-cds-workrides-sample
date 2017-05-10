using CarPool.Clients.Core.Maps.Controls;
using MapKit;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

namespace CarPool.Clients.iOS.Maps
{
    public class MapDrawingManager
    {
        private readonly CustomMap _formsMap;

        private Color _routeColor = Color.Black;

        public MapDrawingManager(CustomMap formsMap, Color color)
        {
            _formsMap = formsMap;
            _routeColor = color;
        }

        public MKOverlayRenderer GetOverlayRenderer(MKMapView mapView, IMKOverlay overlay)
        {
            if (overlay is MKPolyline)
            {
                var polylineRenderer = new MKPolylineRenderer(overlay as MKPolyline);

                polylineRenderer.FillColor = _routeColor.ToUIColor();
                polylineRenderer.StrokeColor = _routeColor.ToUIColor();
                polylineRenderer.LineWidth = 8;

                return polylineRenderer;
            }
            else if (overlay is MKPolygon)
            {
                var polygonRenderer = new MKPolygonRenderer(overlay as MKPolygon);

                polygonRenderer.FillColor = _formsMap.PolygonColor.ToUIColor();
                polygonRenderer.StrokeColor = Xamarin.Forms.Color.Transparent.ToUIColor();

                return polygonRenderer;
            }

            return null;
        }
    }
}