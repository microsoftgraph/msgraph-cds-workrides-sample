using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CarPool.Clients.Core.Models;

using Microsoft.OData.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using CarPool.Clients.Core.Helpers;

namespace CarPool.Clients.Core.Services.Data
{
    public class CDSODataProvider : IDataService
    {
        private const string serviceUri = "https://pa-rts-sf-6431c9599bf-prod-westus.rsu.powerapps.com/namespaces/de5c0dcf-2aa1-4df3-bf80-5388c301b18d/data/";

        private Microsoft.Dynamics.DataEntities.Resources cds;

        public CDSODataProvider()
        {
            init();
        }

        public void init()
        {
            if (cds != null)
            {
                return;
            }

            // Create OData proxy
            cds = new Microsoft.Dynamics.DataEntities.Resources(new Uri(serviceUri));

            //cds.SaveChangesDefaultOptions(SaveChangesOptions)

            //Add authorization header
            cds.SendingRequest2 += new EventHandler<SendingRequest2EventArgs>(ODataResource_OnSendingRequest);

            //Ignore nulls - suggestion from http://stackoverflow.com/questions/24138082/wcf-odata-client-how-to-ignore-null-properties-in-request
            cds.Configurations.RequestPipeline.OnEntryStarting((args) =>
            {
                args.Entry.Properties = args.Entry.Properties.Where((property) => property.Value != null);
            });

            ManuallyRemoveReadOnlyFields();
        }

        private void ManuallyRemoveReadOnlyFields()
        {
            //Manually remove read-only fields that aren't marked correctly by CDS OData service
            string[] requestPropertiesToRemove = { "CreatedOnDateTime",
                                                    "LastModifiedDateTime",
                                                    "RecordId",
                                                    "RecordVersion",
                                                    "CreatedByUser",
                                                    "LastModifiedByUser"
                                                    };
            cds.Configurations.RequestPipeline.RemoveProperties<Microsoft.Dynamics.DataEntities.FabrikamEmployee_>(cds.ResolveType, requestPropertiesToRemove);
            cds.Configurations.RequestPipeline.RemoveProperties<Microsoft.Dynamics.DataEntities.FabrikamDrivers_>(cds.ResolveType, requestPropertiesToRemove);
            cds.Configurations.RequestPipeline.RemoveProperties<Microsoft.Dynamics.DataEntities.FabrikamRideShare_>(cds.ResolveType, requestPropertiesToRemove);

            string[] responsePropertiesToRemove = { "CreatedOnDateTime",
                                                    "LastModifiedDateTime"
                                                    };
            //cds.Configurations.ResponsePipeline.RemoveProperties<Microsoft.Dynamics.DataEntities.FabrikamEmployee_>(cds.ResolveType, responsePropertiesToRemove);
        }

        /// <summary>
        /// Adds an authentication header when sending requests to OData services.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The event's arguments.</param>
        private void ODataResource_OnSendingRequest(object sender, SendingRequest2EventArgs e)
        {
            //Authenticate
            e.RequestMessage.SetHeader("Authorization", authHeader);
        }

        private static string authHeader;
        
        private Task<AuthenticationResult> AccessToken()
        {
            Task<AuthenticationResult> result = DependencyService.Get<IAuthenticator>()
                    .Authenticate(AppSettings.CdsAuthorityUri, 
                        AppSettings.CdsResourceUri, 
                        AppSettings.CarpoolClientId, 
                        AppSettings.CdsRedirectUri);

            return result;
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            List<Driver> drivers = new List<Driver>();

            var driverData = await cds.FabrikamDrivers_.ExecuteAsync();

            foreach (var x in driverData)
            {
                Driver driver = CDSDriverToAppDriver(x);
                drivers.Add(driver);
            }

            return drivers;
        }

        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            List<Employee> employees = new List<Employee>();

            var employeeData = await cds.FabrikamEmployee_.ExecuteAsync();

            foreach (var x in employeeData)
            {
                Employee employee = CDSEmployeeToAppEmployee(x);
                employees.Add(employee);
            }

            return employees;
        }

        public async Task<IEnumerable<Employee>> GetEmployeeAsync(string name)
        {
            List<Employee> employees = new List<Employee>();

            var employeeData = await cds.FabrikamEmployee_.ExecuteAsync();

            foreach (var x in employeeData)
            {
                Employee employee = CDSEmployeeToAppEmployee(x);
                employees.Add(employee);
            }

            return employees;
        }

