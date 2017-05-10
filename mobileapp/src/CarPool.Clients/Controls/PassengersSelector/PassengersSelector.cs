using System.Collections.Generic;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Controls
{
    public class PassengersSelector : ContentView
    {
        private List<ToggleButton> _passengersList = new List<ToggleButton>();

        public static readonly BindableProperty PassengersProperty =
          BindableProperty.Create(nameof(Passengers), typeof(int?), typeof(PassengersSelector), default(int?), 
              propertyChanged: onPassengerSelected);

        public int? Passengers
        {
            get { return (int?)GetValue(PassengersProperty); }
            set { SetValue(PassengersProperty, value); }
        }

        public static readonly BindableProperty MaxPassengersProperty =
          BindableProperty.Create(nameof(MaxPassengers), typeof(int), typeof(PassengersSelector), 5);

        public int MaxPassengers
        {
            get { return (int)GetValue(MaxPassengersProperty); }
            set { SetValue(MaxPassengersProperty, value); }
        }

        public PassengersSelector() : base()
        {
            Grid grid = new Grid();
            
            for (int i = 1; i <= MaxPassengers; i++)
            {
                Grid layout = new Grid();

                //ToggleButton
                ToggleButton label = new ToggleButton();
                label.Checked = false;
                label.Animate = true;
                label.HorizontalOptions = LayoutOptions.Center;
                label.VerticalOptions = LayoutOptions.Center;
                switch (Device.RuntimePlatform)
                {
                    case Device.iOS:
                        label.CheckedImage = $"Passengers/passengers{i}_selected";
                        label.UnCheckedImage = $"Passengers/passengers{i}";
                        break;
                    case Device.Android:
                        label.CheckedImage = $"passengers{i}_selected";
                        label.UnCheckedImage = $"passengers{i}";
                        break;
                    default:
                        label.CheckedImage = $"Assets/Passengers/passengers{i}_selected.png";
                        label.UnCheckedImage = $"Assets/Passengers/passengers{i}.png";
                        break;
                }
                label.CommandParameter = i - 1;
                label.Command = new Command((index) =>
                {
                    this.OnPassengerSelected((int)index);
                });

                _passengersList.Add(label);
                layout.Children.Add(label);

                grid.Children.Add(layout, i - 1, 0);
            }

            this.Content = grid;
        }

        private static void onPassengerSelected(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable != null && bindable is PassengersSelector)
            {
                ((PassengersSelector)bindable).OnPassengerSelected((int)newValue - 1);
            }
        }

        private void OnPassengerSelected(int index)
        {
            for(int i = 0; i < _passengersList.Count; i ++) {
                var layout = _passengersList[i];
                ((ToggleButton)layout).Checked = index == i;
            }
        }
    }
}