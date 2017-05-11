using CarPool.Clients.Core.ViewModels.Base;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarPool.Clients.Core.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private MenuViewModel _menuViewModel;

        public MainViewModel(MenuViewModel menuViewModel)
        {
            _menuViewModel = menuViewModel;
        }

        public MenuViewModel MenuViewModel
        {
            get
            {
                return _menuViewModel;
            }

            set
            {
                _menuViewModel = value;
                RaisePropertyChanged(() => MenuViewModel);
            }
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await _menuViewModel.InitializeAsync(navigationData);

            if (navigationData is DriverPreferencesViewModel)
            {
                await NavigationService.NavigateToAsync<DriveViewModel>();
            }
            else if (navigationData is ProfileViewModel)
            {
                await NavigationService.NavigateToAsync<ProfileViewModel>();
            }
            else if (navigationData is LoginViewModel)
            {
                await NavigationService.NavigateToAsync<ScheduleViewModel>();
            }
            else if (navigationData is NewMessageViewModel)
            {
                await NavigationService.NavigateToAsync<DriveViewModel>();
            }
            else
            {
                await NavigationService.NavigateToAsync<FindRideViewModel>();
            }
        }
    }
}