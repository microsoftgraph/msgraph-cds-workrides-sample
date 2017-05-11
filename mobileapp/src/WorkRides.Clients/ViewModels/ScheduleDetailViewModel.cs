using System.Threading.Tasks;
using CarPool.Clients.Core.ViewModels.Base;
using CarPool.Clients.Core.Services.Graph.Calendar;
using System;
using CarPool.Clients.Core.Models;
using System.Windows.Input;
using Xamarin.Forms;
using Carpool.Clients.Services.Dialog;
using System.Linq;
using CarPool.Clients.Core.Services.Graph.User;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using CarPool.Clients.Core.Maps.Model;
//using CarPool.Clients.Core.Services.PhoneCall;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Services.OpenMap;
using System.IO;

namespace CarPool.Clients.Core.ViewModels
{
    public class ScheduleDetailViewModel : ViewModelBase
    {
        private readonly ICalendarService _calendarService;
        private readonly IDialogService _dialogService;
        private readonly IDataService _dataService;
        private readonly IUserService _userService;
        private readonly IOpenMapService _openMapService;
        //private readonly IPhoneCallService _phoneCallService;

        private GraphEvent _event;
        private GraphUser _selectedItem;
        private ObservableCollection<GraphUser> _drivers;
        private ObservableCollection<CustomPin> _pushpinsList;
        private ObservableCollection<CustomRider> _ridersList;
        private DateTime _pickTime;
        private DateTime _arriveTime;
        private bool _isDriver;
        private bool _isPickup;

        public ScheduleDetailViewModel(
            ICalendarService calendarService,
            IDialogService dialogservice,
            IDataService dataService,
            IUserService userService,
            IOpenMapService openMapService)
        {
            _calendarService = calendarService;
            _dialogService = dialogservice;
            _dataService = dataService;
            _userService = userService;
            _openMapService = openMapService;

            //_phoneCallService = DependencyService.Get<IPhoneCallService>();
            _pushpinsList = new ObservableCollection<CustomPin>();
        }

        public ObservableCollection<GraphUser> Users
        {
            get
            {
                return _drivers;
            }

            set
            {
                _drivers = value;
                RaisePropertyChanged(() => Users);
            }
        }

        public GraphUser SelectedItem
        {
            get
            {
                return _selectedItem;
            }

            set
            {
                _selectedItem = value;

                if (_selectedItem != null)
                {
                    _selectedItem = null;
                }
                
                RaisePropertyChanged(() => SelectedItem);
            }
        }

        public DateTime PickTime
        {
            get
            {
                return _pickTime;
            }

            set
            {
                _pickTime = value;
                RaisePropertyChanged(() => PickTime);
            }
        }

        public DateTime ArriveTime
        {
            get
            {
                return _arriveTime;
            }

            set
            {
                _arriveTime = value;
                RaisePropertyChanged(() => ArriveTime);
            }
        }

        public bool IsDriver
        {
            get
            {
                return _isDriver;
            }

            set
            {
                _isDriver = value;
                RaisePropertyChanged(() => IsDriver);
            }
        }

        public bool IsPickup
        {
            get
            {
                return _isPickup;
            }

            set
            {
                _isPickup = value;
                RaisePropertyChanged(() => IsPickup);
            }
        }

        public ObservableCollection<CustomPin> PushpinsList
        {
            get { return _pushpinsList; }
            set
            {
                _pushpinsList = value;
                RaisePropertyChanged(() => PushpinsList);
            }
        }

        public ObservableCollection<CustomRider> RidersList
        {
            get
            {
                return _ridersList;
            }

            set
            {
                _ridersList = value;
                RaisePropertyChanged(() => RidersList);
            }
        }

