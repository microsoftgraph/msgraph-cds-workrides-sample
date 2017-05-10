using Carpool.Clients.Services.Dialog;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Services.Graph.Calendar;
using CarPool.Clients.Core.ViewModels.Base;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.ViewModels
{
    public class DriverPreferencesViewModel : SignInViewModelBase
    {
        private readonly ICalendarService _calendarService;
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private int _passengers;

        public DriverPreferencesViewModel(
            ICalendarService calendarService,
            IDataService dataService,
            IDialogService dialogService)
        {
            _calendarService = calendarService;
            _dataService = dataService;
            _dialogService = dialogService;

            Passengers = 3;
        }

        public int Passengers
        {
            get { return _passengers; }
            set
            {
                _passengers = value;
                RaisePropertyChanged(() => Passengers);
            }
        }

        public override Task InitializeAsync(object navigationData)
        {
            WorkAddress = App.CurrentUser.WorkAddress;
            HomeAddress = App.CurrentUser.HomeAddress;

            return base.InitializeAsync(navigationData);
        }

        public ICommand SearchCommand => new Command(async () => await SearchAsync());

        private async Task SearchAsync()
        {
            try
            {
                IsBusy = true;

                // home address changed, fetch coordinates
                App.CurrentUser.HomeAddress = HomeAddress;
                if (!await UserHelper.GetUserHomeLocation(App.CurrentUser))
                {
                    await _dialogService.ShowAlertAsync("Cannot find the home address you provided, please verify that it's correct.", "Invalid home address", "Retry");
                    return;
                }
                else
                {
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedHomeAddress);
                }

                // work address changed, fetch coordinates
                App.CurrentUser.WorkAddress = WorkAddress;
                if (!await UserHelper.GetUserWorkLocation(App.CurrentUser))
                {
                    await _dialogService.ShowAlertAsync("Cannot find the work address you provided, please verify that it's correct.", "Invalid work address", "Retry");
                    return;
                }
                else
                {
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedWorkAddress);
                }

                try
                {
                    var drivers = await _dataService.GetAllDriversAsync();
                    var driver = drivers.FirstOrDefault(d => d.Name.Equals(CurrentUser.UserPrincipalName));

                    if (driver == null)
                    {
                        // Create Driver if new 
                        driver = new Driver
                        {
                            Name = App.CurrentUser.UserPrincipalName,
                            DisplayName = CurrentUser.DisplayName,
                            Latitude = CurrentUser.Latitude,
                            Longitude = CurrentUser.Longitude,
                            Arrival = DateTime.Today + ArrivalTimeSpan,
                            Departure = DateTime.Today + DepartureTimeSpan
                        };

                        if (TodayChecked)
                        {
                            driver.Schedule = "today";
                        }
                        else if (TomorrowChecked)
                        {
                            driver.Arrival = driver.Arrival.AddDays(1);
                            driver.Departure = driver.Departure.AddDays(1);
                            driver.Schedule = "tomorrow";
                        }
                        else
                        {
                            driver.Schedule = "every day";
                        }

                        await _dataService.InsertOrUpdateDriverAsync(driver);
                        Settings.DriverEnabled = true;
                    }
                }
                catch(Exception ex)
                {
                    Debug.WriteLine($"[SearchAsync] Error creating user: {ex}");
                    if (!IsConnected)
                    {
                        await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                    }
                    else
                    {
                        await DialogService.ShowAlertAsync("CDS service currently unavailable, please try again", "Service unavailable", "Try again");
                    }
                }

                await NavigationService.NavigateToAsync<MainViewModel>(this);
            }
            catch (Exception ex)
            {

                await DialogService.ShowAlertAsync("An error occured", "Search failure", "Try again");
                Debug.WriteLine($"[SearchAsync] Error searching in: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}