        public Driver CDSDriverToAppDriver(Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver)
        {
            Driver driver = new Driver();
            driver.Id = cdsDriver.PrimaryId;
            driver.Name = cdsDriver.Name;
            driver.Arrival = cdsDriver.Arrival.HasValue ? cdsDriver.Arrival.Value.DateTime.ToLocalTime() : new DateTime();
            driver.Departure = cdsDriver.Departure.HasValue ? cdsDriver.Departure.Value.DateTime.ToLocalTime() : new DateTime();
            driver.AverageMiles = cdsDriver.AverageMiles.HasValue ? Convert.ToDouble(cdsDriver.AverageMiles) : 0;
            driver.DisplayName = cdsDriver.DisplayName;
            driver.Latitude = cdsDriver.HomeLatitude.HasValue ? Convert.ToDouble(cdsDriver.HomeLatitude) : 0;
            driver.Longitude = cdsDriver.HomeLongitude.HasValue ? Convert.ToDouble(cdsDriver.HomeLongitude) : 0;
            driver.RatePerMile = cdsDriver.RatePerMile.ToString();
            driver.Rider1 = cdsDriver.Rider1Name;
            driver.Rider1Status = cdsDriver.Rider1Status.HasValue ? cdsDriver.Rider1Status.Value : false;
            driver.Rider2 = cdsDriver.Rider2Name;
            driver.Rider2Status = cdsDriver.Rider2Status.HasValue ? cdsDriver.Rider2Status.Value : false;
            driver.Rider3 = cdsDriver.Rider3Name;
            driver.Rider3Status = cdsDriver.Rider3Status.HasValue ? cdsDriver.Rider3Status.Value : false;
            driver.Rider4 = cdsDriver.Rider4Name;
            driver.Rider4Status = cdsDriver.Rider4Status.HasValue ? cdsDriver.Rider4Status.Value : false;
            driver.Schedule = cdsDriver.Schedule;

            return driver;
        }

        public Microsoft.Dynamics.DataEntities.FabrikamDrivers_ AppDriverToCDSDriver(Driver driver)
        {
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = new Microsoft.Dynamics.DataEntities.FabrikamDrivers_();

            cdsDriver.PrimaryId = driver.Id;
            UpdateCDSDriverFromAppDriver(cdsDriver, driver);

            return cdsDriver;
        }

        public void UpdateCDSDriverFromAppDriver(Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver, Driver driver)
        {
            //Could check for id match before update
            //cdsEmployee.PrimaryId = employee.Id;

            cdsDriver.Name = driver.Name;
            cdsDriver.Arrival = driver.Arrival.ToUniversalTime();
            cdsDriver.Departure = driver.Departure.ToUniversalTime();
            cdsDriver.AverageMiles = Convert.ToDecimal(driver.AverageMiles);
            cdsDriver.DisplayName = driver.DisplayName;
            cdsDriver.HomeLatitude = driver.Latitude.HasValue ? Convert.ToDecimal(driver.Latitude) : 0;
            cdsDriver.HomeLongitude = driver.Longitude.HasValue ? Convert.ToDecimal(driver.Longitude) : 0;
            cdsDriver.RatePerMile = !String.IsNullOrEmpty(driver.RatePerMile) ? Convert.ToDecimal(driver.RatePerMile) : 0;
            cdsDriver.Rider1Name = driver.Rider1;
            cdsDriver.Rider1Status = driver.Rider1Status;
            cdsDriver.Rider2Name = driver.Rider2;
            cdsDriver.Rider2Status = driver.Rider2Status;
            cdsDriver.Rider3Name = driver.Rider3;
            cdsDriver.Rider3Status = driver.Rider3Status;
            cdsDriver.Rider4Name = driver.Rider4;
            cdsDriver.Rider4Status = driver.Rider4Status;
            cdsDriver.Schedule = driver.Schedule;
        }

