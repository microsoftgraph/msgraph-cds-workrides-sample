using CarPool.Clients.Core.Maps.Model;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace CarPool.Clients.Core.Maps.Pushpins
{
    internal class PushpinsObserver
    {
        private readonly MapManager _mapManager;
        private IEnumerable<CustomPin> _pushpins;

        public PushpinsObserver(MapManager mapManager)
        {
            _mapManager = mapManager;
        }

        public void AttachPushpins(IEnumerable<CustomPin> pushpins)
        {
            if(pushpins == null)
            {
                return;
            }

            //_mapManager.ResponderManager.StopResponderUpdater();
            _mapManager.PushpinManager.RemoveAllPushins();

            // Remove previous incidents list instance handlers (if existing)
            INotifyCollectionChanged currentCollection = _pushpins as INotifyCollectionChanged;

            if (currentCollection != null)
            {
                currentCollection.CollectionChanged -= OnIncidentsCollectionChanged;
            }

            _pushpins = pushpins;

            if (_pushpins != null)
            {
                _mapManager.PushpinManager.AddPushpins(pushpins);
            }

            // Add new incidents list instance handlers (if needed)
            if (currentCollection != null)
            {
                currentCollection.CollectionChanged -= OnIncidentsCollectionChanged;
            }
        }

        private void OnIncidentsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                IEnumerable<CustomPin> newPushpins = e.NewItems.OfType<CustomPin>();
                _mapManager.PushpinManager.AddPushpins(newPushpins);
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                IEnumerable<CustomPin> removedPushpins = e.OldItems.OfType<CustomPin>();
                _mapManager.PushpinManager.AddPushpins(removedPushpins);
            }
        }
    }
}
