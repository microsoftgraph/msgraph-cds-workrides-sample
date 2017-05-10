using Foundation;
using UIKit;
using Carpool.Clients.ViewModels.Base;
using Carpool.Clients.Services.Navigation;
using FFImageLoading.Forms.Touch;
using FFImageLoading.Transformations;
using Xamarin.Forms.Platform.iOS;
using CarPool.Clients.iOS.Services.PhoneCall;
using HockeyApp.iOS;

namespace CarPool.Clients.iOS
{
    // The UIApplicationDelegate for the application. This class is responsible for launching the 
    // User Interface of the application, as well as listening (and optionally responding) to 
    // application events from iOS.
    [Register("AppDelegate")]
    public partial class AppDelegate : FormsApplicationDelegate
    {
        //
        // This method is invoked when the application has loaded and is ready to run. In this 
        // method you should instantiate the window, load the UI into it and then make the window
        // visible.
        //
        // You have 17 seconds to return from this method, or iOS will terminate your application.
        //
        public override bool FinishedLaunching(UIApplication app, NSDictionary options)
        {
            if (!string.IsNullOrWhiteSpace(AppSettings.HockeyAppiOS))
            {
                var manager = BITHockeyManager.SharedHockeyManager;
                manager.Configure(AppSettings.HockeyAppiOS);

                // Disable update manager
                manager.DisableUpdateManager = true;
                manager.StartManager();
            }
            
            global::Xamarin.Forms.Forms.Init();

            RegisterPlatformDependencies();
            Microsoft.WindowsAzure.MobileServices.CurrentPlatform.Init();
            CachedImageRenderer.Init();
            PhoneCallService.Init();
            var ignore = new CircleTransformation();
            Xamarin.FormsMaps.Init();

            LoadApplication(new App());

            return base.FinishedLaunching(app, options);
        }

        private static void RegisterPlatformDependencies()
        {
            ViewModelLocator.Instance.RegisterSingleton<INavigationService, iOSNavigationService>();
        }
    }
}
