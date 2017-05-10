using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Text;

namespace CarPool.DataServiceConsole
{
    public class Program
    {
        private static CarPool.Clients.Core.Services.Data.IDataService _dataService;
        
        static void Main()
        {
            MainAsync().Wait();
        }

        static async Task MainAsync()
        {
            Console.WriteLine("---- Sample data service tester ----");

            //_dataService = new CarPool.Clients.Core.Services.Data.AzureMobileAppService();
            //_dataService = new CarPool.Clients.Core.Services.Data.CDSODataProvider();
            _dataService = new CarPool.Clients.Core.Services.Data.CDSDataProvider(new PlatformParameters(PromptBehavior.RefreshSession),
                false); // Hit local service?

            //var token = await RequestTokenAsync();
            //await _dataService.InitializeAsync(token);

            string option = string.Empty;

            while (!"X".Equals(option, StringComparison.CurrentCultureIgnoreCase))
            {
                Console.WriteLine("Select the entity to test :");
                Console.WriteLine("1 -> Driver");
                Console.WriteLine("2 -> Employee");
                Console.WriteLine("3 -> RideDetails");
                Console.WriteLine("4 -> List Drivers");
                Console.WriteLine("5 -> List Employees");
                Console.WriteLine("6 -> List RideDetails");
                Console.WriteLine("X -> Exit\n\n");
                option = Console.ReadLine();

                try
                {
                    switch (option)
                    {
                        case ("1"):
                            await TestDriverService();
                            break;
                        case ("2"):
                            await TestEmployeeService();
                            break;
                        case ("3"):
                            await TestRideDetailsService();
                            break;
                        case ("4"):
                            await TestListDriversService();
                            break;
                        case ("5"):
                            await TestListEmployeesService();
                            break;
                        case ("6"):
                            await TestListRideDetailsService();
                            break;
                        case ("X"):
                            break;
                        default:
                            Console.WriteLine("Really?");
                            break;
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Opss an exception has been catched: {e}");
                }
            }
        }

        private static async Task TestDriverService()
        {
            var riderName = "Meganb@mod627549.onmicrosoft.com";
            var driver = new Driver
            {
                Name = "JhonD",
                DisplayName = "Jhon Doe",
                Arrival = DateTime.Now,
                Departure = DateTime.Now,
            };

            var watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("Inserting new Driver ...");
            await _dataService.InsertAsync<Driver>(driver);

            Console.WriteLine($"Inserting new Driver in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            // check driver is stored
            driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(driver.Name));

            Console.WriteLine($"retrieve list Drivers in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (driver == null)
            {
                Console.WriteLine("Driver hasn't been created!!!");
                return;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // INSERT rider
            Console.WriteLine("Inserting a Rider on the carpool ...");
            driver.Rider1 = riderName;
            await _dataService.InsertOrUpdateDriverAsync(driver);

            Console.WriteLine($"Updating Driver in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            //check
            driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(driver.Name));

            Console.WriteLine($"Retrieve list Drivers in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (!riderName.Equals(driver.Rider1))
            {
                Console.WriteLine("Rider hasn't been included!!!");
                return;
            }
            else if (driver.Rider1Status)
            {
                Console.WriteLine("Rider status must be init as false.!!!");
                return;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // ACCEPT rider
            Console.WriteLine("Accepting the Rider in the carpool ...");
            await _dataService.UpdateRiderStatusAsync(driver, riderName);

            Console.WriteLine($"Updating rider status in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            //check
            driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(driver.Name));

            Console.WriteLine($"Retrieve list Drivers in {watch.ElapsedMilliseconds}ms");
            watch.Restart();
            
            if (!riderName.Equals(driver.Rider1))
            {
                Console.WriteLine("Rider has been deleted?");
                return;
            }
            else if (!driver.Rider1Status)
            {
                Console.WriteLine("Rider status must be set to true.!!!");
                return;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // REJECT RIDER
            Console.WriteLine("Rejecting the Rider in the carpool ...");
            await _dataService.RejectRiderAsync(driver, riderName);

            Console.WriteLine($"Rejected Driver in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            // check
            driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Name.Equals(driver.Name));

            Console.WriteLine($"Retrieve list Drivers in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (!string.IsNullOrEmpty(driver.Rider1))
            {
                Console.WriteLine("Rider hasn't been deleted!!!");
                return;
            }
            else if (driver.Rider1Status)
            {
                Console.WriteLine("Rider status must be set to false again!!!");
                return;
            }
            else
            {
                Console.WriteLine("OK");
            }

            // REMOVE user
            Console.WriteLine("Deleting the Driver ...");
            await _dataService.Remove<Driver>(driver);
            string id = driver.Id;

            Console.WriteLine($"Deleted Driver in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            driver = null;

            // check driver is removed
            driver = (await _dataService.GetAllDriversAsync())
                .FirstOrDefault(d => d.Id.Equals(id));

            Console.WriteLine($"Retrieve list Drivers in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (driver != null)
            {
                Console.WriteLine("Driver hasn't been deleted!!!");
                return;
            }
            else
            {
                Console.WriteLine("Driver successfully deleted");
            }
        }

        private static async Task TestEmployeeService()
        {
            Employee employee = new Employee
            {
                Name = "JhonD",
                Email = "JhonD@carpool.onmicrosoft.com",
                WorkAddress = "Microsoft Westlake terry office, Seattle",
                HomeAddress = "18600 Union Hill Road, Redmond 98052",
                Arrival = DateTime.Now,
                Departure = DateTime.Now,
                Latitude = 47.4923372,
                Longitude = -122.2901927,
                WorkLatitude = 47.621421,
                WorkLongitude = -122.338221,
                PhoneNo = "425-123-4567"
            };

            Console.WriteLine($"Employee count: {(await _dataService.GetAllEmployeesAsync()).Count()}");
            // INSERT employee
            var watch = System.Diagnostics.Stopwatch.StartNew();

            Console.WriteLine("Inserting new Employee ...");
            await _dataService.InsertAsync<Employee>(employee);

            Console.WriteLine($"Inserting new Employee in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            // check if employee is stored
            employee = (await _dataService.GetAllEmployeesAsync())
                .FirstOrDefault(d => d.Name.Equals(employee.Name));

            Console.WriteLine($"List employee in {watch.ElapsedMilliseconds}ms");

            Console.WriteLine($"Employee count: {(await _dataService.GetAllEmployeesAsync()).Count()}");
            watch.Restart();

            if (employee == null)
            {
                Console.WriteLine("Employee hasn't been created!!!");
                return;
            }
            else
            {
                Console.WriteLine("Employee created successfully");
            }

            // UPDATE employee
            Console.WriteLine("Updating the Employee ...");
            string updatedAddress = "3350 157th Ave N.E, Redmond 98052";
            employee.HomeAddress = updatedAddress;
            await _dataService.InsertOrUpdateEmployeeAsync(employee);

            Console.WriteLine($"Update employee in {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Employee count: {(await _dataService.GetAllEmployeesAsync()).Count()}");
            watch.Restart();

            //check
            employee = (await _dataService.GetAllEmployeesAsync())
                .FirstOrDefault(d => !string.IsNullOrEmpty(d.HomeAddress) && d.HomeAddress.Equals(updatedAddress));

            Console.WriteLine($"List employee in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (employee == null)
            {
                Console.WriteLine("Employee not found after update");
                return;
            }
            else if (!string.IsNullOrEmpty(employee.HomeAddress) && employee.HomeAddress.Equals(updatedAddress))
            {
                Console.WriteLine("Employee.HomeAddress updated successfully");
            }
            else
            {
                Console.WriteLine("Employee hasn't been updated");
            }

            // REMOVE employee
            Console.WriteLine("Deleting the Employee ...");
            string id = employee.Id;
            await _dataService.Remove<Employee>(employee);

            Console.WriteLine($"Delete employee in {watch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Employee count: {(await _dataService.GetAllEmployeesAsync()).Count()}");
            watch.Restart();

            // check employee is removed
            employee = (await _dataService.GetAllEmployeesAsync())
                .FirstOrDefault(d => d.Id.Equals(id));

            Console.WriteLine($"List employee in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (employee != null)
            {
                Console.WriteLine("Employee hasn't been deleted!!!");
                return;
            }
            else
            {
                Console.WriteLine("Employee deleted successfully");
            }
            Console.WriteLine();
        }

        private static async Task TestRideDetailsService()
        {
            RideDetails rideDetails = new RideDetails
            {
                Driver = "JhonD",
                Date = DateTime.Now,
                Rider1 = "MeganB",
                Rider1Status = true,
                Distance = 140
            };

            var watch = System.Diagnostics.Stopwatch.StartNew();

            // INSERT rideDetails
            Console.WriteLine("Inserting new Ride Details ...");
            await _dataService.InsertAsync<RideDetails>(rideDetails);

            Console.WriteLine($"Inserting new Ride Details in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            // check rideDetails is stored
            rideDetails = (await _dataService.GetAllRideDetailsAsync())
                .FirstOrDefault(d => d.Driver.Equals(rideDetails.Driver));

            Console.WriteLine($"Retrieve list of Ride Details in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (rideDetails == null)
            {
                Console.WriteLine("Ride Details hasn't been created!!!");
                return;
            }
            else
            {
                Console.WriteLine("RideDetails created successfully");
            }

            // UPDATE rideDetails
            Console.WriteLine("Updating the Ride Details ...");
            rideDetails.Rider1 = "JhonDoe";
            await _dataService.InsertOrUpdateRideDetailsAsync(rideDetails);

            Console.WriteLine($"Updating the Ride Details in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            //check
            rideDetails = (await _dataService.GetAllRideDetailsAsync())
                .FirstOrDefault(d => d.Rider1.Equals("JhonDoe"));

            Console.WriteLine($"Retrieve list of Ride Details in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (rideDetails == null)
            {
                Console.WriteLine("Ride Details hasn't been updated!!!");
                return;
            }
            else
            {
                Console.WriteLine("RideDetails.Rider1 updated successfully");
            }

            // REMOVE rideDetails
            Console.WriteLine("Deleting the Ride Details ...");
            await _dataService.Remove<RideDetails>(rideDetails);

            Console.WriteLine($"Deleting the Ride Details in {watch.ElapsedMilliseconds}ms");
            watch.Restart();
            string id = rideDetails.Id;

            // check rideDetails is removed
            rideDetails = (await _dataService.GetAllRideDetailsAsync())
                    .FirstOrDefault(item => item.Id.Equals(id));
            
            Console.WriteLine($"Retrieve list of Ride Details in {watch.ElapsedMilliseconds}ms");
            watch.Restart();

            if (rideDetails != null)
            {
                Console.WriteLine("Ride Details hasn't been deleted!!!");
                return;
            }
            else
            {
                Console.WriteLine("Ride Details deleted successfully");
            }
            Console.WriteLine();
        }
        private static async Task TestListEmployeesService()
        {
            List<Employee> employees = (await _dataService.GetAllEmployeesAsync()).ToList<Employee>();

            Console.WriteLine($"Employee count: {employees.Count()}");
            foreach (Employee e in employees)
            {
                Console.WriteLine($"ID= {e.Id}, Name='{e.Name}'");
            }
            Console.WriteLine("");
        }

        private static async Task TestListRideDetailsService()
        {
            List<RideDetails> rides = (await _dataService.GetAllRideDetailsAsync()).ToList<RideDetails>();

            Console.WriteLine($"RideDetails count: {rides.Count()}");
            foreach (RideDetails ride in rides)
            {
                Console.WriteLine($"ID= {ride.Id}, Driver='{ride.Driver}'");
            }
            Console.WriteLine("");
        }

        private static async Task TestListDriversService()
        {
            List<Driver> drivers = (await _dataService.GetAllDriversAsync()).ToList<Driver>();

            Console.WriteLine($"Driver count: {drivers.Count()}");
            foreach (Driver driver in drivers)
            {
                Console.WriteLine($"ID= {driver.Id}, DisplayName='{driver.DisplayName}'");
            }
            Console.WriteLine("");
        }

        static async Task<string> RequestTokenAsync()
        {
            //Get access token:   
            // To call a Data Catalog REST operation, create an instance of AuthenticationContext and call AcquireToken  
            // AuthenticationContext is part of the Active Directory Authentication Library NuGet package  
            // To install the Active Directory Authentication Library NuGet package in Visual Studio,   
            //  run "Install-Package Microsoft.IdentityModel.Clients.ActiveDirectory Version 2.19.208020213" from the nuget Package Manager Console.  

            //Resource Uri for Data Catalog API  
            string resourceUri = "http://rts.powerapps.com";

            //To learn how to register a client app and get a Client ID, see https://msdn.microsoft.com/en-us/library/azure/mt403303.aspx#clientID     
            string clientId = "7a6b341a-4db6-4e58-b7c0-d602792e5e5c";

            //A redirect uri gives AAD more details about the specific application that it will authenticate.  
            //Since a client app does not have an external service to redirect to, this Uri is the standard placeholder for a client app.  
            string redirectUri = "http://localhost";

            // Create an instance of AuthenticationContext to acquire an Azure access token  
            // OAuth2 authority Uri  
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);

            // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint  
            //  AcquireToken takes a Client Id that Azure AD creates when you register your client app.  
            AuthenticationResult result = await authContext.AcquireTokenAsync(resourceUri,
                                                    clientId,
                                                    new Uri(redirectUri),
                                                    new PlatformParameters(PromptBehavior.RefreshSession));

            return result.CreateAuthorizationHeader();
        }
    }
}
