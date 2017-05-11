using System.Threading.Tasks;
using CarPool.Clients.Core.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Maps.Model;
using System.Linq;
using System.Collections.Generic;
using CarPool.Clients.Core.Models;
using Xamarin.Forms.Maps;
using System.Collections.ObjectModel;
using CarPool.Clients.Core.Services.Graph.Driver;
using CarPool.Clients.Core.Services.Data;
using System;
using Carpool.Clients.Services.Dialog;
using CarPool.Clients.Core.Services.Graph.Mail;

namespace CarPool.Clients.Core.ViewModels
{
    public class InspectRiderRequestViewModel : ViewModelBase
    {
        private readonly IDriverService _driverService;
        private readonly IDataService _dataService;
        private readonly IDialogService _dialogService;
        private readonly IMailService _mailService;

        private GraphUser _user;

        private ObservableCollection<RouteModel> _routes;

        private ObservableCollection<CustomPin> _pushpins;

        private ObservableCollection<CustomRider> _riderPins;

        private double _addedMinutes;

        private RidePeriod? _ridePeriod;

        public InspectRiderRequestViewModel(
            IDriverService driverService,
            IMailService mailService,
            IDataService dataService,
            IDialogService dialogService)
        {
            _driverService = driverService;
            _dataService = dataService;
            _dialogService = dialogService;
            _mailService = mailService;
            _routes = new ObservableCollection<RouteModel>();
            _pushpins = new ObservableCollection<CustomPin>();
            _riderPins = new ObservableCollection<CustomRider>();
        }

        public double Distance { get; set; }

        public GraphUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public RidePeriod? RequestedRidePeriod
        {
            get { return _ridePeriod; }
            set
            {
                _ridePeriod = value;
                RaisePropertyChanged(() => RequestedRidePeriod);
            }
        }

        public ObservableCollection<RouteModel> RoutesList
        {
            get
            {
                return _routes;
            }

            set
            {
                _routes = value;
                RaisePropertyChanged(() => RoutesList);
            }
        }

        public ObservableCollection<CustomPin> PushpinsList
        {
            get
            {
                return _pushpins;
            }

            set
            {
                _pushpins = value;
                RaisePropertyChanged(() => PushpinsList);
            }
        }

        public ObservableCollection<CustomRider> RidersList
        {
            get
            {
                return _riderPins;
            }

            set
            {
                _riderPins = value;
                RaisePropertyChanged(() => RidersList);
            }
        }

        public double AddedMinutes
        {
            get
            {
                return _addedMinutes;
            }

            set
            {
                _addedMinutes = value;
                RaisePropertyChanged(() => AddedMinutes);
            }
        }

        public ICommand SendEmailCommand => new Command(async () => await SendEmailAsync());

        public ICommand RejectRiderCommand => new Command(async () => await RejectRiderAsync());

