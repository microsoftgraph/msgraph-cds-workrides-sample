using CarPool.Clients.Core.Models;
using System;
using System.Globalization;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Converters
{
    public class MenuItemTypeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var menuItemType = (MenuItemType)value;

            switch (menuItemType)
            {
                case MenuItemType.Schedule:
                    return (Device.RuntimePlatform == Device.Windows ? "Assets/Menu/menu_schedule.png" : "menu_schedule");
                case MenuItemType.FindRide:
                    return (Device.RuntimePlatform == Device.Windows ? "Assets/Menu/menu_find.png" : "menu_find");
                case MenuItemType.MyProfile:
                    return (Device.RuntimePlatform == Device.Windows ? "Assets/Menu/menu_settings.png" : "menu_settings");
                case MenuItemType.Drive:
                    return (Device.RuntimePlatform == Device.Windows ? "Assets/Menu/menu_drive.png" : "menu_drive");
                default:
                    return string.Empty;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}