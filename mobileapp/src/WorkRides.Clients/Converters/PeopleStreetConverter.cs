using CarPool.Clients.Core.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class PeopleStreetConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is GraphUser))
                return string.Empty;

            var user = value as GraphUser;

            return $"{user.StreetAddress} {user.City} {user.State} {user.PostalCode}";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
