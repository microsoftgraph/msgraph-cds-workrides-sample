using Microsoft.Graph;
using System;
using System.Globalization;

namespace CarPool.Clients.Core.Models
{
    public class ScheduleGraphEvent
    {
        public GraphEvent From { get; set; }

        public GraphEvent To { get; set; }

        public ScheduleGraphEvent() {
            To = new GraphEvent();
            From = new GraphEvent();
        }

        public ScheduleGraphEvent(GraphEvent to, GraphEvent from)
        {
            To = to;
            From = from;
        }
    }
}