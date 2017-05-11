using System.Threading.Tasks;
using CarPool.Clients.Core.ViewModels.Base;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Services.Graph.Calendar;
using CarPool.Clients.Core.Helpers;
using System;
using CarPool.Clients.Core.Services.Graph.User;
using System.IO;
using System.Diagnostics;

namespace CarPool.Clients.Core.ViewModels
{
    public class UserPreferencesViewModel : ViewModelBase
    {
        private readonly ICalendarService _calendarService;
        private readonly IUserService _userService;

        private DateTime _arrivalTime;
        private DateTime _departureTime;

        public UserPreferencesViewModel(
            ICalendarService calendarService,
            IUserService userService)
        {
            _calendarService = calendarService;
            _userService = userService;
        }

        public ICommand SkipCommand => new Command(async () => await SkipAsync());

        public ICommand RideCommand => new Command(async () => await RideAsync());

        public ICommand DriveCommand => new Command(async () => await DriveAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            try
            {
                IsBusy = true;

                var today = DateTime.Today;
                var nextWeek = today.AddDays(7);

                // Get pickup and drop time based on scheduled calendar events
                var events = await _calendarService.GetDayEventsAsync(today, nextWeek);
                CalendarHelper.CalculateWeekdaysCalendarWorkTime(events, out _arrivalTime, out _departureTime);

                // Get user photo
                var photo = await _userService.GetUserPhotoAsync(App.CurrentUser.Mail);
                if (photo != null)
                {
                    App.CurrentUser.PhotoStream = ImageSource.FromStream(() => new MemoryStream(photo));
                }
            }
            catch (Exception ex)
            {
                _arrivalTime = DateTime.Today + AppSettings.DefaultArrivalTime;
                _departureTime = DateTime.Today + AppSettings.DefaultDepartureTime;
                Debug.WriteLine($"[UserPreferences] Error getting user info in: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task RideAsync()
        {
            CurrentUser.ArrivalTime = _arrivalTime;
            CurrentUser.DepartureTime = _departureTime;

            MessagingCenter.Send(App.CurrentUser, MessengerKeys.UserChangedTimeRange);
            await NavigationService.NavigateToAsync<RidePreferencesViewModel>();
        }

        private async Task SkipAsync()
        {
            CurrentUser.ArrivalTime = default(DateTime);
            CurrentUser.DepartureTime = default(DateTime);
            
            await NavigationService.NavigateToAsync<MainViewModel>(this);
        }

        private async Task DriveAsync()
        {
            // The user want to drive
            CurrentUser.ArrivalTime = _arrivalTime;
            CurrentUser.DepartureTime = _departureTime;

            MessagingCenter.Send(App.CurrentUser, MessengerKeys.UserChangedTimeRange);
            await NavigationService.NavigateToAsync<DriverPreferencesViewModel>(this);
        }
    }
}