// Helpers/Settings.cs
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace CarPool.Clients.Core.Helpers
{
    /// <summary>
    /// This is the Settings static class that can be used in your Core solution or in any
    /// of your client applications. All settings are laid out the same exact way with getters
    /// and setters. 
    /// </summary>
    public static class Settings
    {
        private static ISettings AppSettings
        {
            get
            {
                return CrossSettings.Current;
            }
        }

        #region Setting Constants

        private const string TokenKey = "token";
        private static readonly string TokenDefault = string.Empty;

        private const string ExpirationKey = "expiration";
        private static readonly string ExpirationDefault = string.Empty;

        private const string DriverEnabledKey = "driver";
        private static readonly bool DriverEnabledDefault = false;

        private const string DeltaLinkKey = "deltaLink";
        private static readonly string DeltaLinkDefault = string.Empty;

        #endregion


        public static string TokenForUser
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(TokenKey, TokenDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(TokenKey, value);
            }
        }

        public static bool DriverEnabled
        {
            get
            {
                return AppSettings.GetValueOrDefault<bool>(DriverEnabledKey, DriverEnabledDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<bool>(DriverEnabledKey, value);
            }
        }

        public static string Expiration
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(ExpirationKey, ExpirationDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(ExpirationKey, value);
            }
        }

        public static string DeltaLink
        {
            get
            {
                return AppSettings.GetValueOrDefault<string>(DeltaLinkKey, DeltaLinkDefault);
            }
            set
            {
                AppSettings.AddOrUpdateValue<string>(DeltaLinkKey, value);
            }
        }
    }
}