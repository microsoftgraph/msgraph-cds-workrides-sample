using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Maps.Model
{
    public class RouteModel
    {
        public event EventHandler<EventArgs> RouteCalculationCompleted;

        public int Id { get; set; }

        public IEnumerable<Position> RoutePoints { get; set; }

        public Color Color { get; set; }

        public double Duration { get; set; }

        public double Distance { get; set; }

        public Position From { get; set; }

        public Position To { get; set; }

        public IEnumerable<Position> WayPoints { get; set; }

        internal void RouteCalculated()
        {
            RouteCalculationCompleted?.Invoke(null, new EventArgs());
        }
    }
}