        public async override Task InitializeAsync(object navigationData)
        {
            if (navigationData is GraphEvent)
            {
                IsBusy = true;

                try
                {
                    _event = navigationData as GraphEvent;

                    if (_event == null)
                    {
                        return;
                    }

                    PickTime = _event.StartDate;
                    ArriveTime = _event.EndDate;
                    IsPickup = _event.BodyPreview != null && _event.BodyPreview.Contains("Pickup");

                    if (_event.BodyPreview
                        .Contains(AppSettings.CarpoolEventBody))
                    {
                        int index = _event.BodyPreview.IndexOf(AppSettings.CarpoolEventBody);
                        var driverName = _event.BodyPreview
                            .Substring(index)
                            .Replace(AppSettings.CarpoolEventBody, string.Empty);
                        var result = await _dataService.GetAllDriversAsync();
                        var driver = result.FirstOrDefault(d => d.Name.Equals(driverName));
                        if (driver != null)
                        {
                            IsDriver = driver.Name.Equals(App.CurrentUser.UserPrincipalName);

                            _drivers = new ObservableCollection<GraphUser>();
                            var employees = await _dataService.GetAllEmployeesAsync();
                            var riders = new List<GraphUser>();
                            if (!string.IsNullOrEmpty(driver.Rider1) && driver.Rider1Status)
                            {
                                var rider1 = new GraphUser(await _userService.GetUserByDisplayNameAsync(driver.Rider1));
                                rider1.Merge(employees.FirstOrDefault(e => e.Name.Equals(driver.Rider1)));
                                _drivers.Add(rider1);
                            }
                            if (!string.IsNullOrEmpty(driver.Rider2) && driver.Rider2Status)
                            {
                                var rider2 = new GraphUser(await _userService.GetUserByDisplayNameAsync(driver.Rider2));
                                rider2.Merge(employees.FirstOrDefault(e => e.Name.Equals(driver.Rider2)));
                                _drivers.Add(rider2);
                            }
                            if (!string.IsNullOrEmpty(driver.Rider3) && driver.Rider3Status)
                            {
                                var rider3 = new GraphUser(await _userService.GetUserByDisplayNameAsync(driver.Rider3));
                                rider3.Merge(employees.FirstOrDefault(e => e.Name.Equals(driver.Rider3)));
                                _drivers.Add(rider3);
                            }
                            if (!string.IsNullOrEmpty(driver.Rider4) && driver.Rider4Status)
                            {
                                var rider4 = new GraphUser(await _userService.GetUserByDisplayNameAsync(driver.Rider4));
                                rider4.Merge(employees.FirstOrDefault(e => e.Name.Equals(driver.Rider4)));
                                _drivers.Add(rider4);
                            }

                            RidersList = CalculateRidePushpins();

                            // Add my position to the map

                            var graphdriverUser = new GraphUser(driver);
                            graphdriverUser.Merge(employees.FirstOrDefault(e => e.Email.Equals(driver.Name)));
                            PushpinsList = CalculateDrivePushpins(graphdriverUser);

                            _drivers.Insert(0, graphdriverUser);
                            Users = _drivers;

                            // Update user photo in background
                            var tasks = new List<Task>();
                            foreach (var user in _drivers)
                            {
                                tasks.Add(GetUserPhotoAsync(user));
                            }
                            await Task.WhenAll(tasks);
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
                        await _dialogService.ShowAlertAsync("CDS service currently unavailable, please try again.", "Service unavailable", "Ok");
                    }
                } 
                finally
                {
                    IsBusy = false;
                }

            }

            await base.InitializeAsync(navigationData);
        }

        public ICommand OpenMapCommand => new Command(async () => await OpenMapAsync());

        public ICommand CancelTripCommand => new Command(async () => await CancelTripAsync());

        public ICommand LeaveCarpoolCommand => new Command(async () => await LeaveCarpoolAsync());

        public ICommand SendEmailCommand => new Command(async (u) => await SendEmailAsync(u));

        //public ICommand CallPhoneCommand => new Command(async (u) => await CallPhoneAsync(u));


        private async Task OpenMapAsync()
        {
            if (Users == null || !Users.Any())
            {
                return;
            }

            var user = Users.FirstOrDefault();

            if (!string.IsNullOrEmpty(user.StreetAddress))
            {
                var userAddress = App.CurrentUser.StreetAddress;
                _openMapService.OpenMapAppWithRoute(userAddress, user.StreetAddress);
            }
            else
            {
                await _dialogService.ShowAlertAsync("User don't have address", "Oops", "Ok");
            }
        }

        private async Task SendEmailAsync(object param)
        {
            if (param is GraphUser)
            {
                var user = param as GraphUser;
                if (!string.IsNullOrEmpty(user.Mail))
                {
                    Xamarin.Forms.Device.OpenUri(new Uri($"mailto:{user.Mail}"));
                }
                else
                {
                    await _dialogService.ShowAlertAsync("User don't have mail", "Oops", "Ok");
                }
            }
        }

        private async Task GetUserPhotoAsync(GraphUser user)
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

        //private async Task CallPhoneAsync(object param)
        //{
        //    if (param is GraphUser)
        //    {
        //        var user = param as GraphUser;
        //        if (!string.IsNullOrEmpty(user.MobilePhone) || (user.BusinessPhones != null && user.BusinessPhones.Any()))
        //        {
        //            if (!string.IsNullOrEmpty(user.MobilePhone))
        //            {
        //                _phoneCallService.MakeCall(user.MobilePhone);
        //            }
        //            else
        //            {
        //                _phoneCallService.MakeCall(user.BusinessPhones.First());
        //            }
        //        }
        //        else
        //        {
        //            await _dialogservice.ShowAlertAsync("User don't have phone number", "Oops", "Ok");
        //        }
        //    }
        //}

        private async Task LeaveCarpoolAsync()
        {
            try
            {
                if (await _dialogService.ShowConfirmAsync(
                "This will remove you from all trips with this driver.",
                "Leave the carpool?",
                "Ok",
                "Cancel"))
                {
                    await _calendarService.CancelEventAsync(_event.Id);

                    await NavigationService.NavigateToAsync<MainViewModel>();
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
                    await _dialogService.ShowAlertAsync("Event was deleted previously", "Event canceled", "Ok");
                }
            }
        }

        private async Task CancelTripAsync()
        {
            if (_event == null)
            {
                return;
            }

            try
            {
                if (await _dialogService.ShowConfirmAsync(
                "This will remove you from today's carpool",
                "Cancel this trip?",
                "Ok",
                "Cancel"))
                {
                    await _calendarService.DeleteEventAsync(_event.Id);

                    await NavigationService.NavigateToAsync<MainViewModel>();
                }
            } catch
            {
                await _dialogService.ShowAlertAsync("Event was deleted previously", "Event canceled", "Ok");
            }
        }

        private ObservableCollection<CustomPin> CalculateDrivePushpins(GraphUser driver)
        {
            ObservableCollection<CustomPin> pins = new ObservableCollection<CustomPin>();

            if (driver.HasLocation())
            {
                pins.Add(driver.CreateUserPin());
            }

            if (driver.HasWorkLocation())
            {
                pins.Add(driver.CreateWorkPin());
            }

            return pins;
        }

        private ObservableCollection<CustomRider> CalculateRidePushpins()
        {
            ObservableCollection<CustomRider> riders = new ObservableCollection<CustomRider>();

            if (_drivers.Any())
            {
                for (int i= 0; i < _drivers.Count; i ++)
                {
                    var rider = _drivers[i];
                    if (rider.HasLocation())
                    {
                        riders.Add(rider.CreateRidePin());
                    }
                }
            }

            return riders;
        }
    }
}