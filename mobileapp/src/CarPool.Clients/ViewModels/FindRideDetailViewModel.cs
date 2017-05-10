using System.Threading.Tasks;
using CarPool.Clients.Core.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Models;
using System;
using CarPool.Clients.Core.Helpers;
using Carpool.Clients.Services.Dialog;
using CarPool.Clients.Core.Services.Data;
using System.Linq;

namespace CarPool.Clients.Core.ViewModels
{
    public class FindRideDetailViewModel : ViewModelBase
    {
        private GraphUser _user;

        private DateTime _estimatedPickup;

        private DateTime _estimatedArrival;

        private DateTime _estimatedDropoff;

        private DateTime _estimatedDeparture;

        private readonly IDialogService _dialogService;

        private readonly IDataService _dataService;

        public FindRideDetailViewModel(
            IDialogService dialogService,
            IDataService dataService)
        {
            _dialogService = dialogService;
            _dataService = dataService;
        }

        public GraphUser User
        {
            get { return _user; }
            set
            {
                _user = value;
                RaisePropertyChanged(() => User);
            }
        }

        public DateTime EstimatedPickup
        {
            get
            {
                return _estimatedPickup;
            }

            set
            {
                _estimatedPickup = value;
                RaisePropertyChanged(() => EstimatedPickup);
            }
        }

        public DateTime EstimatedArrival
        {
            get
            {
                return _estimatedArrival;
            }

            set
            {
                _estimatedArrival = value;
                RaisePropertyChanged(() => EstimatedArrival);
            }
        }

        public DateTime EstimatedDropoff
        {
            get
            {
                return _estimatedDropoff;
            }

            set
            {
                _estimatedDropoff = value;
                RaisePropertyChanged(() => EstimatedDropoff);
            }
        }

        public DateTime EstimatedDeparture
        {
            get
            {
                return _estimatedDeparture;
            }

            set
            {
                _estimatedDeparture = value;
                RaisePropertyChanged(() => EstimatedDeparture);
            }
        }

        public ICommand SendEmailCommand => new Command(async () => await SendEmailAsync());

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is GraphUser)
            {
                User = (GraphUser)navigationData;

                if (CurrentUser != null)
                {
                    var tripTime = UserHelper.GetTripTimeEstimationAsync(App.CurrentUser.Location(), App.CurrentUser.WorkLocation());
                    if (!CurrentUser.ArrivalTime.Date.Equals(default(DateTime).Date))
                    {
                        EstimatedPickup = CurrentUser.ArrivalTime.AddMinutes(-tripTime);
                        EstimatedArrival = CurrentUser.ArrivalTime;
                        EstimatedDeparture = CurrentUser.DepartureTime;
                        EstimatedDropoff = CurrentUser.DepartureTime.AddMinutes(tripTime);
                    }
                    else
                    {
                        EstimatedPickup = User.ArrivalTime.AddMinutes(-tripTime);
                        EstimatedArrival = User.ArrivalTime;
                        EstimatedDeparture = User.DepartureTime;
                        EstimatedDropoff = User.DepartureTime.AddMinutes(tripTime);
                    }
                }
            }

            return base.InitializeAsync(navigationData);
        }

        private async Task SendEmailAsync()
        {
            if (User.OOF)
            {
                await _dialogService.ShowAlertAsync($"{User.DisplayName} is currently out of office and cannot accepts riders.", "Out of office", "Ok");
            }
            else
            {
                try
                {
                    var driver = (await _dataService.GetAllDriversAsync())
                                 .FirstOrDefault(d => User.UserPrincipalName.Equals(d.Name));

                    if (driver != null && (
                       App.CurrentUser.DisplayName.Equals(driver.Rider1) ||
                       App.CurrentUser.DisplayName.Equals(driver.Rider2) ||
                       App.CurrentUser.DisplayName.Equals(driver.Rider3) ||
                       App.CurrentUser.DisplayName.Equals(driver.Rider4)))
                    {
                        await _dialogService.ShowAlertAsync("You already are riding this carpool.", "Already riding", "Ok");
                    }
                    else if (driver != null &&
                       !string.IsNullOrEmpty(driver.Rider1) &&
                       !string.IsNullOrEmpty(driver.Rider2) &&
                       !string.IsNullOrEmpty(driver.Rider3) &&
                       !string.IsNullOrEmpty(driver.Rider4))
                    {
                        await _dialogService.ShowAlertAsync("This carpool is full.", "Carpool full", "Ok");
                    }
                    else
                    {
                        await NavigationService.NavigateToAsync<NewMessageViewModel>(User);
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
                        await DialogService.ShowAlertAsync("CDS service currently unavailable, please try again", "Service unavailable", "Try again");
                    }
                }
            }
        }
    }
}