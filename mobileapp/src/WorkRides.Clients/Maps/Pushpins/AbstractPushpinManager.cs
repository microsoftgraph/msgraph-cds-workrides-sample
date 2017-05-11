using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Model;
using System;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Maps.Pushpins
{
    public abstract class AbstractPushpinManager
    {
        public event EventHandler<CustomPin> PinSelected;

        public event EventHandler<CustomPin> PinUnselected;

        public event EventHandler<CustomPin> NavigationRequested;

        protected readonly CustomMap FormsMap;

        protected AbstractPushpinManager(CustomMap formsMap)
        {
            FormsMap = formsMap;
        }

        public abstract void RemoveAllRiders();

        public abstract void RemoveAllPushins();

        public abstract void RemovePushpins(IEnumerable<CustomPin> removedPushpins);

        public abstract void ShowPushpinInformationPanel(CustomPin pin);

        public abstract void HidePushpinInformationPanel();

        public abstract void SetInteraction(bool active);

        protected abstract void AddPushpinToMap(CustomPin pin);

        protected abstract void AddRiderToMap(CustomRider rider);

        public void AddPushpins(IEnumerable<CustomPin> pins)
        {
            if(pins == null)
            {
                return;
            }

            foreach (var responder in pins)
            {
                AddPushpinToMap(responder);
            }
        }

        public void AddRiders(IEnumerable<CustomRider> riders)
        {
            if (riders == null)
            {
                return;
            }

            foreach (var rider in riders)
            {
                AddRiderToMap(rider);
            }
        }

        protected void OnPushpinSelected(CustomPin pin)
        {
            if (pin != null)
            {
                FormsMap.CurrentPushpin = pin;

                var handler = PinSelected;
                handler?.Invoke(this, pin);
            }
        }

        protected void OnPushpinUnselected(CustomPin incident)
        {
            var handler = PinUnselected;

            handler?.Invoke(this, incident);
        }

        protected void OnNavigationRequested(CustomPin pin)
        {
            var handler = NavigationRequested;

            handler?.Invoke(this, pin);
        }
    }
}
