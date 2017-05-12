using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CarPool.Clients.Core.Models;

using Microsoft.OData.Client;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CarPool.Clients.Core.Services.Data
{
    public class CDSDataProvider : IDataService
    {
        public async Task InitializeAsync(string token = "")
        {
            // Does nothing
            await Task.Delay(0);
        }

        // Local host 
        private const string localServiceApiUri = "http://localhost:9689/api";
        //Use this variation for fiddler visibility from console application. Documented as solution 2 here: http://docs.telerik.com/fiddler/observe-traffic/troubleshooting/notraffictolocalhost
        //private const string localServiceApiUri = "http://localhost.fiddler:9689/api";

        //Azure hosted 
        //Published as a web app from Visual Studio: https://docs.microsoft.com/en-us/azure/app-service-web/app-service-web-get-started-dotnet
        private const string azureServiceApiUri = "http://carpoolwebapi20170508025245.azurewebsites.net/api";

        private string serviceApiUri;

        private const string ApplicationJSON = "application/json";
        private const string RideDetailsAPI = "/RideDetails";
        private const string EmployeeAPI = "/Employee";
        private const string DriverAPI = "/Driver";


        public CDSDataProvider(bool hitLocalService = false)
        {
            serviceApiUri = hitLocalService ? localServiceApiUri : azureServiceApiUri;
        }

        public CDSDataProvider(IPlatformParameters _platformParameters, bool hitLocalService = false) : this(hitLocalService)
        {
            platformParameters = _platformParameters;
        }        

        private IPlatformParameters platformParameters;

        private static AuthenticationHeaderValue authHeader;
        private AuthenticationHeaderValue AuthHeader
        {
            get
            {
                if (authHeader == null)
                {
                    var authResult = AccessToken();
                    var authHeaderString = authResult.CreateAuthorizationHeader();
                    var authorizationSchemeAndParameter = authHeaderString.Split(' ');
                    authHeader = new AuthenticationHeaderValue(authorizationSchemeAndParameter[0], authorizationSchemeAndParameter[1]);
                }
                return authHeader;
            }
        }

        
        private AuthenticationResult AccessToken()
        {
            string resourceUri = "https://fabrikamco.onmicrosoft.com/c6e243df-7fa8-454c-8fc4-0089dc7574e2";
            string clientId = "adfdd950-ecd3-4b3e-9257-57acbcb3fcdf"; //Point to app that has permissions to the functions app
            string redirectUri = "http://localhost/";



            // Create an instance of AuthenticationContext to acquire an Azure access token  
            // OAuth2 authority Uri  
            string authorityUri = "https://login.windows.net/common/oauth2/authorize";
            AuthenticationContext authContext = new AuthenticationContext(authorityUri);

            // Call AcquireToken to get an Azure token from Azure Active Directory token issuance endpoint  
            //  AcquireToken takes a Client Id that Azure AD creates when you register your client app.  
            AuthenticationResult result = authContext.AcquireTokenAsync(resourceUri,
                                                    clientId,
                                                    new Uri(redirectUri),
                                                    this.platformParameters).Result;
            return result;           
        }

        public async Task<IEnumerable<Driver>> GetAllDriversAsync()
        {
            return await GetAllRecordsAsync<Driver>(DriverAPI);
        }
        
        public async Task<IEnumerable<Employee>> GetAllEmployeesAsync()
        {
            return await GetAllRecordsAsync<Employee>(EmployeeAPI);
        }

        public async Task<IEnumerable<RideDetails>> GetAllRideDetailsAsync()
        {
            return await GetAllRecordsAsync<RideDetails>(RideDetailsAPI);
        }

        private async Task<IEnumerable<T>> GetAllRecordsAsync<T>(string api)
        {
            //Prepare to make HTTP request
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = this.AuthHeader;

            //Build URI string with web API as the base and the specific API added as a suffix
            var uriString = serviceApiUri + api;

            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJSON));

            //Make GET request and capture response
            HttpResponseMessage response = await client.GetAsync(uriString);
            var responseString = await response.Content.ReadAsStringAsync();

            //Transform JSON response into array of data model objects
            var results = JsonConvert.DeserializeObject<T[]>(responseString);

            return results;
        }

        public async Task<Driver> GetDriverAsync(string itemId)
        {
            return await GetRecordAsync<Driver>(DriverAPI, itemId);
        }

        public async Task<Employee> GetEmployeeAsync(string itemId)
        {
            return await GetRecordAsync<Employee>(EmployeeAPI, itemId);
        }

        public async Task<RideDetails> GetRideDetailAsync(string itemId)
        {
            return await GetRecordAsync<RideDetails>(RideDetailsAPI, itemId);
        }

        private async Task<T> GetRecordAsync<T>(string api, string itemId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = this.AuthHeader;
            var uriString = serviceApiUri + api + "/" + itemId;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJSON));
            HttpResponseMessage response = await client.GetAsync(uriString);
            var responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<T>(responseString);

            return result;
        }

        public async Task InsertAsync<T>(T item)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Driver))
            {
                await InsertDriver(item as Driver);
            }
            if (itemType == typeof(Employee))
            {
                await InsertEmployee(item as Employee);
            }
            if (itemType == typeof(RideDetails))
            {
                await InsertRideDetails(item as RideDetails);
            }
        }

        private async Task InsertDriver(Driver item)
        {
            await InsertRecord<Driver>(item, DriverAPI);
       }

        private async Task InsertEmployee(Employee item)
        {
            await InsertRecord<Employee>(item, EmployeeAPI);
       }

        private async Task InsertRideDetails(RideDetails item)
        {
            await InsertRecord<RideDetails>(item, RideDetailsAPI);
        }

        private async Task InsertRecord<T>(T item, string api)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = this.AuthHeader;
            var updateUriString = serviceApiUri + api;
            var requestString = JsonConvert.SerializeObject(item);
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJSON));
            HttpResponseMessage response = await client.PostAsync(updateUriString, new StringContent(requestString, Encoding.UTF8, ApplicationJSON));
        }

        public async Task InsertOrUpdateDriverAsync(Driver item)
        {
            await InsertOrUpdateRecordAsync<Driver>(item, DriverAPI, item.Id);
        }

        public async Task InsertOrUpdateEmployeeAsync(Employee item)
        {
            await InsertOrUpdateRecordAsync<Employee>(item, EmployeeAPI, item.Id);
        }

        public async Task InsertOrUpdateRideDetailsAsync(RideDetails item)
        {
            await InsertOrUpdateRecordAsync<RideDetails>(item, RideDetailsAPI, item.Id);
        }

        private async Task InsertOrUpdateRecordAsync<T>(T item, string api, string itemId)
        {
            if (string.IsNullOrEmpty(itemId))
            {
                throw new Exception("InsertOrUpdateRecordAsync called for update, but doing an insert because itemId: '" + itemId + "'");
                await InsertRecord<T>(item, api);
            }
            else
            {
                var client = new HttpClient();
                client.DefaultRequestHeaders.Authorization = this.AuthHeader;
                var updateUriString = serviceApiUri + api + "/" + itemId;
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJSON));
                var requestString = JsonConvert.SerializeObject(item);
                HttpResponseMessage response = await client.PutAsync(updateUriString, new StringContent(requestString, Encoding.UTF8, ApplicationJSON));
            }
        }


        public async Task Remove<T>(T item)
        {
            Type itemType = typeof(T);

            if (itemType == typeof(Driver))
            {
                await DeleteDriver(item as Driver);
            }
            if (itemType == typeof(Employee))
            {
                await DeleteEmployee(item as Employee);
            }
            if (itemType == typeof(RideDetails))
            {
                await DeleteRideDetails(item as RideDetails);
            }
        }

        private async Task DeleteDriver(Driver item)
        {
            await DeleteRecord(DriverAPI, item.Id);
        }

        private async Task DeleteEmployee(Employee item)
        {
            await DeleteRecord(EmployeeAPI, item.Id);
        }
        private async Task DeleteRideDetails(RideDetails item)
        {
            await DeleteRecord(RideDetailsAPI, item.Id);
        }

        private async Task DeleteRecord(string api, string itemId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Authorization = this.AuthHeader;
            var updateUriString = serviceApiUri + api + "/" + itemId;
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(ApplicationJSON));
            HttpResponseMessage response = await client.DeleteAsync(updateUriString);
        }

        public async Task AddRiderAsync(Driver driver, string newRider)
        {
            Driver cdsDriver = await GetDriverAsync(driver.Id);

            if (cdsDriver != null)
            {
                if (String.IsNullOrEmpty(cdsDriver.Rider1))
                {
                    cdsDriver.Rider1 = newRider;
                }
                else if (String.IsNullOrEmpty(cdsDriver.Rider2))
                {
                    cdsDriver.Rider2 = newRider;
                }
                else if (String.IsNullOrEmpty(cdsDriver.Rider3))
                {
                    cdsDriver.Rider3 = newRider;
                }
                else if (String.IsNullOrEmpty(cdsDriver.Rider4))
                {
                    cdsDriver.Rider4 = newRider;
                }
                else
                {
                    throw new Exception("No rider spots left");
                }

                await InsertOrUpdateDriverAsync(cdsDriver);
            }
        }

        public async Task UpdateRiderStatusAsync(Driver driver, string rider)
        {
            Driver cdsDriver = await GetDriverAsync(driver.Id);

            if (cdsDriver != null)
            {
                if (rider.Equals(cdsDriver.Rider1))
                {
                    cdsDriver.Rider1Status = true;
                }
                if (rider.Equals(cdsDriver.Rider2))
                {
                    cdsDriver.Rider2Status = true;
                }
                if (rider.Equals(cdsDriver.Rider3))
                {
                    cdsDriver.Rider3Status = true;
                }
                if (rider.Equals(cdsDriver.Rider4))
                {
                    cdsDriver.Rider4Status = true;
                }

                await InsertOrUpdateDriverAsync(cdsDriver);
            }
        }

        private async Task AdjustRiderStatus(Driver driver, string rider, bool status)
        {
            Driver cdsDriver = await GetDriverAsync(driver.Id);

            if (cdsDriver != null)
            {
                if (rider.Equals(cdsDriver.Rider1))
                {
                    cdsDriver.Rider1Status = status;
                }
                if (rider.Equals(cdsDriver.Rider2))
                {
                    cdsDriver.Rider2Status = status;
                }
                if (rider.Equals(cdsDriver.Rider3))
                {
                    cdsDriver.Rider3Status = status;
                }
                if (rider.Equals(cdsDriver.Rider4))
                {
                    cdsDriver.Rider4Status = status;
                }

                await InsertOrUpdateDriverAsync(cdsDriver);
            }
        }

        public async Task RejectRiderAsync(Driver driver, string rider)
        {
            Driver cdsDriver = await GetDriverAsync(driver.Id);

            if (cdsDriver != null)
            {
                if (rider.Equals(cdsDriver.Rider1))
                {
                    cdsDriver.Rider1 = null;
                    cdsDriver.Rider1Status = false;
                }
                if (rider.Equals(cdsDriver.Rider2))
                {
                    cdsDriver.Rider2 = null;
                    cdsDriver.Rider2Status = false;
                }
                if (rider.Equals(cdsDriver.Rider3))
                {
                    cdsDriver.Rider3 = null;
                    cdsDriver.Rider3Status = false;
                }
                if (rider.Equals(cdsDriver.Rider4))
                {
                    cdsDriver.Rider4 = null;
                    cdsDriver.Rider4Status = false;
                }

                await InsertOrUpdateDriverAsync(cdsDriver);
            }
        }
    }
}
