using CarPool.Clients.Core.Maps;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.UWP.Maps.Extensions.Pushpins;
using CarPool.Clients.UWP.Maps.Renderers;
using CarPool.Clients.UWP.Maps.Routes;
using System;
using System.ComponentModel;
using Windows.UI.Xaml.Controls.Maps;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Maps.UWP;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(CustomMap), typeof(CustomMapRenderer))]
namespace CarPool.Clients.UWP.Maps.Renderers
{
    public class CustomMapRenderer : MapRenderer
    {
        private MapControl _nativeMap;
        private CustomMap _customMap;

        private MapManager _mapManager;

        protected override void OnElementChanged(ElementChangedEventArgs<Map> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                _nativeMap = null;
                _mapManager = null;
            }

            if (e.NewElement != null)
            {
                _nativeMap = Control;
                _customMap = (CustomMap)Element;

                AddManagers();
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName.Equals("Renderer", StringComparison.CurrentCultureIgnoreCase))
            {
                _mapManager?.Initialize();
            }
            else
            {
                _mapManager?.HandleCustomMapPropertyChange(e);
            }
        }

        private void AddManagers()
        {
            var annotationManager = new PushpinManager(_nativeMap, _customMap);
            var routeManager = new RouteManager(_nativeMap, _customMap, annotationManager);
            
            _mapManager = new MapManager(_customMap, annotationManager, routeManager);
        }
    }
}
