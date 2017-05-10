using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Linq;
using System.Windows.Input;
using CarPool.Clients.Core.Services.Graph.Driver;
using System;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Helpers;
using System.IO;
using CarPool.Clients.Core.Services.Graph.User;
using Carpool.Clients.Services.Dialog;

namespace CarPool.Clients.Core.ViewModels
{
    public class DriveViewModel : ViewModelBase
    {
        private ObservableCollection<CustomPin> _pushpinsList;
        private ObservableCollection<CustomRider> _ridersList;
        private ObservableCollection<GraphUser> _myRiders;
        private ObservableCollection<GraphUser> _myRiderRequests;
        private bool _riderChecked;
        private bool _riderRequestsChecked;

        private readonly IDataService _dataService;
        private readonly IUserService _userService;
        private readonly IDialogService _dialogService;

        public DriveViewModel(
            IDialogService dialogService,
            IDataService dataService,
            IUserService userService)
        {
            _dataService = dataService;
            _userService = userService;
            _dialogService = dialogService;

            _pushpinsList = new ObservableCollection<CustomPin>();
            _ridersList = new ObservableCollection<CustomRider>();
            RiderChecked = true;
            RiderRequestsChecked = false;
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
            get { return _ridersList; }
            set
            {
                _ridersList = value;
                RaisePropertyChanged(() => RidersList);
            }
        }

        public bool IsDriverEnabled
        {
            get
            {
                return Settings.DriverEnabled;
            }
        }

        public ObservableCollection<GraphUser> MyRiders
        {
            get
            {
                return _myRiders;
            }

            set
            {
                _myRiders = value;
                RaisePropertyChanged(() => MyRiders);
            }
        }

        public ObservableCollection<GraphUser> MyRiderRequests
        {
            get
            {
                return _myRiderRequests;
            }

            set
            {
                _myRiderRequests = value;
                RaisePropertyChanged(() => MyRiderRequests);
            }
        }

        public bool RiderChecked
        {
            get
            {
                return _riderChecked;
            }

            set
            {
                _riderChecked = value;
                RaisePropertyChanged(() => RiderChecked);
            }
        }

        public bool RiderRequestsChecked
        {
            get
            {
                return _riderRequestsChecked;
            }

            set
            {
                _riderRequestsChecked = value;
                RaisePropertyChanged(() => RiderRequestsChecked);
            }
        }

        public ICommand SelectorCommand => new Command<string>(Selector);

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync());

        public ICommand EditSettingsCommand => new Command(async () => await EditSettingsAsync());

        public ICommand InspectRiderRequestCommand => new Command<GraphUser>(async (u) => await InspectRiderRequestAsync(u));


        public async override Task InitializeAsync(object navigationData)
        {
            await RefreshAsync();
        }

        private async Task EditSettingsAsync()
        {
            await NavigationService.NavigateToAsync<ProfileViewModel>(this);
        }

        private async Task RefreshAsync()
        {
            IsBusy = true;

            try
            {
                var employees = await _dataService.GetAllEmployeesAsync();
                var driver = (await _dataService.GetAllDriversAsync()).FirstOrDefault(d => d.Name.Equals(App.CurrentUser.UserPrincipalName));
            
                var riders = DriverHelper.GetRiders(driver);
                var myRiders = new Collection<GraphUser>();

                foreach (var rider in riders)
                {
                    var graphRider = new GraphUser(await _userService.GetUserByDisplayNameAsync(rider));
                    graphRider.Merge(employees.FirstOrDefault(e => e.Name.Equals(rider)));
                    myRiders.Add(graphRider);
                    graphRider.Distance = UserHelper.CalculateDistance(App.CurrentUser, graphRider);
                }

                MyRiders = new ObservableCollection<GraphUser>(myRiders);

                var riderRequests = DriverHelper.GetRiderRequests(driver);
                var myRiderRequests = new Collection<GraphUser>();

                foreach (var rider in riderRequests)
                {
                    var graphRider = new GraphUser(await _userService.GetUserByDisplayNameAsync(rider));
                    myRiderRequests.Add(graphRider);
                    graphRider.Merge(employees.FirstOrDefault(e => e.Name.Equals(rider)));
                    graphRider.Distance = UserHelper.CalculateDistance(App.CurrentUser, graphRider);
                }

                MyRiderRequests = new ObservableCollection<GraphUser>(myRiderRequests);

                // Add my pushpin to the map
                PushpinsList = CalculateDrivePushpins();
                RidersList = CalculateRiderPushpins();

                IsBusy = false;

                // background photo requests
                foreach (var rider in myRiders)
                {
                    await GetUserPhotoAsync(rider);
                }
                foreach (var rider in myRiderRequests)
                {
                    await GetUserPhotoAsync(rider);
                }
            }
            catch
            {
                IsBusy = false;
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    await DialogService.ShowAlertAsync("CDS service currently unavailable, please try again", "Service unavailable", "Try again");
                }
            }
        }

        private ObservableCollection<CustomPin> CalculateDrivePushpins()
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

        private ObservableCollection<CustomRider> CalculateRiderPushpins()
        {
            ObservableCollection<CustomRider> pins = new ObservableCollection<CustomRider>();

            if (MyRiders.Any())
            {
                foreach (var rider in MyRiders)
                {
                    if (rider.HasLocation())
                    {
                        pins.Add(rider.CreateRidePin());
                    }
                }
            }

            return pins;
        }

        private async Task InspectRiderRequestAsync(GraphUser user)
        {
            await NavigationService.NavigateToAsync<InspectRiderRequestViewModel>(user);
        }

        private void Selector(string parameter)
        {
            RiderRequestsChecked = false;
            RiderChecked = false;

            if (parameter.Equals("Riders", StringComparison.CurrentCultureIgnoreCase))
            {
                RiderChecked = true;
            }
            else
            {
                RiderRequestsChecked = true;
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
    }
}