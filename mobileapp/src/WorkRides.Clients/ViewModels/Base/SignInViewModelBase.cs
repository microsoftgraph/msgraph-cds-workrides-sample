using System;
using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.ViewModels.Base
{
    public class SignInViewModelBase : ViewModelBase
    {
        private bool _todayChecked;
        private bool _tomorrowChecked;
        private bool _weekdaysChecked;
        private string _homeAddress;
        private string _workAddress;

        public SignInViewModelBase()
        {
            TodayChecked = false;
            TomorrowChecked = true;
            WeekdaysChecked = false;
        }

        public bool TodayChecked
        {
            get
            {
                return _todayChecked;
            }
            set
            {
                _todayChecked = value;
                RaisePropertyChanged(() => TodayChecked);
            }
        }

        public bool TomorrowChecked
        {
            get
            {
                return _tomorrowChecked;
            }
            set
            {
                _tomorrowChecked = value;
                RaisePropertyChanged(() => TomorrowChecked);
            }
        }

        public bool WeekdaysChecked
        {
            get
            {
                return _weekdaysChecked;
            }
            set
            {
                _weekdaysChecked = value;
                RaisePropertyChanged(() => WeekdaysChecked);
            }
        }

        public string HomeAddress
        {
            get
            {
                return _homeAddress;
            }

            set
            {
                if (!value.Equals(_homeAddress))
                {
                    _homeAddress = value;
                    RaisePropertyChanged(() => HomeAddress);
                }
            }
        }

        public string WorkAddress
        {
            get
            {
                return _workAddress;
            }

            set
            {
                if (!value.Equals(_workAddress))
                {
                    _workAddress = value;
                    RaisePropertyChanged(() => WorkAddress);
                }
            }
        }

        public TimeSpan ArrivalTimeSpan
        {
            get
            {
                return CurrentUser.ArrivalTime.TimeOfDay;
            }

            set
            {
                if (value != null && !value.Equals(CurrentUser.ArrivalTime.TimeOfDay))
                {
                    CurrentUser.ArrivalTime = CurrentUser.ArrivalTime.Date + value;
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedTimeRange);
                    RaisePropertyChanged(() => ArrivalTimeSpan);
                }
            }
        }

        public TimeSpan DepartureTimeSpan
        {
            get
            {
                return CurrentUser.DepartureTime.TimeOfDay;
            }

            set
            {
                if (value != null && !value.Equals(CurrentUser.DepartureTime.TimeOfDay))
                {
                    CurrentUser.DepartureTime = CurrentUser.DepartureTime.Date + value;
                    MessagingCenter.Send(CurrentUser, MessengerKeys.UserChangedTimeRange);
                    RaisePropertyChanged(() => DepartureTimeSpan);
                }
            }
        }

        public ICommand SelectorCommand => new Command<string>(Selector);

        private void Selector(string parameter)
        {
            TodayChecked = false;
            TomorrowChecked = false;
            WeekdaysChecked = false;

            if (parameter.Equals("Today"))
            {
                TodayChecked = true;
            }
            else if (parameter.Equals("Tomorrow"))
            {
                TomorrowChecked = true;
            }
            else
            {
                WeekdaysChecked = true;
            }
        }
    }
}