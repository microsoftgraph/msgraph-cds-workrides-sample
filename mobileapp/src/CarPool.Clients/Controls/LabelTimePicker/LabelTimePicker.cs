using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Controls
{
    class LabelTimePicker : StackLayout
    {
        private TimePicker _picker;
        private Label _pickerLabel;

        public LabelTimePicker ()
        {
            _picker = new TimePicker
            {
                IsVisible = false,
                Time = DateTime.Now.TimeOfDay,
                HorizontalOptions = LayoutOptions.Start
            };

            _pickerLabel = new Label
            {
                TextColor = Color.Black,
                Text = DateTime.Now.ToString("h:mm tt")?
                .ToLowerInvariant(),
                HorizontalOptions = LayoutOptions.Start
            };

            GestureRecognizers.Add(new TapGestureRecognizer
            {
                Command = new Command<View>(LabelTapped)
            });

            Children.Add(_picker);
            Children.Add(_pickerLabel);
            HorizontalOptions = LayoutOptions.Start;
        }
        
        #region Bindable Properties

        public static readonly BindableProperty TimeProperty =
          BindableProperty.Create(
              nameof(Time), 
              typeof(TimeSpan), 
              typeof(LabelTimePicker), 
              default(TimeSpan),
              propertyChanged: OnTimeChanged);
        
        public TimeSpan Time
        {
            get { return (TimeSpan)GetValue(TimeProperty); }
            set { SetValue(TimeProperty, value); }
        }

        public static readonly BindableProperty TextStyleProperty =
          BindableProperty.Create(
              nameof(TextStyle),
              typeof(Style),
              typeof(LabelTimePicker),
              default(Style),
              propertyChanged: OnTextStyleChanged);

        public Style TextStyle
        {
            get { return (Style)GetValue(TextStyleProperty); }
            set { SetValue(TextStyleProperty, value); }
        }

        public static readonly BindableProperty TextColorProperty =
          BindableProperty.Create(
              nameof(TextColor),
              typeof(Color),
              typeof(LabelTimePicker),
              default(Color),
              propertyChanged: (b, o, n) => { ((LabelTimePicker)b)._pickerLabel.TextColor = (Color)n; });

        public Color TextColor
        {
            get { return (Color)GetValue(TextColorProperty); }
            set { SetValue(TextColorProperty, value); }
        }

        #endregion
        
        private static void OnTimeChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabelTimePicker)
            {
                var labelpicker = bindable as LabelTimePicker;
                labelpicker._pickerLabel.Text = string.Format("{0:hh:mm tt}", DateTime.Today + (TimeSpan)newValue)?
                    .ToLowerInvariant();

                if (!labelpicker._picker.Time.Equals((TimeSpan)newValue))
                {
                    labelpicker._picker.Time = (TimeSpan)newValue;
                }
            }
        }

        private static void OnTextStyleChanged(BindableObject bindable, object oldValue, object newValue)
        {
            if (bindable is LabelTimePicker)
            {
                var labelpicker = bindable as LabelTimePicker;
                labelpicker._pickerLabel.Style = (Style)newValue;
            }
        }

        private void LabelTapped(View obj)
        {
            Device.BeginInvokeOnMainThread(() =>
            {
                switch (Device.RuntimePlatform)
                {
                    case Device.Android:
                    case Device.iOS:
                        _picker.Focus();
                        break;
                    default:
                        _pickerLabel.IsVisible = false;
                        _picker.IsVisible = true;
                        _picker.Focus();
                        break;
                }
                _picker.PropertyChanged -= SelectedTime;
                _picker.PropertyChanged += SelectedTime;
            });
        }

        private void SelectedTime(object sender, PropertyChangedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.PropertyName);
            if (e.PropertyName.Equals(nameof(_picker.Time)))
            {
                if (Device.RuntimePlatform == Device.Android || Device.RuntimePlatform == Device.WinPhone)
                {
                    _picker.PropertyChanged -= SelectedTime;
                }
                _picker.IsVisible = false;
                Time = _picker.Time;
                _pickerLabel.IsVisible = true;
            }
            else if (e.PropertyName.Equals(nameof(_picker.IsFocused)))
            {
                if (Device.RuntimePlatform == Device.iOS && !_picker.IsFocused)
                {
                    _picker.PropertyChanged -= SelectedTime;
                }
            }
        }
    }
}