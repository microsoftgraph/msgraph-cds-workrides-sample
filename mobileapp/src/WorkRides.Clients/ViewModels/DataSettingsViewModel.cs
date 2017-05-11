using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Extensions;
using CarPool.Clients.Core.Services.Graph.Calendar;
using System;
using System.Diagnostics;
using CarPool.Clients.Core.Services.Graph;

namespace CarPool.Clients.Core.ViewModels
{
    public class DataSettingsViewModel : ViewModelBase
    {
        private ObservableCollection<string> _filters;
        private string _filter;
        private ObservableCollection<Employee> _employees;
        private ObservableCollection<Driver> _drivers;
        private ObservableCollection<RideDetails> _rideDetails;
        private bool _isEmployee;
        private bool _isDriver;
        private bool _isRideDetails;

        private readonly IDataService _dataService;
        private readonly ICalendarService _calendarService;

        public DataSettingsViewModel(
            IDataService dataService,
            ICalendarService calendarService)
        {
            _dataService = dataService;
            _calendarService = calendarService;

            InitializeFilter();
        }

        public ObservableCollection<string> Filters
        {
            get { return _filters; }
            set
            {
                _filters = value;
                RaisePropertyChanged(() => Filters);
            }
        }

        public string Filter
        {
            get { return _filter; }
            set
            {
                _filter = value;
                RaisePropertyChanged(() => Filter);
            }
        }

        public ObservableCollection<Employee> Employees
        {
            get { return _employees; }
            set
            {
                _employees = value;
                RaisePropertyChanged(() => Employees);
            }
        }

        public ObservableCollection<Driver> Drivers
        {
            get { return _drivers; }
            set
            {
                _drivers = value;
                RaisePropertyChanged(() => Drivers);
            }
        }

        public ObservableCollection<RideDetails> RideDetails
        {
            get { return _rideDetails; }
            set
            {
                _rideDetails = value;
                RaisePropertyChanged(() => RideDetails);
            }
        }

        public bool IsEmployee
        {
            get { return _isEmployee; }
            set
            {
                _isEmployee = value;
                RaisePropertyChanged(() => IsEmployee);
            }
        }

        public bool IsDriver
        {
            get { return _isDriver; }
            set
            {
                _isDriver = value;
                RaisePropertyChanged(() => IsDriver);
            }
        }

        public bool IsRideDetails
        {
            get { return _isRideDetails; }
            set
            {
                _isRideDetails = value;
                RaisePropertyChanged(() => IsRideDetails);
            }
        }

        public ICommand ExitCommand => new Command(async () => await ExitAsync());

        public ICommand SearchCommand => new Command(async () => await SearchAsync());

        public ICommand ResetEmployeesCommand => new Command(async () => await ResetEmployeesAsync());

        public ICommand ResetDriversCommand => new Command(async () => await ResetDriversAsync());

        public ICommand ResetRideDetailsCommand => new Command(async () => await ResetRideDetailsAsync());

        public ICommand ResetCalendarCommand => new Command(async () => await ResetCalendarAsync());

        public ICommand ResetAllCommand => new Command(async () => await ResetAllAsync());

        public ICommand InserDriversCommand => new Command(async () => await InserDriversAsync());

        private async Task ExitAsync()
        {
            GraphClient.Instance.SignOut();
            await NavigationService.NavigateToAsync<LoginViewModel>();
        }

        public override async Task InitializeAsync(object navigationData)
        {
            await _dataService.InitializeAsync();
        }

