using System;
using System.Collections.Generic;
using System.Linq;
using Windows.Devices.Geolocation;
using Windows.Foundation;
using Windows.Storage.Streams;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls.Maps;
using MSCorp.FirstResponse.Client.UWP.Controls;
using Xamarin.Forms.Maps;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Pushpins;
using CarPool.Clients.UWP.Maps.Helpers;
using CarPool.Clients.Core.Maps.Model;
using System.Diagnostics;

namespace CarPool.Clients.UWP.Maps.Extensions.Pushpins
{
    public class PushpinManager : AbstractPushpinManager
    {
        private readonly MapControl _nativeMap;
        private readonly MapItemsControl _mapItems;
        private readonly Dictionary<MapIcon, string> _pushpinMappings;

        public PushpinManager(MapControl nativeMap, CustomMap formsMap) 
            : base(formsMap)
        {
            _nativeMap = nativeMap;
            _mapItems = new MapItemsControl();
            _nativeMap.Children.Add(_mapItems);
            _pushpinMappings = new Dictionary<MapIcon, string>();

            _nativeMap.MapElementClick += MapElementClick;
        }

        protected async override void AddPushpinToMap(CustomPin pin)
        {
            try
            {
                var geoLocation = CoordinateConverter.ConvertToNative(pin.Position);

                var mapIcon = new MapIcon();
                mapIcon.CollisionBehaviorDesired = MapElementCollisionBehavior.RemainVisible;
                mapIcon.Location = geoLocation;
                mapIcon.NormalizedAnchorPoint = new Point(0.5, 1.0);
                mapIcon.ZIndex = 1000;

                if (pin.Icon != null)
                {
                    var iconImageUri = default(Uri);
                    iconImageUri = new Uri("ms-appx:///Assets/" + pin.Icon);
                    RandomAccessStreamReference stream = RandomAccessStreamReference.CreateFromUri(iconImageUri);
                    mapIcon.Image = await stream.ScaleTo(40, 58);
                }

                _nativeMap.MapElements.Add(mapIcon);
                _pushpinMappings.Add(mapIcon, pin.Id);
                if (pin.Duration.HasValue)
                {
                    ShowPushpinInformationPanel(pin);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public override void ShowPushpinInformationPanel(CustomPin pin)
        {
            DismissCurrentIncidentPanel();

            var panel = new PushpinInfoIcon(pin, true);
            panel.IncidentIconExit += OnIncidentPanelClosed;
            panel.Margin = new Thickness(0, 0, 0, 60);

            _mapItems.Items.Add(panel);
            SetMapIconPosition(panel, pin.Position, new Point(0.5, 1));
        }

        public MapIcon getPushpin(CustomPin pin)
        {
            return _pushpinMappings.Where(i => i.Value == pin.Id).FirstOrDefault().Key;
        }
        
        public override void RemoveAllPushins()
        {
            var allIcons = _mapItems.Items.OfType<PushpinInfoIcon>()
                                                   .ToList();
            RemoveIcons(allIcons);
        }
        
        public override void HidePushpinInformationPanel()
        {
            DismissCurrentIncidentPanel();
        }
        
        public override void SetInteraction(bool active)
        {
            // For UWP we leave this empty
        }

        public void DismissCurrentIncidentPanel()
        {
            var panel = _mapItems.Items.OfType<PushpinInfoIcon>()
                                       .FirstOrDefault();

            if (panel != null)
            {
                panel.IncidentIconExit -= OnIncidentPanelClosed;
                _mapItems.Items.Remove(panel);
            }
        }

        private Position GetIconPosition(DependencyObject icon)
        {
            if (icon == null)
            {
                return default(Position);
            }

            Geopoint geoLocation = MapControl.GetLocation(icon);

            return CoordinateConverter.ConvertToAbstraction(geoLocation);
        }

        private void SetMapIconPosition(DependencyObject icon, Position geoLocation, Point anchorPoint)
        {
            var nativeCoordinate = CoordinateConverter.ConvertToNative(geoLocation);

            MapControl.SetLocation(icon, nativeCoordinate);
            MapControl.SetNormalizedAnchorPoint(icon, anchorPoint);
        }
        
        private void MapElementClick(MapControl sender, MapElementClickEventArgs args)
        {
            MapIcon icon = args.MapElements.OfType<MapIcon>()
                                           .FirstOrDefault();

            if (icon != null && _pushpinMappings.ContainsKey(icon))
            {
                string incidentId = _pushpinMappings[icon];
                CustomPin pushpins = FormsMap.Pushpins?                        
                    .Where(i => i.Id == incidentId)    
                    .FirstOrDefault();

                if (pushpins != null)
                {
                    OnPushpinSelected(pushpins);
                }
            }
        }

        private void OnIncidentPanelClosed(object sender, PushpinSelectedEventArgs e)
        {
            FormsMap.CurrentPushpin = null;
            DismissCurrentIncidentPanel();
        }

        public override void RemovePushpins(IEnumerable<CustomPin> removedPushpins)
        {
            var iconsToRemove = _pushpinMappings.Where(x => removedPushpins.Any(i => i.Id == x.Value))                               
                .Select(x => x.Key)           
                .ToList();

            RemoveIcons(iconsToRemove);
        }

        private void RemoveIcons(IEnumerable<DependencyObject> icons)
        {
            if (icons != null)
            {
                foreach (var icon in icons)
                {
                    _mapItems?.Items?.Remove(icon);
                    if (icon is MapIcon)
                    {
                        _pushpinMappings.Remove(icon as MapIcon);
                    }
                }
            }
        }

        public override void RemoveAllRiders()
        {
            var allRiderIcons = _mapItems.Items.OfType<ResponderIcon>()
                                                   .ToList();
            RemoveIcons(allRiderIcons);
        }

        protected override void AddRiderToMap(CustomRider rider)
        {
            var responderIcon = new ResponderIcon(rider);

            _mapItems.Items.Add(responderIcon);
            SetMapIconPosition(responderIcon, rider.Position, new Point(0.5, 0.5));
        }
    }
}