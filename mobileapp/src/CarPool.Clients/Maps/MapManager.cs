using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Extensions;
using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Maps.Pushpins;
using CarPool.Clients.Core.Maps.Routes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Maps
{
    public class MapManager
    {
        private readonly PushpinsObserver _pushpinsObserver;

        private bool _mapAlreadyCentered;

        public AbstractPushpinManager PushpinManager { get; private set; }

        public AbstractRouteManager RouteManager { get; private set; }

        public CustomMap FormsMap { get; private set; }

        public MapManager(
            CustomMap formsMap, 
            AbstractPushpinManager pushpinManager, 
            AbstractRouteManager routeManager)
        {
            FormsMap = formsMap;
            PushpinManager = pushpinManager;
            RouteManager = routeManager;

            _mapAlreadyCentered = false;
            _pushpinsObserver = new PushpinsObserver(this);
        }

        public void Initialize()
        {
            PushpinManager.PinSelected += PushpinSelected;
            PushpinManager.PinUnselected += PushpinUnselected;
            PushpinManager.NavigationRequested += NavigationRequested;
        }

        public Task SetCurrentPushpin(CustomPin pushpin)
        {
            return HandleIncidentSelection(pushpin);
        }

        public async Task HandleCustomMapPropertyChange(PropertyChangedEventArgs e)
        {
            if (e.PropertyName.Equals(CustomMap.CurrentPushpinProperty.PropertyName, StringComparison.CurrentCultureIgnoreCase))
            {
                await SetCurrentPushpin(FormsMap.CurrentPushpin);
            }
            else if (e.PropertyName.Equals(CustomMap.PushpinsProperty.PropertyName, StringComparison.CurrentCultureIgnoreCase))
            {
                _pushpinsObserver.AttachPushpins(FormsMap.Pushpins);

                if (!_mapAlreadyCentered && FormsMap.Pushpins?.Any() == true)
                {
                    if (FormsMap.Pushpins?.Count() > 1)
                    {
                        InitializeMapPosition();
                    }
                    _mapAlreadyCentered = true;
                }
            }
            else if (e.PropertyName.Equals(CustomMap.RoutesProperty.PropertyName, StringComparison.CurrentCultureIgnoreCase))
            {
                if (FormsMap.Routes != null && FormsMap.Routes.Any())
                {
                    foreach (var route in FormsMap.Routes)
                    {
                        await RouteManager.DrawRouteInMap(route);
                    }
                }
            }
            else if (e.PropertyName.Equals(CustomMap.RidersProperty.PropertyName, StringComparison.CurrentCultureIgnoreCase))
            {
                AttachRiders(FormsMap.Riders);
            }
        }

        private void InitializeMapPosition()
        {
            IEnumerable<CustomPin> incidents = FormsMap?.Pushpins;

            if (incidents?.Any() == false)
            {
                return;
            }

            var centerPosition = new Position(incidents.Average(x => x.Position.Latitude),
                incidents.Average(x => x.Position.Longitude));

            var minLongitude = incidents.Min(x => x.Position.Longitude);
            var minLatitude = incidents.Min(x => x.Position.Latitude);

            var maxLongitude = incidents.Max(x => x.Position.Longitude);
            var maxLatitude = incidents.Max(x => x.Position.Latitude);

            var distance = MapHelper.CalculateDistance(minLatitude, minLongitude,
                maxLatitude, maxLongitude, DistanceMeasure.Kilometers) / 2;

            FormsMap.MoveToRegion(MapSpan.FromCenterAndRadius(centerPosition, Distance.FromMiles(distance)));
        }

        private async void PushpinSelected(object sender, CustomPin pushpin)
        {
            if (FormsMap?.CurrentPushpin != pushpin)
            {
                FormsMap.CurrentPushpin = pushpin;
                await HandleIncidentSelection(pushpin);
            }
        }

        private async Task HandleIncidentSelection(CustomPin pushpin)
        {
            await Task.Delay(0);

            if (pushpin != null)
            {
                PushpinManager.ShowPushpinInformationPanel(pushpin);
                FormsMap.SetPosition(pushpin.Position);
            }
            else
            {
                PushpinManager.HidePushpinInformationPanel();
                PushpinManager.SetInteraction(true);
            }
        }

        private void PushpinUnselected(object sender, CustomPin incident)
        {
            FormsMap.CurrentPushpin = null;
        }

        private void NavigationRequested(object sender, CustomPin incident)
        {
        }

        private void LoadTicketsRequested(object sender, IEnumerable<CustomPin> pushpins)
        {
            PushpinManager.AddPushpins(pushpins);
        }

        private void UnloadTicketsRequested(object sender, EventArgs e)
        {
            PushpinManager.RemoveAllPushins();
        }

        private void AttachRiders(IEnumerable<CustomRider> riders)
        {
            PushpinManager.RemoveAllRiders();
            PushpinManager.AddRiders(riders);
        }
    }
}
