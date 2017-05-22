using System;
using Xamarin.Forms;

namespace CarPool.Clients
{
    public static class AppSettings
    {
        public const string CarpoolClientId = "b6fba673-649f-4bfc-b9f4-5c760bd0fa82";

        // Graph API
        public const string GraphApiEndpoint = "https://graph.microsoft.com/";
        //A redirect uri gives AAD more details about the specific application that it will authenticate.  
        //Since a client app does not have an external service to redirect to, this Uri is the standard placeholder for a client app.  
        public const string GraphRedirectUri = "http://workRides";
        // Create an instance of AuthenticationContext to acquire an Azure access token  
        // OAuth2 authority Uri  
        public const string GraphAuthorityUri = "https://login.windows.net/fabrikamco.onmicrosoft.com";

        // CDS
        public const string CdsResourceUri = "https://fabrikamco.onmicrosoft.com/c6e243df-7fa8-454c-8fc4-0089dc7574e2";
        //A redirect uri gives AAD more details about the specific application that it will authenticate.  
        //Since a client app does not have an external service to redirect to, this Uri is the standard placeholder for a client app.  
        public const string CdsRedirectUri = "http://workRides";
        // Create an instance of AuthenticationContext to acquire an Azure access token  
        // OAuth2 authority Uri  
        public const string CdsAuthorityUri = "https://login.windows.net/common/oauth2/authorize";

        public const bool HitLocalService = false;

        // Maps
        public const string BingMapsAPIKey = "pQUKHgYH426DqgbLZtlq~BQ3H10m5gxwUsYa4qiw18Q~ArXhd2-QP9UR2WHV8EdNkmFIEn6VEGb0m-_EOvJOmKzkL14lcoEdVef_fS9Jc6gb";
        public const string GoogleMapsAPIKey = "AIzaSyBoQAHK-txnbmD8zjebtVdg6St0GrPciUY";
        public const string GoogleMapsGeocodeAPIKey = "AIzaSyB_giuFgQGC3owq0uWC3JWzIZUDiZy7NGU";
        public const double MapDistanceMiles = 12;
        public static int currentColor = 0;
        public static Color[] RiderColors = new Color[] {
            Color.FromHex("1DC8EF"),
            Color.FromHex("BAD809"),
            Color.Red,
            Color.Gold,
            Color.DarkMagenta,
            Color.GreenYellow,
            Color.LightCoral,
            Color.Green
        };

        // SQLite
        public const string SQLiteDatabaseName = "carpool.db3";
        public const string OfflineDatabaseName = "offlinecarpool.db3";

        // User
        public const string DefaultDemoUser = "Denis";
        public const string WorkAddressGraphExtensionKey = "workAddress";
        public const string WorkCityGraphExtensionKey = "workCity";
        public const string DefaultWorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA";
        public static TimeSpan DefaultArrivalTime = new TimeSpan(8, 30, 0);
        public static TimeSpan DefaultDepartureTime = new TimeSpan(16, 30, 0);

        // trip estimation
        public const double DefaultMphSpeed = 40;
        public const double DefaultTripTime = 30;
        
        // Events
        public const string CarpoolEventSubject = "Carpool Ride";
        public const string CarpoolEventBody = "Driver:";
        public const int ScheduleMonths = 3;
        public const int ScheduleTopItems = 250;

        // HockeyApp
        public const string HockeyAppiOS = "";
        public const string HockeyAppAndroid = "";
        public const string HockeyAppUWP = "";
    }
}