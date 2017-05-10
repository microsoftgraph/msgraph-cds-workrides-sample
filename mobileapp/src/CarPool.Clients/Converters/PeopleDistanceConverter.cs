using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class PeopleDistanceConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null && value is double?)
            {
                var distance = value as double?;
                if (distance.HasValue && double.NaN != distance.Value)
                {
                    return String.Format("Lives {0:0.#} miles from you", Math.Round(distance.Value, 1));
                }
            }

            return "Lives - miles from you";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
