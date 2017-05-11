using Microsoft.Graph;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class SchedulePickupConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || !(value is ItemBody))
                return false;

            var itemBody = (ItemBody)value;
            var body = itemBody.Content;

            if(!string.IsNullOrEmpty(body) 
                && body.Contains("Pickup"))
            {
                return true;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