        public Employee CDSEmployeeToAppEmployee(Microsoft.Dynamics.DataEntities.FabrikamEmployee_ cdsEmployee)
        {
            Employee employee = new Employee();
            employee.Id = cdsEmployee.PrimaryId;
            employee.Name = cdsEmployee.FullName;
            employee.Email = cdsEmployee.Email;
            employee.PhoneNo = cdsEmployee.Phone;
            employee.WorkAddress = cdsEmployee.WorkAddress_AddressLine1;
            employee.HomeAddress = cdsEmployee.HomeAddress_AddressLine1;
            employee.Arrival = cdsEmployee.PreferredArrivalTimeAtWork.HasValue ? cdsEmployee.PreferredArrivalTimeAtWork.Value.DateTime.ToLocalTime() : new DateTime();
            employee.Departure = cdsEmployee.PreferredDepartureTimeFromWork.HasValue ? cdsEmployee.PreferredDepartureTimeFromWork.Value.DateTime.ToLocalTime() : new DateTime();
            employee.Latitude = cdsEmployee.HomeLatitude.HasValue ? Convert.ToDouble(cdsEmployee.HomeLatitude) : 0;
            employee.Longitude = cdsEmployee.HomeLongitude.HasValue ? Convert.ToDouble(cdsEmployee.HomeLongitude) : 0;
            employee.WorkLatitude = cdsEmployee.WorkLatitude.HasValue ? Convert.ToDouble(cdsEmployee.WorkLatitude) : 0;
            employee.WorkLongitude = cdsEmployee.WorkLongitude.HasValue ? Convert.ToDouble(cdsEmployee.WorkLongitude) : 0;

            return employee;
        }

        public Microsoft.Dynamics.DataEntities.FabrikamEmployee_ AppEmployeeToCDSEmployee(Employee employee)
        {
            Microsoft.Dynamics.DataEntities.FabrikamEmployee_ cdsEmployee = new Microsoft.Dynamics.DataEntities.FabrikamEmployee_();

            cdsEmployee.PrimaryId = employee.Id;
            UpdateCDSEmployeeFromAppEmployee(cdsEmployee, employee);

            return cdsEmployee;
        }

        public void UpdateCDSEmployeeFromAppEmployee(Microsoft.Dynamics.DataEntities.FabrikamEmployee_ cdsEmployee, Employee employee)
        {
            //Could check for id match before update
            //cdsEmployee.PrimaryId = employee.Id;

            cdsEmployee.FullName = employee.Name;
            cdsEmployee.Email = employee.Email;
            cdsEmployee.Phone = employee.PhoneNo;
            cdsEmployee.WorkAddress_AddressLine1 = employee.WorkAddress;
            cdsEmployee.HomeAddress_AddressLine1 = employee.HomeAddress;
            cdsEmployee.PreferredArrivalTimeAtWork = new DateTimeOffset(employee.Arrival.ToUniversalTime());
            cdsEmployee.PreferredDepartureTimeFromWork = new DateTimeOffset(employee.Departure.ToUniversalTime());
            cdsEmployee.HomeLatitude = Convert.ToDecimal(employee.Latitude);
            cdsEmployee.HomeLongitude = Convert.ToDecimal(employee.Longitude);
            cdsEmployee.WorkLatitude = Convert.ToDecimal(employee.WorkLatitude);
            cdsEmployee.WorkLongitude = Convert.ToDecimal(employee.WorkLongitude);
        }

        public RideDetails CDSRideDetailsToAppRideDetails(Microsoft.Dynamics.DataEntities.FabrikamRideShare_ cdsRideDetails)
        {
            RideDetails rideDetails = new RideDetails();
            rideDetails.Id = cdsRideDetails.PrimaryId;
            rideDetails.Driver = cdsRideDetails.DriverName;
            rideDetails.Distance = cdsRideDetails.Distance.HasValue ? Convert.ToDouble(cdsRideDetails.Distance) : 0;
            rideDetails.EmployeeSubmissionStatus = cdsRideDetails.ExpenseReportSubmitted.HasValue ? cdsRideDetails.ExpenseReportSubmitted.Value : false;
            rideDetails.Date = cdsRideDetails.RideDateTime.HasValue? cdsRideDetails.RideDateTime.Value.DateTime : new DateTime();
            rideDetails.FinanceApprovalStatus = cdsRideDetails.FinanceApprovalStatus.HasValue ? cdsRideDetails.FinanceApprovalStatus.Value : false;
            rideDetails.ManagerApprovalStatus = cdsRideDetails.ExpenseReportApproved.HasValue ? cdsRideDetails.ExpenseReportApproved.Value : false;
            rideDetails.Route = cdsRideDetails.RouteDescription;
            rideDetails.Rider1 = cdsRideDetails.Passenger1Name;
            rideDetails.Rider1Status = cdsRideDetails.Passenger1OnRide.HasValue ? cdsRideDetails.Passenger1OnRide.Value : false;
            rideDetails.Rider2 = cdsRideDetails.Passenger2Name;
            rideDetails.Rider2Status = cdsRideDetails.Passenger2OnRide.HasValue ? cdsRideDetails.Passenger2OnRide.Value : false;
            rideDetails.Rider3 = cdsRideDetails.Passenger3Name;
            rideDetails.Rider3Status = cdsRideDetails.Passenger3OnRide.HasValue ? cdsRideDetails.Passenger3OnRide.Value : false;
            rideDetails.Rider4 = cdsRideDetails.Passenger4Name;
            rideDetails.Rider4Status = cdsRideDetails.Passenger4OnRide.HasValue ? cdsRideDetails.Passenger4OnRide.Value : false;
            
            return rideDetails;
        }

