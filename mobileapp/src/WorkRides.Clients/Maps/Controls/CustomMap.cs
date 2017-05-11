using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Maps.Pushpins;
using System;
using System.Collections.Generic;
using System.Windows.Input;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Maps.Controls
{
    public class CustomMap : Map
    {

        public CustomMap() {
            Initialize();
        }

        public event EventHandler<PushpinSelectedEventArgs> PushpinSelected;

        public static readonly BindableProperty PushpinsProperty =
            BindableProperty.Create("Pushpins",
                typeof(IEnumerable<CustomPin>), typeof(CustomMap), default(IEnumerable<CustomPin>),
                BindingMode.TwoWay);

        public IEnumerable<CustomPin> Pushpins
        {
            get { return (IEnumerable<CustomPin>)base.GetValue(PushpinsProperty); }
            set { base.SetValue(PushpinsProperty, value); }
        }
        
        public static readonly BindableProperty RoutesProperty =
            BindableProperty.Create("Routes",
                typeof(IEnumerable<RouteModel>), typeof(CustomMap), default(IEnumerable<RouteModel>),
                BindingMode.TwoWay);

        public IEnumerable<RouteModel> Routes
        {
            get { return (IEnumerable<RouteModel>)base.GetValue(RoutesProperty); }
            set { base.SetValue(RoutesProperty, value); }
        }

        public static readonly BindableProperty RidersProperty =
            BindableProperty.Create("Riders",
                typeof(IEnumerable<CustomRider>), typeof(CustomMap), default(IEnumerable<CustomRider>),
                BindingMode.TwoWay);

        public IEnumerable<CustomRider> Riders
        {
            get { return (IEnumerable<CustomRider>)base.GetValue(RidersProperty); }
            set { base.SetValue(RidersProperty, value); }
        }

        public static readonly BindableProperty StartNavigationProperty =
            BindableProperty.Create("StartNavigation",
                typeof(bool), typeof(CustomMap), default(bool));

        public bool StartNavigation
        {
            get { return (bool)base.GetValue(StartNavigationProperty); }
            set { base.SetValue(StartNavigationProperty, value); }
        }
        
        public static readonly BindableProperty CurrentPushpinProperty =
            BindableProperty.Create("CurrentPushpin",
                typeof(CustomPin), typeof(CustomMap), null, propertyChanged: OnCurrentPushpinChanged);

        public CustomPin CurrentPushpin
        {
            get { return (CustomPin)base.GetValue(CurrentPushpinProperty); }
            set { base.SetValue(CurrentPushpinProperty, value); }
        }
        
        public static readonly BindableProperty PolygonProperty =
            BindableProperty.Create("Polygon",
                typeof(Position[]), typeof(CustomMap), default(Position[]));

        public Position[] Polygon
        {
            get { return (Position[])base.GetValue(PolygonProperty); }
            set { base.SetValue(PolygonProperty, value); }
        }


        public static readonly BindableProperty PolygonColorProperty =
            BindableProperty.Create("PolygonColor",
                typeof(Color), typeof(CustomMap), Color.Default);

        public Color PolygonColor
        {
            get { return (Color)base.GetValue(PolygonColorProperty); }
            set { base.SetValue(PolygonColorProperty, value); }
        }

        public ICommand MapTypeCommand => new Command(ChangeMapType);

        private void Initialize()
        {
            if (App.CurrentUser.Latitude.HasValue &&
                App.CurrentUser.Longitude.HasValue)
            {
                // Map initially centered on the user position with a configurable zoom level
                MoveToRegion(MapSpan.FromCenterAndRadius(
                    new Position(
                        App.CurrentUser.Latitude.Value, 
                        App.CurrentUser.Longitude.Value),
                    Distance.FromMiles(AppSettings.MapDistanceMiles)));
            }
        }

        private static void OnCurrentPushpinChanged(BindableObject bindable, object oldValue, object newValue)
        {
            var map = bindable as CustomMap;

            var handler = map?.PushpinSelected;

            if (handler != null && map.CurrentPushpin != null)
            {
                handler(map, new PushpinSelectedEventArgs(map.CurrentPushpin.Id));
            }
        }

        private void ChangeMapType()
        {
            if (MapType.Equals(MapType.Street))
            {
                MapType = MapType.Hybrid;
            }
            else
            {
                MapType = MapType.Street;
            }
        }
    }
}