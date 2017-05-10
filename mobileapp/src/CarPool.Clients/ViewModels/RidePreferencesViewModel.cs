using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.ViewModels.Base;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Helpers;
using Carpool.Clients.Services.Dialog;

namespace CarPool.Clients.Core.ViewModels
{
    public class RidePreferencesViewModel : SignInViewModelBase
    {
        private readonly IDialogService _dialogService;

        public RidePreferencesViewModel(IDialogService dialogService)
        {
            _dialogService = dialogService; 
        }

        public override Task InitializeAsync(object navigationData)
        {
            WorkAddress = App.CurrentUser.WorkAddress;
            HomeAddress = App.CurrentUser.HomeAddress;

            return base.InitializeAsync(navigationData);
        }

        public ICommand DoneCommand => new Command(async () => await DoneAsync());

        private async Task DoneAsync()
        {
            IsBusy = true;

            try
            {
                if (TodayChecked)
                {
                    App.CurrentUser.RidePeriod = RidePeriod.Today;
                }
                else if (TomorrowChecked)
                {
                    App.CurrentUser.RidePeriod = RidePeriod.Tomorrow;
                }
                else
                {
                    App.CurrentUser.RidePeriod = RidePeriod.EveryDay;
                }

                // home address changed, calculate coordinates
                App.CurrentUser.HomeAddress = HomeAddress;
                if (!await UserHelper.GetUserHomeLocation(App.CurrentUser))
                {
                    await _dialogService.ShowAlertAsync("Cannot find the home address you provided, please verify it's correct.", "Invalid home address", "Retry");
                    return;
                }
                else
                {
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedHomeAddress);
                }


                // work address changed, calculate coordinates
                App.CurrentUser.WorkAddress = WorkAddress;
                if (!await UserHelper.GetUserWorkLocation(App.CurrentUser))
                {
                    await _dialogService.ShowAlertAsync("Cannot find the work address you provided, please verify it's correct.", "Invalid work address", "Retry");
                    return;
                }
                else
                {
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedWorkAddress);
                }
            }
            finally
            {
                IsBusy = false;
            }

            await NavigationService.NavigateToAsync<MainViewModel>(this);
        }
    }
}