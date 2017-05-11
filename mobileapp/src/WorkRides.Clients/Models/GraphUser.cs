using CarPool.Clients.Core.Maps.Model;
using Microsoft.Graph;
using System;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Models
{
    public class GraphUser : User, INotifyPropertyChanged
    {
        private ImageSource _photoStream;

        private bool _oof;

        public GraphUser(User user) : base()
        {
            initProfileColor();
            this.Merge(user);
        }

        public GraphUser(Driver driver) : base()
        {
            initProfileColor();

            if (driver != null)
            {
                this.IsDriver = true;
                this.UserPrincipalName = driver.Name;
                this.Mail = driver.Name;
                this.DisplayName = driver.DisplayName;
                this.ArrivalTime = driver.Arrival;
                this.DepartureTime = driver.Departure;
                this.Latitude = driver.Latitude;
                this.Longitude = driver.Longitude;
            }

        }

        public GraphUser(Employee employee) : base()
        {
            initProfileColor();
            this.Merge(employee);
        }

        private void initProfileColor()
        {
            var index = Interlocked.Increment(ref AppSettings.currentColor);
            this.ProfileColor = AppSettings.RiderColors[index % AppSettings.RiderColors.Length];
        }

        public void Merge(User user)
        {
            if (user != null)
            {
                this.BusinessPhones = user.BusinessPhones;
                this.CompanyName = user.CompanyName;
                this.Country = user.Country;
                this.Department = user.Department;
                this.DisplayName = user.DisplayName;
                this.GivenName = user.GivenName;
                this.UserPrincipalName = user.UserPrincipalName;
                if (string.IsNullOrEmpty(user.Mail))
                {
                    this.Mail = user.UserPrincipalName;
                }
                else
                {
                    this.Mail = user.Mail;
                }
                this.Id = user.Id;
                this.JobTitle = user.JobTitle;
                this.City = user.City;
                this.PostalCode = user.PostalCode;
                this.StreetAddress = user.StreetAddress;
                if (!string.IsNullOrEmpty(StreetAddress))
                {
                    this.HomeAddress = $"{this.StreetAddress}, {this.City} {this.PostalCode}";
                }
                else
                {
                    this.HomeAddress = string.Empty;
                }
                this.WorkAddress = string.Empty;
                this.Surname = user.Surname;
                this.State = user.State;
                this.BusinessPhones = user.BusinessPhones;
                this.MobilePhone = user.MobilePhone;
            }
        }

        internal void Merge(Employee employee)
        {
            if (employee != null)
            {
                this.HomeAddress = employee.HomeAddress;
                this.Latitude = employee.Latitude;
                this.Longitude = employee.Longitude;
                this.WorkAddress = employee.WorkAddress;
                this.WorkLatitude = employee.WorkLatitude;
                this.WorkLongitude = employee.WorkLongitude;
                this.ArrivalTime = employee.Arrival;
                this.DepartureTime = employee.Departure;
            }
        }

        public RidePeriod RidePeriod { get; set; } = RidePeriod.Tomorrow;

        public ImageSource PhotoStream
        {
            get { return _photoStream; }
            set
            {
                _photoStream = value;
                OnPropertyChanged("PhotoStream");
            }
        }

        public string HomeAddress { get; set; }

        public string WorkAddress { get; set; }

        public double? WorkLatitude { get; set; }

        public double? WorkLongitude { get; set; }

        public double? Latitude { get; set; }

        public double? Longitude { get; set; }

        public double? Distance { get; set; }

        public DateTime DepartureTime { get; set; }

        public DateTime ArrivalTime { get; set; }

        public Color ProfileColor { get; set; }

        public int RiderRequests { get; set; }

        public bool IsDriver { get; set; }

        public bool OOF
        {
            get
            {
                return _oof;
            }
            set
            {
                _oof = value;
                OnPropertyChanged("OOF");
            }
        }

        public bool HasLocation() => Latitude.HasValue && 
            Longitude.HasValue && 
            Latitude.Value != default(double) && 
            Longitude.Value != default(double);

        public bool HasWorkLocation() => WorkLatitude.HasValue && 
            WorkLongitude.HasValue &&
            WorkLatitude.Value != default(double) &&
            WorkLongitude.Value != default(double);

        public Position? Location()
        {
            if (HasLocation())
            {
                return new Position(Latitude.Value, Longitude.Value);
            }
            else
            {
                return null;
            }
        }

        public Position? WorkLocation()
        {
            if (HasWorkLocation())
            {
                return new Position(WorkLatitude.Value, WorkLongitude.Value);
            }
            else
            {
                return null;
            }
        }

        public CustomPin CreateUserPin()
        {
            var pin = new CustomPin()
            {
                Id = string.IsNullOrEmpty(Id)?UserPrincipalName:Id,
                Title = GivenName,
                Description = GivenName
            };

            if (HasLocation())
            {
                pin.Longitude = Longitude.Value;
                pin.Latitude = Latitude.Value;
            }

            pin.Icon = Xamarin.Forms.Device.OnPlatform(
                            "pins/Pin_Green.png",
                            "Pins/Pin_Green.png",
                            "Pins/Pin_Green.png");

            return pin;
        }

        public CustomRider CreateRidePin()
        {
            var pin = new CustomRider()
            {
                Id = string.IsNullOrEmpty(Id) ? UserPrincipalName : Id,
                Title = GivenName,
                Description = GivenName
            };

            if (HasLocation())
            {
                pin.Longitude = Longitude.Value;
                pin.Latitude = Latitude.Value;

            }

            pin.Color = ProfileColor;

            if (!string.IsNullOrEmpty(DisplayName))
            {
                var split = DisplayName.Split(' ');
                if (split.Any())
                {
                    pin.Acronym = string.Format("{0}{1}",
                        split.First().Substring(0, 1),
                        split.Last().Substring(0, 1));
                }
            }
            else
            {
                pin.Acronym = string.Format("{0}{1}",
                        UserPrincipalName.Substring(0, 1),
                        UserPrincipalName.Substring(1, 2));
            }
            

            return pin;
        }

        public CustomPin CreateWorkPin()
        {
            var pin = new CustomPin()
            {
                Id = string.IsNullOrEmpty(Id) ? UserPrincipalName : Id,
                Title = GivenName,
                Description = GivenName
            };

            if (HasWorkLocation())
            {
                pin.Longitude = WorkLongitude.Value;
                pin.Latitude = WorkLatitude.Value;
            }

            pin.Icon = Xamarin.Forms.Device.OnPlatform(
                            "pins/Pin_Orange.png",
                            "Pins/Pin_Orange.png",
                            "Pins/Pin_Orange.png");

            return pin;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }

        public override bool Equals(object obj)
        {
            if (obj is GraphUser)
            {
                if (!string.IsNullOrEmpty(UserPrincipalName))
                {
                    return UserPrincipalName.Equals(((GraphUser)obj).UserPrincipalName);
                }
                else if (!string.IsNullOrEmpty(Mail))
                {
                    return Mail.Equals(((GraphUser)obj).Mail);
                }
            }

            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (!string.IsNullOrEmpty(UserPrincipalName))
            {
                return UserPrincipalName.GetHashCode();
            }
            else if (!string.IsNullOrEmpty(Mail))
            {
                return Mail.GetHashCode();
            }

            return base.GetHashCode();
        }
    }

    public enum RidePeriod
    {
        Today,
        Tomorrow,
        EveryDay
    }
}
