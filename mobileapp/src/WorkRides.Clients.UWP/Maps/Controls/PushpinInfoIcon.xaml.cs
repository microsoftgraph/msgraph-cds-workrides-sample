using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Maps.Pushpins;
using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Input;

namespace MSCorp.FirstResponse.Client.UWP.Controls
{
    public sealed partial class PushpinInfoIcon : UserControl
    {
        public event EventHandler<PushpinSelectedEventArgs> IncidentIconExit;
        public event EventHandler<PushpinSelectedEventArgs> IncidentIconDetails;
        public event EventHandler<PushpinSelectedEventArgs> IncidentIconNavigate;

        public PushpinInfoIcon(CustomPin pin, bool menuVisible = false)
        {
            InitializeComponent();

            Pushpin = pin;

            ButtonOne.Visibility = menuVisible ? Visibility.Visible : Visibility.Collapsed;
            if (Pushpin.Duration.HasValue)
            {
                Description.Text = string.Format("{0:0.#} min added", Pushpin.Duration.Value);
            }
            else
            {
                ButtonOne.Visibility = Visibility.Collapsed;
            }
        }

        public CustomPin Pushpin { get; }
        private void OnIncidentIconExit(string incidentId) => IncidentIconExit?.Invoke(this, new PushpinSelectedEventArgs(incidentId));
        private void OnIncidentIconDetails(string incidentId, bool detailsVisible) => IncidentIconDetails?.Invoke(this, new PushpinSelectedEventArgs(incidentId));
        private void OnIncidentIconNavigate(string incidentId) => IncidentIconNavigate?.Invoke(this, new PushpinSelectedEventArgs(incidentId));

        public void Close()
        {
            OnIncidentIconExit(Pushpin.Id);
        }

        private void OnIconImageTapped(object sender, TappedRoutedEventArgs e)
        {
            var currentVis = ButtonOne.Visibility;
            OnIncidentIconDetails(Pushpin.Id, currentVis == Visibility.Collapsed);
        }

        private void OnNavigateButtonClick(object sender, RoutedEventArgs e)
        {
            OnIncidentIconNavigate(Pushpin.Id);
        }

        private void OnCloseButtonClicked(object sender, RoutedEventArgs e)
        {
            OnIncidentIconExit(Pushpin.Id);
        }
    }
}