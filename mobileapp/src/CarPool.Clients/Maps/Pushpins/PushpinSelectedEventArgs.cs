using System;

namespace CarPool.Clients.Core.Maps.Pushpins
{
    public class PushpinSelectedEventArgs : EventArgs
    {
        public PushpinSelectedEventArgs(string pushpinId)
        {
            PushpinId = pushpinId;
        }

        public string PushpinId { get; }
    }
}