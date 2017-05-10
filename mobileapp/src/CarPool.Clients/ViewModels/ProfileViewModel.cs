using Carpool.Clients.Services.Dialog;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Services.Graph;
using CarPool.Clients.Core.Services.Graph.User;
using CarPool.Clients.Core.ViewModels.Base;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.ViewModels
{
    public class ProfileViewModel : ViewModelBase
    {
        private int _passengers;

        private string _homeAddress;

        private string _workAddress;

        private readonly IDataService _dataService;

        private readonly IDialogService _dialogService;

        private readonly IUserService _userService;

        public ProfileViewModel(
            IDataService dataService,
            IDialogService dialogService,
            IUserService userService)
        {
            _dataService = dataService;
            _dialogService = dialogService;
            _userService = userService;
        }

        public TimeSpan ArrivalTimeSpan
        {
            get
            {
                return CurrentUser.ArrivalTime.TimeOfDay;
            }

            set
            {
                if (value != null && !value.Equals(CurrentUser.ArrivalTime.TimeOfDay))
                {
                    CurrentUser.ArrivalTime = CurrentUser.ArrivalTime.Date + value;
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedTimeRange);
                    if (Settings.DriverEnabled)
                    {
                        MessagingCenter.Send(CurrentUser, MessengerKeys.DriverChangedTimeRange);
                    }
                    RaisePropertyChanged(() => ArrivalTimeSpan);
                }
            }
        }

        public TimeSpan DepartureTimeSpan
        {
            get
            {
                return CurrentUser.DepartureTime.TimeOfDay;
            }

            set
            {
                if (value != null && !value.Equals(CurrentUser.DepartureTime.TimeOfDay))
                {
                    CurrentUser.DepartureTime = CurrentUser.DepartureTime.Date + value;
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedTimeRange);
                    RaisePropertyChanged(() => DepartureTimeSpan);
                }
            }
        }

        public string HomeAddress
        {
            get
            {
                return _homeAddress;
            }

            set
            {
                if (!value.Equals(_homeAddress))
                {
                    _homeAddress = value;
                    RaisePropertyChanged(() => HomeAddress);
                }
            }
        }

        public string WorkAddress
        {
            get
            {
                return _workAddress;
            }

            set
            {
                if (!value.Equals(_workAddress))
                {
                    _workAddress = value;
                    RaisePropertyChanged(() => WorkAddress);
                }
            }
        }

        public int Passengers
        {
            get
            {
                return _passengers;
            }
            set
            {
                _passengers = value;
                RaisePropertyChanged(() => Passengers);
            }
        }

        public bool IsDriver
        {
            get
            {
                return Settings.DriverEnabled;
            }
            set
            {
                if (value != Settings.DriverEnabled)
                {
                    Settings.DriverEnabled = value;
                    RaisePropertyChanged(()=>IsDriver);

                    if (Settings.DriverEnabled)
                    {
                        if (App.CurrentUser.ArrivalTime.Date.Equals(default(DateTime).Date)) {
                            App.CurrentUser.ArrivalTime = DateTime.Today + AppSettings.DefaultArrivalTime;
                            App.CurrentUser.DepartureTime = DateTime.Today + AppSettings.DefaultDepartureTime;
                            RaisePropertyChanged(() => DepartureTimeSpan);
                            RaisePropertyChanged(() => ArrivalTimeSpan);
                        }
                    }

                    UpdateDriverAsync();
                }
            }
        }

        public async override Task InitializeAsync(object navigationData)
        {
            WorkAddress = CurrentUser.WorkAddress;
            HomeAddress = CurrentUser.HomeAddress;
            Passengers = 3;
            if (navigationData is DriveViewModel)
            {
                await NavigationService.RemoveLastFromBackStackAsync();
            }

            // Get user photo
            if (App.CurrentUser.PhotoStream == null)
            {
                var photo = await _userService.GetUserPhotoAsync(App.CurrentUser.Mail);
                if (photo != null)
                {
                    App.CurrentUser.PhotoStream = ImageSource.FromStream(() => new MemoryStream(photo));
                }
            }
        }

        public ICommand LogoutCommand => new Command(async () => await LogoutAsync());

        public ICommand UpdateWorkAddressCommand => new Command(async () => await UpdateWorkAddressAsync());

        public ICommand UpdateHomeAddressCommand => new Command(async () => await UpdateHomeAddressAsync());

        private async Task LogoutAsync()
        {
            GraphClient.Instance.SignOut();

            await NavigationService.NavigateToAsync<LoginViewModel>();
        }

        private async Task<bool> UpdateWorkAddressAsync()
        {
            IsBusy = true;

            var oldWorkAddress = CurrentUser.WorkAddress;
            CurrentUser.WorkAddress = WorkAddress;
            if (!await UserHelper.GetUserWorkLocation(App.CurrentUser))
            {
                await _dialogService.ShowAlertAsync("Cannot find the work address you provided, please verify that it's correct.", "Invalid work address", "Retry");
                CurrentUser.WorkAddress = oldWorkAddress;
                return false;
            }
            else
            {
                MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedWorkAddress);
            }

            IsBusy = false;
            return true;
        }

        private async Task<bool> UpdateHomeAddressAsync()
        {
            IsBusy = true;

            var oldHomeAddress = CurrentUser.HomeAddress;
            App.CurrentUser.HomeAddress = HomeAddress;
            if (!await UserHelper.GetUserHomeLocation(App.CurrentUser))
            {
                await _dialogService.ShowAlertAsync("Cannot find the home address you provided, please verify that it's correct.", "Invalid home address", "Retry");
                CurrentUser.HomeAddress = oldHomeAddress;
                return false;
            }
            else
            {
                MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedHomeAddress);
            }

            IsBusy = false;
            return true;
        }

        private async void UpdateDriverAsync()
        {
            try
            {
                if (CurrentUser == null)
                {
                    return;
                }

                var drivers = await _dataService.GetAllDriversAsync();
                var driver = drivers.FirstOrDefault(d => d.Name.Equals(CurrentUser.UserPrincipalName));

                if (Settings.DriverEnabled)
                {
                    if (!CurrentUser.HasLocation())
                    {
                        await UpdateHomeAddressAsync();
                    }

                    if (!CurrentUser.HasWorkLocation())
                    {
                        await UpdateWorkAddressAsync();
                    }

                    if (CurrentUser.HasLocation() && CurrentUser.HasWorkLocation())
                    {
                        if (driver == null)
                        {
                            driver = new Driver
                            {
                                Name = CurrentUser.UserPrincipalName,
                                DisplayName = CurrentUser.DisplayName,
                                Latitude = CurrentUser.Latitude.HasValue ? CurrentUser.Latitude.Value : default(double),
                                Longitude = CurrentUser.Longitude.HasValue ? CurrentUser.Longitude.Value : default(double)
                            };
                        }

                        driver.Arrival = CurrentUser.ArrivalTime;
                        driver.Departure = CurrentUser.DepartureTime;

                        await _dataService.InsertOrUpdateDriverAsync(driver);
                    }
                    else
                    {
                        IsDriver = false;
                        if (!CurrentUser.HasLocation())
                        {
                            await _dialogService.ShowAlertAsync("Cannot find the home address you provided, please verify that it's correct.", "Invalid home address", "Retry");
                        }
                        else
                        {
                            await _dialogService.ShowAlertAsync("Cannot find the work address you provided, please verify that it's correct.", "Invalid work address", "Retry");
                        }
                    }
                }
                else
                {
                    if (driver != null)
                    {
                        if (!driver.HasRiders() || 
                            await _dialogService.ShowConfirmAsync("Your carpool have riders. This action will cancel your current carpool.",
                                "Are you sure?",
                                "Ok",
                                "Cancel"))
                        {
                            await _dataService.Remove<Driver>(driver);
                        }
                        else
                        {
                            IsDriver = true;
                        }

                    }
                }
            }
            catch(Exception ex)
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    await DialogService.ShowAlertAsync("An error occured", "Update failure", "Try again");
                    Debug.WriteLine($"[UpdateDriver] Error in: {ex}");
                }
            }
        }
    }
}