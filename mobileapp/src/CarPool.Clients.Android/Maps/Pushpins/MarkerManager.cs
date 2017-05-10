using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Maps.Pushpins;
using CarPool.Clients.Droid.Maps.Adapters;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System;
using CarPool.Clients.Droid.Maps.Icons;

namespace CarPool.Clients.Droid.Maps.Pushpins
{
    public class MarkerManager : AbstractPushpinManager
    {
        private readonly Dictionary<string, Marker> _pushpinMappings;
        private readonly Dictionary<string, Marker> _ridersMappings;
        private GoogleMap _nativeMap;
        private bool _isUserInteractionEnabled;
        private readonly MapInfoWindowAdapter _infoWindowAdapter;

        public MarkerManager(GoogleMap nativeMap, CustomMap formsMap)
            : base(formsMap)
        {
            _nativeMap = nativeMap;
            _pushpinMappings = new Dictionary<string, Marker>();
            _ridersMappings = new Dictionary<string, Marker>();
            _isUserInteractionEnabled = true;

            _nativeMap.MarkerClick += OnMarkerClick;

            // Incident info panel
            _infoWindowAdapter = new MapInfoWindowAdapter();
            _nativeMap.SetInfoWindowAdapter(_infoWindowAdapter);
            _nativeMap.InfoWindowClick += InfoWindowClick;
            _nativeMap.InfoWindowClose += InfoWindowClose;
        }

        protected override void AddPushpinToMap(CustomPin pin)
        {
            try
            {
                var incidentIcon = new PushpinIcon(pin);

                var markerOptions = incidentIcon.MarkerOptions;
                markerOptions.SetPosition(new LatLng(pin.Latitude, pin.Longitude));

                Marker marker = _nativeMap.AddMarker(markerOptions);
                if (!_pushpinMappings.ContainsKey(pin.Id))
                {
                    _pushpinMappings.Add(pin.Id, marker);
                    if (pin.Duration.HasValue)
                    {
                        ShowPushpinInformationPanel(pin);
                    }
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        public override void ShowPushpinInformationPanel(CustomPin pin)
        {
            // Open the selected marker info windows
            if (_pushpinMappings.ContainsKey(pin.Id))
            {
               var marker = _pushpinMappings[pin.Id];
               if (!marker.IsInfoWindowShown)
                {
                    if (pin.Duration.HasValue)
                    {
                        _infoWindowAdapter.Duration = pin.Duration.Value;
                    }
                    marker.ShowInfoWindow();
                }
            }
        }

        public override void HidePushpinInformationPanel()
        {
            foreach(var marker in _pushpinMappings.Values)
            {
                if (marker.IsInfoWindowShown)
                {
                    marker.HideInfoWindow();
                }
            }
        }

        public override void RemoveAllPushins()
        {
            foreach(var marker in _pushpinMappings.Values)
            {
                marker.Remove();
            }

            _pushpinMappings.Clear();
        }

        public override void RemovePushpins(IEnumerable<CustomPin> removedPushpins)
        {
            List<KeyValuePair<string, Marker>> entriesToRemove =
                _pushpinMappings.Where(x => removedPushpins.Any(i => i.Id == x.Key))
                                        .ToList();

            foreach (KeyValuePair<string, Marker> entry in entriesToRemove)
            {
                entry.Value.Remove();
                _pushpinMappings.Remove(entry.Key);
            }
        }

        public override void SetInteraction(bool active)
        {
            _isUserInteractionEnabled = active;
        }
        

        public Marker GetMarkerForPushpin(CustomPin pushpin)
        {
            return _pushpinMappings.ContainsKey(pushpin.Id)
                    ? _pushpinMappings[pushpin.Id]
                    : null;
        }

        private void OnMarkerClick(object sender, GoogleMap.MarkerClickEventArgs e)
        {
            var selectedMarker = e.Marker;

            if (selectedMarker == null || !_isUserInteractionEnabled)
            {
                // If user interaction is disabled, don't show info window
                e.Handled = true;
                return;
            }

            KeyValuePair<string, Marker> keyPair = GetPushpinMappedMarker(e.Marker);

            // Check if this is a incident marker
            if (keyPair.Equals(default(KeyValuePair<string, Marker>)))
            {
                return;
            }

            CustomPin pin = FormsMap.Pushpins?.Where(i => i.Id == keyPair.Key)
                                                       .FirstOrDefault();

            if (pin != null)
            {
                OnPushpinSelected(pin);
            }

            _infoWindowAdapter.Duration = pin.Duration;

            // Mark as unhandled (to let info window appear)
            e.Handled = false;
        }

        private KeyValuePair<string, Marker> GetPushpinMappedMarker(Marker marker)
        {
            return _pushpinMappings.FirstOrDefault(m => m.Value.Id == marker.Id);
        }

        private void InfoWindowClick(object sender, GoogleMap.InfoWindowClickEventArgs e)
        {
            OnNavigationRequested(FormsMap.CurrentPushpin);
        }

        private void InfoWindowClose(object sender, GoogleMap.InfoWindowCloseEventArgs e)
        {
            _infoWindowAdapter.Duration = null;
            OnPushpinUnselected(FormsMap.CurrentPushpin);
        }

        public override void RemoveAllRiders()
        {
            List<Marker> allMarkers = _ridersMappings.Select(m => m.Value)
                                                               .ToList();
            _ridersMappings.Clear();

            foreach (Marker marker in allMarkers)
            {
                marker.Remove();
            }
        }

        protected override void AddRiderToMap(CustomRider rider)
        {
            var riderIcon = new RiderIcon(rider);

            var markerOptions = riderIcon.MarkerOptions;
            markerOptions.SetPosition(new LatLng(rider.Latitude, rider.Longitude));

            Marker marker = _nativeMap.AddMarker(riderIcon.MarkerOptions);
            _ridersMappings.Add(rider.Id, marker);
        }
    }
}