using System;
using System.Linq;
using System.Globalization;
using Xamarin.Forms;
using CarPool.Clients.Core.Models;
using System.Collections.Generic;

namespace CarPool.Clients.Core.Converters
{
    public class RiderRequestConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if(value is IEnumerable<GraphUser>)
            {
                IEnumerable<GraphUser> list = (IEnumerable<GraphUser>)value;

                if (list == null || !list.Any())
                {
                    return "Rider requests";
                }
                return string.Format("Rider requests ({0:F0})", list.Count());
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