        private async Task SearchAsync()
        {
            IsBusy = true;

            IsEmployee = false;
            IsDriver = false;
            IsRideDetails = false;

            try
            {
                if (Filter.Equals("Employees"))
                {
                    IsEmployee = true;
                    Employees = (await _dataService.GetAllEmployeesAsync())
                        .ToObservableCollection();
                }
                else if (Filter.Equals("Drivers"))
                {
                    IsDriver = true;
                    Drivers = (await _dataService.GetAllDriversAsync())
                        .ToObservableCollection();
                }
                else
                {
                    IsRideDetails = true;
                    RideDetails = (await _dataService.GetAllRideDetailsAsync())
                        .ToObservableCollection();
                }
            } 
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync($"Exception: {ex.Message}", "Unexpected error", "Ok");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private void InitializeFilter()
        {
            Filters = new ObservableCollection<string>
            {
                "Employees",
                "Drivers",
                "RideDetails"
            };

            Filter = Filters.FirstOrDefault();
        }

        private async Task ResetEmployeesAsync(bool alert = true)
        {
            try
            {
                if (!alert || await DialogService.ShowConfirmAsync(
                    "This will remove every employee data.",
                    "Are you sure?",
                    "Ok",
                    "Cancel"))
                {
                    IsBusy = true;

                    var employees = await _dataService.GetAllEmployeesAsync();

                    foreach (var employee in employees)
                    {
                        try
                        {
                            await _dataService.Remove<Employee>(employee);
                        }
                        catch (Exception e)
                        {
                            Debug.WriteLine($"[ResetEmployees] Error in: {e.Message}");
                        }
                    }

                    await DialogService.ShowAlertAsync("Employees has been deleted!", "Reset success", "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ResetEmployees] Error in: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ResetCalendarAsync()
        {
            try {
                IsBusy = true;
                     
                await GraphClient.Instance.Authenticate();

                var calendarEvents = await _calendarService.GetCarpoolEventsAsync(DateTime.Today, DateTime.Today.AddMonths(3));
                if (calendarEvents != null)
                {
                    foreach (var calendar in calendarEvents)
                    {
                        await _calendarService.DeleteEventAsync(calendar.Id);
                    }
                }

                GraphClient.Instance.SignOut();
                await DialogService.ShowAlertAsync("Calendar has been deleted!", "Reset success", "Ok");
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync("An error occured", "Reset failure", "Try again");
                Debug.WriteLine($"[ResetCalendar] Error in: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ResetDriversAsync(bool alert = true)
        {
            try
            {
                if (!alert || await DialogService.ShowConfirmAsync(
                   "This will remove every driver data.",
                   "Are you sure?",
                   "Ok",
                   "Cancel"))
                {
                    IsBusy = true;

                    var drivers = await _dataService.GetAllDriversAsync();

                    foreach (var driver in drivers)
                    {
                        try
                        {
                            await _dataService.Remove<Driver>(driver);
                        }
                        catch
                        {

                        }
                    }

                    await DialogService.ShowAlertAsync("Drivers has been deleted!", "Reset success", "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ResetDrivers] Error in: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ResetRideDetailsAsync(bool alert = true)
        {
            try
            {
                if (!alert || await DialogService.ShowConfirmAsync(
                    "This will remove every ride detail data.",
                    "Are you sure?",
                    "Ok",
                    "Cancel"))
                {
                    IsBusy = true;

                    var rideDetails = await _dataService.GetAllRideDetailsAsync();

                    foreach (var rideDetail in rideDetails)
                    {
                        try
                        {
                            await _dataService.Remove<RideDetails>(rideDetail);
                        }
                        catch
                        {

                        }
                    }

                    await DialogService.ShowAlertAsync("RideDetails has been deleted!", "Reset success", "Ok");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[ResetRideDetails] Error in: {ex.Message}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task ResetAllAsync()
        {
            try
            {
                if (await DialogService.ShowConfirmAsync(
                    "This will remove every data.",
                    "Are you sure?",
                    "Ok",
                    "Cancel"))
                {
                    IsBusy = true;

                    await ResetRideDetailsAsync(false);
                    await ResetDriversAsync(false);
                    await ResetEmployeesAsync(false);

                    await DialogService.ShowAlertAsync("Employees, Drivers and RideDetails has been deleted!", "Reset success", "Ok");
                }
            }
            catch (Exception ex)
            {
                await DialogService.ShowAlertAsync("An error occured", "Reset failure", "Try again");
                Debug.WriteLine($"[ResetAll] Error in: {ex}");
            }
            finally
            {
                IsBusy = false;
            }
        }

        private async Task InserDriversAsync()
        {
            try
            {
                if (await DialogService.ShowConfirmAsync(
                  "This will insert new drivers.",
                  "Are you sure?",
                  "Ok",
                  "Cancel"))
                {
                    IsBusy = true;

                    var employees = new ObservableCollection<Employee>
                    {
                        new Employee
                        {
                            Name = "Ben Walters",
                            Email = "BenW@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "18600 Union Hill Road, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.4923372,
                            Longitude = -122.2901927,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Lynne Robbins",
                            Email = "LynneR@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "3350 157th Ave N.E, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.5552982,
                            Longitude = -122.0506335,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Isaiah Langer",
                            Email = "IsaiahL@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "16061 N.E. 36th Way, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.5560924,
                            Longitude = -122.0495477,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },new Employee
                        {
                            Name = "Lidia Holloway",
                            Email = "LidiaH@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "16011 N.E. 36th Way, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.493574,
                            Longitude = -122.290587,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Henrietta Mueller",
                            Email = "HenriettaM@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "15255 NE 40th Street, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.555336,
                            Longitude = -122.0490588,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Alex Wilber",
                            Email = "AlexW@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "4400  148th Ave NE, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.493574,
                            Longitude = -122.290587,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Adele Vance",
                            Email = "AdeleV@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "4344 150th Ave NE, Redmond 98052",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.4923372,
                            Longitude = -122.2901927,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Christie Cline",
                            Email = "ChristieC@MOD627549.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Avenue North, Seattle 98109",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.5552982,
                            Longitude = -122.0506335,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Anne Wallace",
                            Email = "AnneW@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.521199,
                            Longitude = -122.338105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "David Longmuir",
                            Email = "DavidL@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.621199,
                            Longitude = -122.338105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Garret Vargas",
                            Email = "GarretV@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.501199,
                            Longitude = -122.338105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Garth Fort",
                            Email = "GarthF@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.501199,
                            Longitude = -122.368105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Molly Dempsey",
                            Email = "MollyD@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.501199,
                            Longitude = -122.258105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Pavel Bansky",
                            Email = "PavelB@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.401199,
                            Longitude = -122.258105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Rob Young",
                            Email = "RobY@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.401199,
                            Longitude = -122.158105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Bonnie Kearney",
                            Email = "BonnieK@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.451199,
                            Longitude = -122.158105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Alex Darrow",
                            Email = "AlexD@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.401199,
                            Longitude = -122.108105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        },
                        new Employee
                        {
                            Name = "Janet Schorr",
                            Email = "JanetS@fabrikamco.onmicrosoft.com",
                            Arrival = DateTime.Today + AppSettings.DefaultArrivalTime,
                            Departure = DateTime.Today + AppSettings.DefaultDepartureTime,
                            HomeAddress = "320 Westlake Ave N, Seattle, WA 98109, USA",
                            WorkAddress = "9825 Willows Road NE, Redmond – 98052, WA USA",
                            Latitude = 47.451199,
                            Longitude = -122.108105,
                            WorkLatitude = 47.687065,
                            WorkLongitude = -122.150736
                        }

                    };

                    foreach (var employee in employees)
                    {
                        await _dataService.InsertOrUpdateEmployeeAsync(employee);
                    }

                    var drivers = new ObservableCollection<Driver>
                    {
                        new Driver { Name = "AdeleV@MOD627549.onmicrosoft.com", DisplayName = "Adele Vance", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.4923372, Longitude = -122.2901927  },
                        new Driver { Name = "AlexW@MOD627549.onmicrosoft.com", DisplayName = "Alex Wilber", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.493574, Longitude = -122.290587 },
                        new Driver { Name = "BenW@MOD627549.onmicrosoft.com", DisplayName = "Ben Walters", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.4923372, Longitude = -122.2901927 },
                        new Driver { Name = "ChristieC@MOD627549.onmicrosoft.com", DisplayName = "Christie Cline", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.5552982, Longitude = -122.0506335 },
                        new Driver { Name = "HenriettaM@MOD627549.onmicrosoft.com", DisplayName = "Henrietta Mueller", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.555336, Longitude = -122.0490588 },
                        new Driver { Name = "IsaiahL@MOD627549.onmicrosoft.com", DisplayName = "Isaiah Langer", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.5560924, Longitude = -122.0495477 },
                        new Driver { Name = "LidiaH@MOD627549.onmicrosoft.com", DisplayName = "Lidia Holloway", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.493574, Longitude = -122.290587 },
                        new Driver { Name = "LynneR@MOD627549.onmicrosoft.com", DisplayName = "Lynne Robbins", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.5552982, Longitude = -122.0506335 },
                        new Driver { Name = "AnneW@fabrikamco.onmicrosoft.com", DisplayName = "Anne Wallace", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.521199, Longitude = -122.338105 },
                        new Driver { Name = "DavidL@fabrikamco.onmicrosoft.com", DisplayName = "David Longmuir", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.621199, Longitude = -122.338105 },
                        new Driver { Name = "GarretV@fabrikamco.onmicrosoft.com", DisplayName = "Garret Vargas", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime,  Latitude = 47.501199, Longitude = -122.338105 },
                        new Driver { Name = "GarthF@fabrikamco.onmicrosoft.com", DisplayName = "Garth Fort", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.501199, Longitude = -122.368105 },
                        new Driver { Name = "MollyD@fabrikamco.onmicrosoft.com", DisplayName = "Molly Dempsey", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.501199, Longitude = -122.258105 },
                        new Driver { Name = "PavelB@fabrikamco.onmicrosoft.com", DisplayName = "Pavel Bansky", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.401199, Longitude = -122.258105 },
                        new Driver { Name = "RobY@fabrikamco.onmicrosoft.com", DisplayName = "Rob Young", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.401199, Longitude = -122.158105 },
                        new Driver { Name = "AlexD@fabrikamco.onmicrosoft.com", DisplayName = "Alex Darrow", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.451199, Longitude = -122.158105 },
                        new Driver { Name = "JanetS@fabrikamco.onmicrosoft.com", DisplayName = "Janet Schorr", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.401199, Longitude = -122.108105 },
                        new Driver { Name = "BonnieK@fabrikamco.onmicrosoft.com", DisplayName = "Bonnie Kearney", Arrival = DateTime.Today + AppSettings.DefaultArrivalTime, Departure = DateTime.Today + AppSettings.DefaultDepartureTime, Latitude = 47.451199, Longitude = -122.108105 }
                    };

                    foreach (var driver in drivers)
                    {
                        await _dataService.InsertOrUpdateDriverAsync(driver);
                    }

                    await DialogService.ShowAlertAsync("Inserted new drivers", "Insert success", "Ok");
                }
            }
            catch
            {

            }
            finally
            {
                IsBusy = false;
            }
        }
    }
}