        public Microsoft.Dynamics.DataEntities.FabrikamRideShare_ AppRideDetailsToCDSRideDetails(RideDetails rideDetails)
        {
            Microsoft.Dynamics.DataEntities.FabrikamRideShare_ cdsRideDetails = new Microsoft.Dynamics.DataEntities.FabrikamRideShare_();

            cdsRideDetails.PrimaryId = rideDetails.Id;
            UpdateCDSRideDetailsFromAppRideDetails(cdsRideDetails, rideDetails);

            return cdsRideDetails;
        }

        public void UpdateCDSRideDetailsFromAppRideDetails(Microsoft.Dynamics.DataEntities.FabrikamRideShare_ cdsRideDetails, RideDetails rideDetails)
        {
            //Could check for id match before update
            //cdsRideDetails.PrimaryId = rideDetails.Id;

            cdsRideDetails.DriverName = rideDetails.Driver;
            cdsRideDetails.Distance = Convert.ToDecimal(rideDetails.Distance);
            cdsRideDetails.ExpenseReportSubmitted = rideDetails.EmployeeSubmissionStatus;
            cdsRideDetails.RideDateTime = rideDetails.Date;
            cdsRideDetails.FinanceApprovalStatus = rideDetails.FinanceApprovalStatus;
            cdsRideDetails.ExpenseReportApproved = rideDetails.ManagerApprovalStatus;
            cdsRideDetails.RouteDescription = rideDetails.Route;
            cdsRideDetails.Passenger1Name = rideDetails.Rider1;
            cdsRideDetails.Passenger1OnRide = rideDetails.Rider1Status;
            cdsRideDetails.Passenger2Name = rideDetails.Rider2;
            cdsRideDetails.Passenger2OnRide = rideDetails.Rider2Status;
            cdsRideDetails.Passenger3Name = rideDetails.Rider3;
            cdsRideDetails.Passenger3OnRide = rideDetails.Rider3Status;
            cdsRideDetails.Passenger4Name = rideDetails.Rider4;
            cdsRideDetails.Passenger4OnRide = rideDetails.Rider4Status;

            return;
        }

        public async Task<IEnumerable<RideDetails>> GetAllRideDetailsAsync()
        {
            List<RideDetails> rides = new List<RideDetails>();

            var rideData = await cds.FabrikamRideShare_.ExecuteAsync();

            foreach (var x in rideData)
            {
                RideDetails ride = CDSRideDetailsToAppRideDetails(x);
                rides.Add(ride);
            }

            return rides;
        }

        public async Task InitializeAsync(string token = "")
        {
            if (string.IsNullOrEmpty(token))
            {
                authHeader = (await AccessToken())?.CreateAuthorizationHeader();
            }
            else
            {
                authHeader = token;
            }
        }

