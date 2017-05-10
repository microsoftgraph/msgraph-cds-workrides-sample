using System.Linq;
using CarPool.Clients.Core.Services.Graph.Mail;
using CarPool.Clients.Core.ViewModels.Base;
using Microsoft.Graph;
using System;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Graph.Calendar;
using System.Collections.Generic;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Helpers;
using Carpool.Clients.Services.Dialog;
using Carpool.Clients.ViewModels.Base;
using System.Collections.ObjectModel;
using CarPool.Clients.Core.Maps.Model;

namespace CarPool.Clients.Core.ViewModels
{
    public class NewMessageViewModel : ViewModelBase
    {
        private GraphUser _user;
        private string _message;
        private string _subject;
        private double _distance;
        private ObservableCollection<CustomPin> _route;
        private NewMessageType _newMessageType;
        private RidePeriod _ridePeriod;

        private readonly IMailService _emailService;
        private readonly IDataService _dataService;
        private readonly ICalendarService _calendarService;
        private readonly IDialogService _dialogService;

        public NewMessageViewModel(
            IMailService emailService,
            IDataService dataService,
            IDialogService dialogService,
            ICalendarService calendarService)
        {
            _emailService = emailService;
            _dataService = dataService;
            _calendarService = calendarService;
            _dialogService = dialogService;

            _newMessageType = NewMessageType.Invite;
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

        public string Message
        {
            get { return _message; }
            set
            {
                _message = value;
                RaisePropertyChanged(() => Message);
            }
        }

        public string Subject
        {
            get
            {
                return _subject;
            }

            set
            {
                _subject = value;
                RaisePropertyChanged(() => Subject);
            }
        }

        public ICommand SendEmailCommand => new Command(async () => await SendEmailAsync());

        public override Task InitializeAsync(object navigationData)
        {
            if (navigationData is GraphUser)
            {
                _newMessageType = NewMessageType.Invite;
                User = (GraphUser)navigationData;
                Subject = $"May I ride with you {GetRidePeriod(App.CurrentUser.RidePeriod)}?";
                Message =
                    $"Hi {User.GivenName}," +
                    Environment.NewLine + Environment.NewLine +
                    $"May I ride with you {GetRidePeriod(App.CurrentUser.RidePeriod)}?" +
                    Environment.NewLine +
                    "The WorkRides app thinks we're a good match!" +
                    Environment.NewLine + Environment.NewLine +
                    "Thank you," +
                    Environment.NewLine +
                    $"{App.CurrentUser.GivenName}";
            }
            else if (navigationData is InspectRiderRequestViewModel)
            {
                _newMessageType = NewMessageType.Join;
                User = ((InspectRiderRequestViewModel)navigationData).User;
                _distance = ((InspectRiderRequestViewModel)navigationData).Distance;
                _ridePeriod = ((InspectRiderRequestViewModel)navigationData).RequestedRidePeriod.HasValue ?
                    ((InspectRiderRequestViewModel)navigationData).RequestedRidePeriod.Value:
                    RidePeriod.Tomorrow;
                _route = ((InspectRiderRequestViewModel)navigationData).PushpinsList;
                Subject = "Thanks for joining my carpool :)";

                // Pickup time
                var tripTime = UserHelper.GetTripTimeEstimationAsync(User.Location(), User.WorkLocation());
                var arrivalTime = User.ArrivalTime.Equals(default(DateTime).Date) ? App.CurrentUser.ArrivalTime : User.ArrivalTime;

                Message =
                    $"Hi {User.GivenName}," +
                    Environment.NewLine + Environment.NewLine +
                    $"Thanks for joining my carpool {GetRidePeriod(_ridePeriod)}!" +
                    Environment.NewLine +
                    $"I'll pick you around {string.Format("{0:h:mm tt}", arrivalTime.AddMinutes(-tripTime))?.ToLowerInvariant()}" +
                    Environment.NewLine + Environment.NewLine +
                    $"{App.CurrentUser.GivenName}";
            }

            return base.InitializeAsync(navigationData);
        }

        private object GetRidePeriod(RidePeriod ridePeriod)
        {
            if (ridePeriod.Equals(RidePeriod.Today))
            {
                return "today";
            }
            else if (ridePeriod.Equals(RidePeriod.EveryDay))
            {
                return "every day";
            }
            else
            {
                return "tomorrow";
            }
        }

        private async Task SendEmailAsync()
        {
            if (string.IsNullOrEmpty(Message))
            {
                await DialogService.ShowAlertAsync("Body is required", "Email failure", "Ok");

                return;
            }

            try
            {
                IsBusy = true;

                await _emailService.ComposeAndSendMailAsync(Subject, Message, BodyType.Text, User.Mail);

                if (_newMessageType == NewMessageType.Invite)
                {
                    // Get actual Driver
                    var drivers = await _dataService.GetAllDriversAsync();
                    var driver = drivers
                        .FirstOrDefault(r => r.Name.Equals(User.UserPrincipalName,
                        StringComparison.CurrentCultureIgnoreCase));

                    if (driver != null)
                    {
                        // Add Rider
                        await _dataService.AddRiderAsync(driver, App.CurrentUser.DisplayName);
                    }


                    await Task.WhenAny(new List<Task>() {
                        _dialogService.ShowAlertAsync(
                            $"Your invitation to {User.DisplayName} has been sent. You'll receive a notification if your ride request is accepted.",
                            "Invitation sent",
                            "OK"),
                        NavigationService.NavigateToAsync<MainViewModel>(ViewModelLocator.Instance.Resolve<UserPreferencesViewModel>())
                    });
                }
                else if (_newMessageType == NewMessageType.Join)
                {

                    // Get actual Driver
                    var drivers = await _dataService.GetAllDriversAsync();
                    var driverName = App.CurrentUser.UserPrincipalName;
                    var driver = drivers
                        .FirstOrDefault(r => r.Name.Equals(driverName,
                        StringComparison.CurrentCultureIgnoreCase));

                    if (driver != null)
                    {
                        // Update Rider Status
                        await _dataService.UpdateRiderStatusAsync(driver, User.DisplayName);
                        CurrentUser.RiderRequests -= 1;
                        MessagingCenter.Send(CurrentUser, MessengerKeys.UserReviewRiderRequest);

                        // create calendar event for pickup
                        await CreateEventsForRide(_ridePeriod);

                        // create ride detail registry
                        await CreateRideDetail();
                    }

                    await Task.WhenAny(new List<Task>() {
                        _dialogService.ShowAlertAsync(
                        $"You've added {User.DisplayName} to your carpool.",
                        "Rider accepted",
                        "OK"),
                        NavigationService.NavigateToAsync<MainViewModel>(this)
                    });
                }
            }
            catch (ServiceException)
            {
                await DialogService.ShowAlertAsync(
                    "System unavailable, please check your office365 subscription", 
                    "Email service unavailable", 
                    "Ok");
            }
            catch (Exception)
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    await DialogService.ShowAlertAsync(
                        "An error ocurred, please try again",
                        "Ops!",
                        "Ok");
                }
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task CreateEventsForRide(RidePeriod ridePeriod)
        {
            var startEvent = DateTime.Today + User.ArrivalTime.TimeOfDay;
            var endEvent = DateTime.Today + User.DepartureTime.TimeOfDay;
            if (startEvent.Date.Equals(default(DateTime).Date))
            {
                startEvent = DateTime.Today + App.CurrentUser.ArrivalTime.TimeOfDay;
                endEvent = DateTime.Today + App.CurrentUser.DepartureTime.TimeOfDay;
            }

            PatternedRecurrence pattern = null;
            
            if (ridePeriod.Equals(RidePeriod.EveryDay))
            {
                var recurrentEndEvent = endEvent.AddMonths(3);

                pattern = new PatternedRecurrence()
                {
                    Range = new RecurrenceRange()
                    {
                        StartDate = new Date(startEvent.Year, startEvent.Month, startEvent.Day),
                        EndDate = new Date(recurrentEndEvent.Year, recurrentEndEvent.Month, recurrentEndEvent.Day),
                        Type = RecurrenceRangeType.EndDate,
                        RecurrenceTimeZone = "UTC"
                    },
                    Pattern = new RecurrencePattern()
                    {
                        Interval = 1,
                        DaysOfWeek = new List<Microsoft.Graph.DayOfWeek>() {
                            Microsoft.Graph.DayOfWeek.Monday,
                            Microsoft.Graph.DayOfWeek.Tuesday,
                            Microsoft.Graph.DayOfWeek.Wednesday,
                            Microsoft.Graph.DayOfWeek.Thursday,
                            Microsoft.Graph.DayOfWeek.Friday
                        },
                        Type = RecurrencePatternType.Daily
                    }
                };
            }
            else if (ridePeriod.Equals(RidePeriod.Tomorrow))
            {
                startEvent = startEvent.AddDays(1);
                endEvent = endEvent.AddDays(1);
            }

            // calculate estimated trip time to event time
            var tripTime = UserHelper.GetTripTimeEstimationAsync(User.Location(), User.WorkLocation());

            await _calendarService.CreateEventAsync(
                startEvent.AddMinutes(-tripTime),
                startEvent,
                $"{AppSettings.CarpoolEventSubject} Pickup",
                User.StreetAddress,
                AppSettings.CarpoolEventSubject,
                new string[]{ User.Mail },
                false,
                App.CurrentUser.UserPrincipalName,
                pattern);

            await _calendarService.CreateEventAsync(
                endEvent,
                endEvent.AddMinutes(tripTime),
                $"{AppSettings.CarpoolEventSubject} Dropoff",
                User.StreetAddress,
                AppSettings.CarpoolEventSubject,
                new string[] { User.Mail },
                false,
                App.CurrentUser.UserPrincipalName,
                pattern);
        }

        private async Task CreateRideDetail()
        {
            var rideDetails = await _dataService.GetAllRideDetailsAsync();
            var rideDetail = rideDetails.FirstOrDefault(rd => rd.Driver.Equals(App.CurrentUser.DisplayName) && 
                    rd.Date.Date.Equals(DateTime.Today.AddDays(1)));

            if (rideDetail != null)
            {
                if (string.IsNullOrEmpty(rideDetail.Rider1))
                {
                    rideDetail.Rider1 = User.DisplayName;
                }
                else if (string.IsNullOrEmpty(rideDetail.Rider2))
                {
                    rideDetail.Rider2 = User.DisplayName;
                }
                else if (string.IsNullOrEmpty(rideDetail.Rider3))
                {
                    rideDetail.Rider3 = User.DisplayName;
                }
                else if (string.IsNullOrEmpty(rideDetail.Rider4))
                {
                    rideDetail.Rider4 = User.DisplayName;
                }

            } 
            else
            {
                rideDetail = new RideDetails
                {
                    Driver = App.CurrentUser.DisplayName,
                    Date = User.ArrivalTime,
                    Rider1 = User.DisplayName
                };
            }

            // Route
            rideDetail.Route = string.Join(";", _route.Select(r=> $"{r.Latitude}:{r.Longitude}"));
            // Route distance
            rideDetail.Distance = 2 * MapHelper.ConvertMetersToMiles(_distance);

            await _dataService.InsertOrUpdateRideDetailsAsync(rideDetail);
        }
    }
}