using Carpool.Clients.Services.Dialog;
using Carpool.Clients.ViewModels.Base;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Services.Graph;
using CarPool.Clients.Core.Services.Graph.Organization;
using CarPool.Clients.Core.Services.Graph.User;
using CarPool.Clients.Core.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.ViewModels
{
    public class LoginViewModel : ViewModelBase
    {
        private readonly IDataService _dataService;

        private readonly IUserService _userService;

        private readonly IDialogService _dialogService;

        private readonly IOrganizationService _organizationService;

        private readonly UserHelper _userHelper;

        public LoginViewModel(
            IDataService dataService,
            IUserService userService,
            IOrganizationService organizationService,
            IDialogService dialogService,
            UserHelper userHelper)
        {
            _dataService = dataService;
            _userService = userService;
            _userHelper = userHelper;
            _organizationService = organizationService;
            _dialogService = dialogService;

            MessagingCenter.Subscribe<GraphUser>(this, MessengerKeys.UserChangedTimeRange, 
                UpdateEmployeeTimeRange);
            MessagingCenter.Subscribe<GraphUser>(this, MessengerKeys.DriverChangedTimeRange,
                UpdateDriverTimeRange);
            MessagingCenter.Subscribe<GraphUser>(this, MessengerKeys.UserChangedHomeAddress,
                UserChangedHomeAddress);
            MessagingCenter.Subscribe<GraphUser>(this, MessengerKeys.UserChangedWorkAddress,
                UserChangedWorkAddress);
        }

        public ICommand SignInCommand => new Command(async () => await SignInAsync());

        public ICommand AppSettingsCommand => new Command(async () => await AppSettingsAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            
            if (GraphClient.Instance.IsAuthenticated())
            {
                await SignInAsync();
            }
        }

        private async Task SignInAsync()
        {
            try
            {
                IsBusy = true;

                App.CurrentUser = new GraphUser(await GraphClient
                 .Instance
                 .Authenticate());

                if (App.CurrentUser != null)
                {
                    await _dataService.InitializeAsync();

                    var employee = (await _dataService.GetAllEmployeesAsync())                     
                        .FirstOrDefault(u => u.Email.Equals(App.CurrentUser.Mail));

                    if (employee == null)
                    {
                        // Employee first Sign in, new entity in CDS
                        await InsertNewEmployeeAsync();

                        // Employee sign in flow
                        await NavigationService.NavigateToAsync<UserPreferencesViewModel>();
                    }
                    else
                    {
                        var hasDeltaChanges = await UpdateSettingsAsync(App.CurrentUser, employee);
                        App.CurrentUser.Merge(employee);

                        if (hasDeltaChanges)
                        {
                            await Task.WhenAny(new List<Task>() {
                                _dialogService.ShowAlertAsync("Your profile has changed, please review it.", "Profile changes detected", "Ok"),
                                NavigationService.NavigateToAsync<MainViewModel>(ViewModelLocator.Instance.Resolve<ProfileViewModel>())
                            });
                        }
                        else if (App.CurrentUser.GivenName.Equals(AppSettings.DefaultDemoUser,
                            StringComparison.CurrentCultureIgnoreCase))
                        {
                            await NavigationService.NavigateToAsync<UserPreferencesViewModel>();
                        }
                        else
                        {
                            await NavigationService.NavigateToAsync<MainViewModel>(this);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                GraphClient.Instance.SignOut();

                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    Debug.WriteLine($"[SignIn] Error signing in: {ex}");
                    await DialogService.ShowAlertAsync("An error occured", "Login failure", "Try again");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task AppSettingsAsync()
        {
            await NavigationService.NavigateToAsync<DataSettingsViewModel>();
        }

        private async Task InsertNewEmployeeAsync()
        {
            // Fetch user work geoposition from graph extensions
            App.CurrentUser.WorkAddress = await GetWorkAddress();
            await UserHelper.GetUserWorkLocation(App.CurrentUser);

            // Fetch user home geoposition from AD
            await UserHelper.GetUserHomeLocation(App.CurrentUser);

            // Persist CDS entity
            var employee = new Employee
            {
                Name = App.CurrentUser.DisplayName,
                Email = string.IsNullOrEmpty(App.CurrentUser.Mail)? App.CurrentUser.UserPrincipalName : App.CurrentUser.Mail,
                PhoneNo = App.CurrentUser.BusinessPhones?.FirstOrDefault(),
                Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                HomeAddress = App.CurrentUser.HomeAddress,
                Latitude = App.CurrentUser.Latitude.HasValue ? App.CurrentUser.Latitude.Value : default(double),
                Longitude = App.CurrentUser.Longitude.HasValue ? App.CurrentUser.Longitude.Value : default(double),
                WorkAddress = App.CurrentUser.WorkAddress,
                WorkLatitude = App.CurrentUser.WorkLatitude.HasValue ? App.CurrentUser.WorkLatitude.Value : default(double),
                WorkLongitude = App.CurrentUser.WorkLongitude.HasValue ? App.CurrentUser.WorkLongitude.Value : default(double)
            };
            
            await _dataService.InsertOrUpdateEmployeeAsync(employee);
        }

        private async Task<bool> UpdateSettingsAsync(GraphUser user, Employee employee)
        {
            var drivers = await _dataService.GetAllDriversAsync();
            var driver = drivers.FirstOrDefault(d =>  
            !string.IsNullOrEmpty(d.Name) && d.Name.Equals(user.UserPrincipalName));

            if (driver != null)
            {
                App.CurrentUser.ArrivalTime = driver.Arrival;
                App.CurrentUser.DepartureTime = driver.Departure;
                App.CurrentUser.RiderRequests = DriverHelper.GetRiderRequests(driver).Count();
                Settings.DriverEnabled = true;
            }
            else
            {
                Settings.DriverEnabled = false;
            }

            // Any changes since last login? delta query
            if (string.IsNullOrEmpty(Settings.DeltaLink))
            {
                // First login, fetch actual delta state
                Settings.DeltaLink = await _userService.InitializeDeltaQueryAsync();
            }
            else
            {
                // Check changes from last login
                var userChanges = await _userService.GetDeltaQueryChangesAsync(Settings.DeltaLink);
                return await _userHelper.UpdateProfilesChanges(userChanges, employee);
            }

            return false;
        }

        private async void UpdateEmployeeTimeRange(GraphUser user)
        {
            try
            {
                if (user == null)
                {
                    return;
                }

                var employee = (await _dataService.GetAllEmployeesAsync())
                    .FirstOrDefault(u => u.Email.Equals(user.Mail));

                if (employee != null)
                {
                    employee.Arrival = user.ArrivalTime;
                    employee.Departure = user.DepartureTime;
                    await _dataService.InsertOrUpdateEmployeeAsync(employee);
                }
            }
            catch(Exception ex)
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                Debug.WriteLine($"[UpdateEmployeeTimeRange] Error updating in: {ex}");
            }
        }

        private async void UpdateDriverTimeRange(GraphUser user)
        {
            try
            {
                if (user == null)
                {
                    return;
                }

                var driver = (await _dataService.GetAllDriversAsync())
                    .FirstOrDefault(u => u.Name.Equals(user.UserPrincipalName));

                if (driver != null)
                {
                    driver.Arrival = user.ArrivalTime;
                    driver.Departure = user.DepartureTime;
                    await _dataService.InsertOrUpdateDriverAsync(driver);
                }
            }
            catch (Exception ex)
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                Debug.WriteLine($"[UpdateEmployeeTimeRange] Error updating in: {ex}");
            }
        }

        private async void UserChangedHomeAddress(GraphUser user)
        {
            try
            {
                if (user == null)
                {
                    return;
                }

                var employee = (await _dataService.GetAllEmployeesAsync())
                    .FirstOrDefault(e => e.Email.Equals(user.UserPrincipalName));

                if (employee != null)
                {
                    employee.HomeAddress = user.HomeAddress;
                    employee.Latitude = user.Latitude.HasValue ? user.Latitude.Value : default(double);
                    employee.Longitude = user.Longitude.HasValue ? user.Longitude.Value : default(double);
                    await _dataService.InsertOrUpdateEmployeeAsync(employee);
                }

                if (Settings.DriverEnabled)
                {
                    var driver = (await _dataService.GetAllDriversAsync())
                    .FirstOrDefault(e => e.Name.Equals(user.UserPrincipalName));

                    driver.Latitude = user.Latitude.HasValue ? user.Latitude.Value : default(double);
                    driver.Longitude = user.Longitude.HasValue ? user.Longitude.Value : default(double);
                    await _dataService.InsertOrUpdateDriverAsync(driver);
                }
            }
            catch (Exception ex)
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                Debug.WriteLine($"[UserChangedHomeAddress] Error updating in: {ex}");
            }
        }

        private async void UserChangedWorkAddress(GraphUser user)
        {
            try
            {
                if (user == null)
                {
                    return;
                }

                var employee = (await _dataService.GetAllEmployeesAsync())
                    .FirstOrDefault(e => e.Email.Equals(user.UserPrincipalName));

                if (employee != null)
                {
                    employee.WorkAddress = user.WorkAddress;
                    employee.WorkLatitude = user.WorkLatitude.HasValue ? user.WorkLatitude.Value : default(double);
                    employee.WorkLongitude = user.WorkLongitude.HasValue ? user.WorkLongitude.Value : default(double);
                    await _dataService.InsertOrUpdateEmployeeAsync(employee);
                }
            }
            catch (Exception ex)
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                Debug.WriteLine($"[UserChangedHomeAddress] Error updating in: {ex}");
            }
        }

        private async Task<string> GetWorkAddress()
        {
            await Task.Delay(0);

            string workAddress = AppSettings.DefaultWorkAddress;

            //var extensions = await _userService.GetUserExtensionsAsync(App.CurrentUser.UserPrincipalName);

            //if (extensions != null
            //    && extensions.ContainsKey(AppSettings.WorkAddressGraphExtensionKey))
            //{
            //    extensions.TryGetValue(AppSettings.WorkAddressGraphExtensionKey, out workAddress);
            //}

            return workAddress;
        }
    }
}