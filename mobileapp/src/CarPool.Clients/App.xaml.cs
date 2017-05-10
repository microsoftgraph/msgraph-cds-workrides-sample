using Carpool.Clients.Services.Navigation;
using Carpool.Clients.ViewModels.Base;
using CarPool.Clients.Core.Models;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarPool.Clients
{
    public partial class App : Application
    {
        public static GraphUser CurrentUser { get; set; }

        public App()
        {
            InitializeComponent();

            if (Device.RuntimePlatform == Device.Windows)
            {
                InitNavigation();
            }
        }

        protected async override void OnStart()
        {
            base.OnStart();

            if (Device.RuntimePlatform != Device.Windows)
            {
                await InitNavigation();
            }
        }

        protected override void OnSleep()
        {
            // Handle when your app sleeps
        }

        protected override void OnResume()
        {
            // Handle when your app resumes
        }

        private Task InitNavigation()
        {
            var navigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
            return navigationService.InitializeAsync();
        }
    }
}