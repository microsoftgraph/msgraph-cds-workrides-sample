using Acr.UserDialogs;
using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using CarPool.Clients.Core.Services.Graph;
using CarPool.Clients.Droid.Services.PhoneCall;
using FFImageLoading;
using FFImageLoading.Forms.Droid;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;

namespace CarPool.Clients.Droid.Activities
{
    [Activity(
        Label = "CarPool",
        Icon = "@drawable/icon",
        Theme = "@style/MainTheme", 
        ScreenOrientation = ScreenOrientation.Portrait,
        ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {
        protected override void OnCreate(Bundle bundle)
        {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(bundle);

            SupportActionBar.SetDisplayShowHomeEnabled(true); // Show or hide the default home button
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowCustomEnabled(true); // Enable overriding the default toolbar layout
            SupportActionBar.SetDisplayShowTitleEnabled(false);

            global::Xamarin.Forms.Forms.Init(this, bundle);

            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            UserDialogs.Init(this);
            CachedImageRenderer.Init();
            PhoneCallService.Init();
            Xamarin.FormsMaps.Init(this, bundle);

            InitializeHockeyApp();

            LoadApplication(new App());
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            AuthenticationAgentContinuationHelper.SetAuthenticationAgentContinuationEventArgs(requestCode, resultCode, data);
        }

        /// <summary>
        /// FFImageLoading image service preserves in heap memory of the device every image newly downloaded 
        /// from url. In order to avoid application crash, you should reclaim memory in low memory situations.
        /// </summary>
        public override void OnTrimMemory([GeneratedEnum] TrimMemory level)
        {
            ImageService.Instance.InvalidateMemoryCache();
            GC.Collect(GC.MaxGeneration, GCCollectionMode.Forced);
            base.OnTrimMemory(level);
        }

        private void InitializeHockeyApp()
        {
            if (string.IsNullOrWhiteSpace(AppSettings.HockeyAppAndroid))
                return;

            HockeyApp.Android.CrashManager.Register(this, AppSettings.HockeyAppAndroid);
            HockeyApp.Android.Metrics.MetricsManager.Register(Application, AppSettings.HockeyAppAndroid);
        }
    }
}