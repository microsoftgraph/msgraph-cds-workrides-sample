using CarPool.Clients.Core.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class OOFToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is GraphUser)
            {
                if (((GraphUser) value).OOF)
                {
                    return App.Current.Resources["GrayColor"];
                }

                return ((GraphUser)value).ProfileColor;
            }

            return App.Current.Resources["YellowColor"];
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}