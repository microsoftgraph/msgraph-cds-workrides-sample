using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using System.ComponentModel;
using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.UWP.Maps.Extensions;

namespace MSCorp.FirstResponse.Client.UWP.Controls
{
    public sealed partial class ResponderIcon : UserControl
    {
        public ResponderIcon(CustomRider rider)
        {
            InitializeComponent();
            
            Rider = rider;
            
            var color = Rider.Color.ToMediaColor();
            StatusColor.Fill = new SolidColorBrush(color);

            ResponderType.Text = Rider.Acronym;
        }

        public CustomRider Rider { get; }
        public int RouteIndex { get; set; } = 0;
        public int RouteStepIndex { get; set; } = 0;
        public double RouteStepLatitude { get; set; } = 0;
        public double RouteStepLongitude { get; set; } = 0;
        public int RouteStepMax { get; set; } = 0;
        public bool OnPatrolRoute { get; set; } = false;
        public IReadOnlyList<BasicGeoposition> IncidentResponsePath { get; set; }
        public DateTime IncidentArrivalTime { get; set; }

        public void UpdateStatus(object sender, PropertyChangedEventArgs e)
        {
            StatusColor.Fill = new SolidColorBrush((sender as CustomRider).Color.ToMediaColor());
        }
    }
}