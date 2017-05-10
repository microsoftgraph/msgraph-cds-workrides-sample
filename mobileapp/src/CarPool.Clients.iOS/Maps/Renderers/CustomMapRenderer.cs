using CarPool.Clients.Core.Maps;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.iOS.Maps.Pushpins;
using CarPool.Clients.iOS.Maps.Renderers;
using CarPool.Clients.iOS.Maps.Routes;
using MapKit;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Maps.iOS;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CarPool.Clients.iOS.Maps.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        private MKMapView _iosMapView;
        private CustomMap _customMap;

        private MapDrawingManager _drawingManager;
        private MapManager _mapManager;

        public static void Init()
        {
            
        }

        protected override void OnElementChanged(ElementChangedEventArgs<View> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _iosMapView = null;
                _drawingManager = null;
                _mapManager = null;
            }

            if (e.NewElement != null)
            {
                _iosMapView = (MKMapView)Control;
                _customMap = (CustomMap)Element;
                _iosMapView.ZoomEnabled = true;

                AddManagers();
            }
        }

        private void AddManagers()
        {
            var annotationManager = new AnnotationManager(_iosMapView, _customMap);
            var routeManager = new RouteManager(_iosMapView, _customMap, annotationManager);

            _mapManager = new MapManager(_customMap, annotationManager, routeManager);
            _iosMapView.GetViewForAnnotation = annotationManager.GetViewForAnnotation;

            _drawingManager = new MapDrawingManager(_customMap, Color.Black);
            _iosMapView.OverlayRenderer = _drawingManager.GetOverlayRenderer;
        }

        protected async override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (_mapManager != null)
            {
                if (e.PropertyName.Equals("Renderer", StringComparison.CurrentCultureIgnoreCase))
                {
                    _mapManager?.Initialize();
                }
                else
                {
                    await _mapManager?.HandleCustomMapPropertyChange(e);
                }
            }
        }
    }
}
