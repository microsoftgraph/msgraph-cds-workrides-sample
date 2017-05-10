using System.Threading.Tasks;
using CarPool.Clients.Core.Services.Graph.Calendar;
using CarPool.Clients.Core.ViewModels.Base;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using System.Windows.Input;
using Xamarin.Forms;
using Carpool.Clients.Services.Dialog;
using Microsoft.Graph;

namespace CarPool.Clients.Core.ViewModels
{
    public class ScheduleViewModel : ViewModelBase
    {
        private ObservableCollection<Grouping<DateTime, ScheduleGraphEvent>> _events;

        private readonly ICalendarService _calendarService;

        private readonly IDialogService _dialogService;

        private bool _hasEvents;

        public ScheduleViewModel(
            ICalendarService calendarService,
            IDialogService dialogService)
        {
            _calendarService = calendarService;
            _dialogService = dialogService;
            HasEvents = true;
        }

        public ObservableCollection<Grouping<DateTime, ScheduleGraphEvent>> Events
        {
            get { return _events; }
            set
            {
                _events = value;
                RaisePropertyChanged(() => Events);
            }
        }

        public bool HasEvents
        {
            get
            {
                return _hasEvents;
            }

            set
            {
                _hasEvents = value;
                RaisePropertyChanged(() => HasEvents);
            }
        }

        public ICommand RefreshCommand => new Command(async () => await RefreshAsync());

        public ICommand EditSettingsCommand => new Command(async () => await EditSettingsAsync());

        public ICommand ItemSelectedCommand => new Command<GraphEvent>(async (e) => await ItemSelectedAsync(e));

        public override async Task InitializeAsync(object navigationData)
        {
            await LoadScheduleAsync();
        }

        private async Task RefreshAsync()
        {
            await LoadScheduleAsync();
        }

        private async Task LoadScheduleAsync()
        {
            IsBusy = true;

            _events = PopulateScheduleCalendar();
            try
            {
                var startDateTime = DateTime.Today;
                var endDateTime = startDateTime.AddMonths(AppSettings.ScheduleMonths);
                var result = await _calendarService.GetCarpoolEventsAsync(startDateTime, endDateTime);
                HasEvents = result.Any();

                if (result != null)
                {
                    foreach (var ev in result.CurrentPage
                        .Where(e => e.Body.Content.Contains(AppSettings.CarpoolEventBody)))
                    {
                        var graphEvent  = new GraphEvent(ev);
                        var scheduleGraphEvent = _events.FirstOrDefault(e => e.Key.Date.Equals(graphEvent.StartDate.Date))?.FirstOrDefault();
                        if (scheduleGraphEvent != null)
                        {
                            if (graphEvent.BodyPreview.Contains("Pickup"))
                            {
                                scheduleGraphEvent.To = graphEvent;
                            }
                            else
                            {
                                scheduleGraphEvent.From = graphEvent;
                            }
                        }
                    }
                }

            }
            catch (ServiceException)
            {
                await _dialogService.ShowAlertAsync("Calendar unavailable, please check your office365 subscription.", "Service error", "Ok");
            }
            catch
            {
                if (!IsConnected)
                {
                    await _dialogService.ShowAlertAsync("No network connection, please check your internet.", "Service unavailable", "Ok");
                }
                else
                {
                    await _dialogService.ShowAlertAsync("An error occured", "Unexpected error", "Try again");
                }
            }
            finally
            {
                Events = _events;
                IsBusy = false;
            }
        }

        private async Task EditSettingsAsync()
        {
            await NavigationService.NavigateToAsync<ProfileViewModel>(this);
        }

        private ObservableCollection<Grouping<DateTime, ScheduleGraphEvent>> PopulateScheduleCalendar()
        {
            var result = new ObservableCollection<Grouping<DateTime, ScheduleGraphEvent>>();

            var fromDate = DateTime.Today;
            var untilDate = DateTime.Today.AddMonths(3);

            while (fromDate < untilDate)
            {
                result.Add(new Grouping<DateTime, ScheduleGraphEvent>(fromDate, new ScheduleGraphEvent()));
                fromDate = fromDate.AddDays(1);
            }

            return result;
        }

        private async Task ItemSelectedAsync(GraphEvent graphEvent)
        {
            await NavigationService.NavigateToAsync<ScheduleDetailViewModel>(graphEvent);
        }
    }
}