using System;
using System.Globalization;
using System.Linq;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class PeopleIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return string.Empty;

            var name = value.ToString();

            var split = name.Split(' ');

            if (split.Any())
            {
                return string.Format("{0}{1}", 
                    split.First().Substring(0, 1), 
                    split.Last().Substring(0, 1));
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
