using System.Threading.Tasks;
using CarPool.Clients.Core.ViewModels.Base;
using System.Windows.Input;
using System.Linq;
using Xamarin.Forms;
using System.Collections.ObjectModel;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Extensions;
using CarPool.Clients.Core.Services.Graph.User;
using CarPool.Clients.Core.Models;
using System;
using CarPool.Clients.Core.Services.Data;
using System.IO;
using CarPool.Clients.Core.Services.Graph.People;
using System.Collections.Generic;
using Carpool.Clients.Services.Dialog;

namespace CarPool.Clients.Core.ViewModels
{
    public class FindRideViewModel : ViewModelBase
    {
        private ObservableCollection<GraphUser> _people;

        private readonly IUserService _userService;
        private readonly IDataService _dataService;
        private readonly IPeopleService _peopleService;
        private readonly IDialogService _dialogService;

        public FindRideViewModel(
            IUserService userService,
            IDialogService dialogService,
            IPeopleService peopleService,
            IDataService dataService)
        {
            _userService = userService;
            _peopleService = peopleService;
            _dataService = dataService;
            _dialogService = dialogService;
        }

        public ObservableCollection<GraphUser> People
        {
            get { return _people; }
            set
            {
                _people = value;
                RaisePropertyChanged(() => People);
            }
        }

        public ICommand PeopleSelectedCommand => new Command<GraphUser>(async (u) => await OnSelectUserAsync(u));

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync());

        public ICommand EditRideSettingsCommand => new Command(async () => await EditRideSettingsAsync());

        public ICommand EditSettingsCommand => new Command(async () => await EditSettingsAsync());
        

        public override async Task InitializeAsync(object navigationData)
        {
            IsBusy = true;
            await RefreshAsync();
        }

        private async Task EditSettingsAsync()
        {
            await NavigationService.NavigateToAsync<ProfileViewModel>();
        }

        private async Task RefreshAsync()
        {
            var currentDrivers = new ObservableCollection<GraphUser>();

            try
            {
                // Fetch driver from database
                var drivers = ((await _dataService.GetAllDriversAsync())
                    .Where(d => !App.CurrentUser.UserPrincipalName.Equals(d.Name)));

                // Filter driver list from who are part of my same department
                var myDepartmentDrivers = await _userService.FilterMyDeparmentUsersAsync(App.CurrentUser.Department, drivers);

                // Filter drivers who are part of the People API called on the signed in user principal 
                var myPeopleDrivers = await _peopleService.FilterMyNearPeople(drivers);

                // Union list
                var myDrivers = myDepartmentDrivers;

                foreach (var driver in myPeopleDrivers)
                {
                    if (!myDrivers.Contains(driver))
                    {
                        myDrivers.Add(driver);
                    }
                }

                foreach (var driver in myDrivers)
                {
                    if (!Settings.DriverEnabled && !App.CurrentUser.ArrivalTime.Date.Equals(default(DateTime).Date))
                    {
                        // Filter based on the time range from my selection
                        if ((driver.Arrival.TimeOfDay <= App.CurrentUser.ArrivalTime.AddMinutes(15).TimeOfDay &&
                            driver.Arrival.TimeOfDay >= App.CurrentUser.ArrivalTime.AddMinutes(-15).TimeOfDay) ||
                            (driver.Departure.TimeOfDay <= App.CurrentUser.DepartureTime.AddMinutes(15).TimeOfDay &&
                            driver.Departure.TimeOfDay >= App.CurrentUser.DepartureTime.AddMinutes(-15).TimeOfDay))
                        {
                            AddDriverToList(new GraphUser(driver), currentDrivers);
                        }
                    }
                    else
                    {
                        AddDriverToList(new GraphUser(driver), currentDrivers);
                    }
                }
            }
            catch
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    await DialogService.ShowAlertAsync("An error occured", "Unexpected error", "Try again");
                }
            }

            People = currentDrivers.OrderBy(u => u.Distance.Value)
                .ToObservableCollection();

            IsBusy = false;

            if (currentDrivers != null && currentDrivers.Any())
            {
                // Calculate OOF in background
                await _userService.GetOutOfOfficeAsync(currentDrivers);

                // Update user photo in background
                var tasks = new List<Task>();
                foreach (var user in currentDrivers)
                {
                    tasks.Add(UpdateUserPhotoAsync(user));
                }
                await Task.WhenAny(tasks);
            }
        }

        private void AddDriverToList(GraphUser driver, ObservableCollection<GraphUser> drivers)
        {
            driver.Distance = UserHelper.CalculateDistance(App.CurrentUser, driver);

            if (driver.Distance.HasValue && !driver.Distance.Value.Equals(Double.NaN))
            {
                drivers.Add(driver);
            }
        }

        private async Task UpdateUserPhotoAsync(GraphUser user)
        {
            try
            {
                var photo = await _userService.GetUserPhotoAsync(user.Mail);

                if (photo != null)
                {
                    user.PhotoStream = ImageSource.FromStream(() => new MemoryStream(photo));
                }
            }
            catch
            {
                user.PhotoStream = null;
            }
        }

        private async Task OnSelectUserAsync(GraphUser user)
        {
            // Merge CDS data with Graph data
            user.Merge(await _userService.GetUserByPrincipalNameAsync(user.UserPrincipalName));
            await NavigationService.NavigateToAsync<FindRideDetailViewModel>(user);
        }

        private async Task EditRideSettingsAsync()
        {
            await NavigationService.NavigateToAsync<RidePreferencesViewModel>();
        }
    }
}