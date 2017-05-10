using CarPool.Clients.Core.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class RidePeriodConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RidePeriod?)
            {
                var ridePeriod = value as RidePeriod?;

                if (ridePeriod.HasValue)
                {
                    if (ridePeriod.Value.Equals(RidePeriod.Today))
                    {
                        return "today";
                    }
                    else if (ridePeriod.Value.Equals(RidePeriod.EveryDay))
                    {
                        return "every day";
                    }
                    else
                    {
                        return "tomorrow";
                    }
                }

                return "";
            }

            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