        public Task InsertAsync<T>(T item)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Driver))
            {
                return InsertDriver(item as Driver);
            }
            if (itemType == typeof(Employee))
            {
                return InsertEmployee(item as Employee);
            }
            if (itemType == typeof(RideDetails))
            {
                return InsertRideDetails(item as RideDetails);
            }
            return Task.Delay(0);
        }

        private Task InsertDriver(Driver item)
        {
            var entityCollection = new DataServiceCollection<Microsoft.Dynamics.DataEntities.FabrikamDrivers_>(cds);
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = AppDriverToCDSDriver(item);
            entityCollection.Add(cdsDriver);
            return cds.SaveChangesAsync();
        }

        private Task InsertEmployee(Employee item)
        {
            var entityCollection = new DataServiceCollection<Microsoft.Dynamics.DataEntities.FabrikamEmployee_>(cds);
            Microsoft.Dynamics.DataEntities.FabrikamEmployee_ cdsEmployee = AppEmployeeToCDSEmployee(item);
            entityCollection.Add(cdsEmployee);
            return cds.SaveChangesAsync();
        }

        private Task InsertRideDetails(RideDetails item)
        {
            var entityCollection = new DataServiceCollection<Microsoft.Dynamics.DataEntities.FabrikamRideShare_>(cds);
            Microsoft.Dynamics.DataEntities.FabrikamRideShare_ cdsRideDetails = AppRideDetailsToCDSRideDetails(item);
            entityCollection.Add(cdsRideDetails);
            return cds.SaveChangesAsync();
        }

        public async Task InsertOrUpdateDriverAsync(Driver item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                await InsertDriver(item);
            }
            else
            {
                Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = await FindDriver(item);

                if (cdsDriver != null)
                {
                    UpdateCDSDriverFromAppDriver(cdsDriver, item);
                    cds.UpdateObject(cdsDriver);
                    await cds.SaveChangesAsync();
                }
                else
                {
                    throw new KeyNotFoundException($"Update called for driver {item.Id} but no driver with that id found in CDS");
                }
            }
        }

        public async Task InsertOrUpdateEmployeeAsync(Employee item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                await InsertEmployee(item);
            }
            else
            {
                Microsoft.Dynamics.DataEntities.FabrikamEmployee_ cdsEmployee = await FindEmployee(item);

                if (cdsEmployee != null)
                {
                    UpdateCDSEmployeeFromAppEmployee(cdsEmployee, item);
                    cds.UpdateObject(cdsEmployee);
                    await cds.SaveChangesAsync();
                }
                else
                {
                    throw new KeyNotFoundException($"Update called for employee {item.Id} but no employee with that id found in CDS");
                }
            }
        }

        private async Task<Microsoft.Dynamics.DataEntities.FabrikamDrivers_> FindDriver(Driver item)
        {
            //Find driver
            var driverData = await cds.FabrikamDrivers_.ExecuteAsync();
            var cdsDriver = driverData.FirstOrDefault(e => e.PrimaryId.Equals(item.Id));
            return cdsDriver;
        }

        private async Task<Microsoft.Dynamics.DataEntities.FabrikamEmployee_> FindEmployee(Employee item)
        {
            //Find employee
            var employeeData = await cds.FabrikamEmployee_.ExecuteAsync();
            var cdsEmployee = employeeData.FirstOrDefault(e => e.PrimaryId.Equals(item.Id));
            return cdsEmployee;
        }

        private async Task<Microsoft.Dynamics.DataEntities.FabrikamRideShare_> FindRideDetails(RideDetails item)
        {
            //Find RideDetails
            var rideData = await cds.FabrikamRideShare_.ExecuteAsync();
            var cdsRideDetails = rideData.FirstOrDefault(e => e.PrimaryId.Equals(item.Id));
            return cdsRideDetails;
        }

        public async Task InsertOrUpdateRideDetailsAsync(RideDetails item)
        {
            if (string.IsNullOrEmpty(item.Id))
            {
                await InsertRideDetails(item);
            }
            else
            {
                Microsoft.Dynamics.DataEntities.FabrikamRideShare_ cdsRideDetails = await FindRideDetails(item);

                if (cdsRideDetails != null)
                {
                    UpdateCDSRideDetailsFromAppRideDetails(cdsRideDetails, item);
                    cds.UpdateObject(cdsRideDetails);
                    await cds.SaveChangesAsync();
                }
                else
                {
                    throw new KeyNotFoundException($"Update called for rideDetails {item.Id} but no rideDetails with that id found in CDS");
                }
            }
        }

        public Task Remove<T>(T item)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Driver))
            {
                return DeleteDriver(item as Driver);
            }
            if (itemType == typeof(Employee))
            {
                return DeleteEmployee(item as Employee);
            }
            if (itemType == typeof(RideDetails))
            {
                return DeleteRideDetails(item as RideDetails);
            }
            return Task.Delay(0);
        }

        private async Task DeleteDriver(Driver item)
        {
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = await FindDriver(item);

            if (cdsDriver != null)
            {
                cds.DeleteObject(cdsDriver);
                await cds.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Delete called for Driver {item.Id} but no Driver with that id found in CDS");
            }
        }

        private async Task DeleteEmployee(Employee item)
        {
            Microsoft.Dynamics.DataEntities.FabrikamEmployee_ cdsEmployee = await FindEmployee(item);

            if (cdsEmployee != null)
            {
                cds.DeleteObject(cdsEmployee);
                await cds.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Delete called for employee {item.Id} but no employee with that id found in CDS");
            }
        }
        private async Task DeleteRideDetails(RideDetails item)
        {
            Microsoft.Dynamics.DataEntities.FabrikamRideShare_ cdsRideDetails = await FindRideDetails(item);

            if (cdsRideDetails != null)
            {
                cds.DeleteObject(cdsRideDetails);
                await cds.SaveChangesAsync();
            }
            else
            {
                throw new KeyNotFoundException($"Delete called for RideDetails {item.Id} but no RideDetails with that id found in CDS");
            }
        }
        public async Task AddRiderAsync(Driver driver, string newRider)
        {
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = await FindDriver(driver);

            if (cdsDriver != null)
            {
                if (String.IsNullOrEmpty(cdsDriver.Rider1Name))
                {
                    cdsDriver.Rider1Name = newRider;
                }
                else if (String.IsNullOrEmpty(cdsDriver.Rider2Name))
                {
                    cdsDriver.Rider2Name = newRider;
                }
                else if (String.IsNullOrEmpty(cdsDriver.Rider3Name))
                {
                    cdsDriver.Rider3Name = newRider;
                }
                else if (String.IsNullOrEmpty(cdsDriver.Rider4Name))
                {
                    cdsDriver.Rider4Name = newRider;
                }
                else
                {
                    throw new Exception("No rider spots left");
                }

                cds.UpdateObject(cdsDriver);

                await cds.SaveChangesAsync();
            }
        }

        public async Task UpdateRiderStatusAsync(Driver driver, string rider)
        {
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = await FindDriver(driver);

            if (cdsDriver != null)
            {
                if (rider.Equals(cdsDriver.Rider1Name))
                {
                    cdsDriver.Rider1Status = true;
                }
                if (rider.Equals(cdsDriver.Rider2Name))
                {
                    cdsDriver.Rider2Status = true;
                }
                if (rider.Equals(cdsDriver.Rider3Name))
                {
                    cdsDriver.Rider3Status = true;
                }
                if (rider.Equals(cdsDriver.Rider4Name))
                {
                    cdsDriver.Rider4Status = true;
                }

                cds.UpdateObject(cdsDriver);

                await cds.SaveChangesAsync();
            }
        }

        private async Task AdjustRiderStatus(Driver driver, string rider, bool status)
        {
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = await FindDriver(driver);

            if (cdsDriver != null)
            {
                if (rider.Equals(cdsDriver.Rider1Name))
                {
                    cdsDriver.Rider1Status = status;
                }
                if (rider.Equals(cdsDriver.Rider2Name))
                {
                    cdsDriver.Rider2Status = status;
                }
                if (rider.Equals(cdsDriver.Rider3Name))
                {
                    cdsDriver.Rider3Status = status;
                }
                if (rider.Equals(cdsDriver.Rider4Name))
                {
                    cdsDriver.Rider4Status = status;
                }

                cds.UpdateObject(cdsDriver);

                await cds.SaveChangesAsync();
            }
        }

        public async Task RejectRiderAsync(Driver driver, string rider)
        {
            Microsoft.Dynamics.DataEntities.FabrikamDrivers_ cdsDriver = await FindDriver(driver);

            if (cdsDriver != null)
            {
                if (rider.Equals(cdsDriver.Rider1Name))
                {
                    cdsDriver.Rider1Name = null;
                    cdsDriver.Rider1Status = false;
                }
                if (rider.Equals(cdsDriver.Rider2Name))
                {
                    cdsDriver.Rider2Name = null;
                    cdsDriver.Rider2Status = false;
                }
                if (rider.Equals(cdsDriver.Rider3Name))
                {
                    cdsDriver.Rider3Name = null;
                    cdsDriver.Rider3Status = false;
                }
                if (rider.Equals(cdsDriver.Rider4Name))
                {
                    cdsDriver.Rider4Name = null;
                    cdsDriver.Rider4Status = false;
                }

                cds.UpdateObject(cdsDriver);

                await cds.SaveChangesAsync();
            }
        }
    }
}