        public async override Task InitializeAsync(object navigationData)
        {
            IsBusy = true;

            if (navigationData is GraphUser)
            {
                User = (GraphUser)navigationData;
            }

            try
            {
                var driver = (await _dataService.GetAllDriversAsync()).FirstOrDefault(r => App.CurrentUser.UserPrincipalName.Equals(r.Name));

                // Check join period from my mail inbox
                RequestedRidePeriod = GetRequestedRidePeriod(driver);
                _pushpins = InitDrivePushpins();

                var waypoints = (await _driverService.GetMyRidersAsync(App.CurrentUser)).ToList();
                await FetchWaypointsLocation(waypoints);

                // Calculate the actual route
                var oldRoute = InitRoute(waypoints, Color.Black);
                oldRoute.RouteCalculationCompleted += (a, s) =>
                {
                    _addedMinutes = -oldRoute.Duration;
                    Distance = oldRoute.Distance;
                };

                // Add new user request to the map 
                RidersList = InitDriveRiders(waypoints);
                waypoints.Add(User);

                // Calculate route for the new request
                var newRoute = InitRoute(waypoints, Color.Blue);
                newRoute.RouteCalculationCompleted += (a, s) =>
                {
                    AddedMinutes = _addedMinutes + newRoute.Duration;
                    var newUserPin = User.CreateUserPin();
                    newUserPin.Duration = AddedMinutes;
                    _pushpins.Add(newUserPin);
                    PushpinsList = _pushpins;
                };
                RoutesList = new ObservableCollection<RouteModel>() { oldRoute, newRoute };

                await base.InitializeAsync(navigationData);
            }
            catch
            {

                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    await _dialogService.ShowAlertAsync("CDS service currently unavailable, please check your internet connection.", "Service unavailable", "Ok");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task FetchWaypointsLocation(List<GraphUser> waypoints)
        {
            var employees = await _dataService.GetAllEmployeesAsync();
            foreach (var point in waypoints)
            {
                point.Merge(employees.FirstOrDefault(e => e.Email.Equals(point.Mail)));
            }
        }

        private async Task RejectRiderAsync()
        {

            if (await _dialogService.ShowConfirmAsync(
                "This will remove the rider request from this trip.",
                "Reject the rider?",
                "Ok",
                "Cancel"))
            {
                IsBusy = true;

                try
                {
                    var drivers = await _dataService.GetAllDriversAsync();
                    var driver = drivers.FirstOrDefault(r => r.Name.Equals(App.CurrentUser.UserPrincipalName,
                        StringComparison.CurrentCultureIgnoreCase));

                    if (driver != null)
                    {
                        await _dataService.RejectRiderAsync(driver, User.DisplayName);
                        CurrentUser.RiderRequests -= 1;
                        MessagingCenter.Send(CurrentUser, MessengerKeys.UserReviewRiderRequest);
                        await NavigationService.NavigateToAsync<DriveViewModel>();
                    }
                    else
                    {
                        await _dialogService.ShowAlertAsync("Trip has been removed", "Oops", "Ok");
                    }
                } 
                catch
                {
                    await _dialogService.ShowAlertAsync("CDS service currently unavailable, please check your internet connection.", "Service unavailable", "Ok");
                }
                finally
                {
                    IsBusy = false;
                }
            }
        }

        private RidePeriod GetRequestedRidePeriod(Driver driver)
        {
            if (!string.IsNullOrEmpty(driver?.Schedule))
            {
                switch (driver.Schedule) {
                    case "today":
                        return RidePeriod.Today;
                    case "every day:":
                        return RidePeriod.EveryDay;
                }
            }

            return RidePeriod.Tomorrow;
        }

        private async Task SendEmailAsync()
        {
            await NavigationService.NavigateToAsync<NewMessageViewModel>(this);
        }

        private RouteModel InitRoute(IEnumerable<GraphUser> users, Color routeColor)
        {
            List<Position> waypoints = new List<Position>();
            foreach (var user in users)
            {
                if (user.HasLocation())
                {
                    waypoints.Add(user.Location().Value);
                }
            }

            return new RouteModel()
            {
                From = new Position(App.CurrentUser.Latitude.Value, App.CurrentUser.Longitude.Value),
                To = new Position(App.CurrentUser.WorkLatitude.Value, App.CurrentUser.WorkLongitude.Value),
                WayPoints = waypoints,
                Id = waypoints.Count,
                Color = routeColor
            };
        }

        private ObservableCollection<CustomPin> InitDrivePushpins()
        {
            ObservableCollection<CustomPin> pins = new ObservableCollection<CustomPin>();

            if (App.CurrentUser.HasLocation())
            {
                pins.Add(App.CurrentUser.CreateUserPin());
            }

            if (App.CurrentUser.HasWorkLocation())
            {
                pins.Add(App.CurrentUser.CreateWorkPin());
            }

            return pins;
        }

        private ObservableCollection<CustomRider> InitDriveRiders(IEnumerable<GraphUser> users)
        {
            ObservableCollection<CustomRider> pins = new ObservableCollection<CustomRider>();

            if (users.Any())
            {
                foreach (var rider in users)
                {
                    if (rider.HasLocation())
                    {
                        pins.Add(rider.CreateRidePin());
                    }
                }
            }

            return pins;
        }
    }
}