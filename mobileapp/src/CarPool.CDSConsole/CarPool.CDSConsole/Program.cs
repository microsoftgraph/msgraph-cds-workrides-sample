using Microsoft.CommonDataService;
using Microsoft.CommonDataService.Configuration;
using Microsoft.CommonDataService.ServiceClient.Security;
using System;
using System.Collections.Generic;
using CarPool.Web.Library;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace CarPool.CDSConsole
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using (var client = ConnectionSettings.Instance.CreateClient().Result)
            {
                Console.WriteLine("Create query");
                var queryEmployee = client.GetRelationalEntitySet<Employee>()
                                  .CreateQueryBuilder()
                                  .Project(entity => entity
                                                            .SelectField(f => f.PrimaryId)
                                                            .SelectField(f => f.FullName)
                                                            .SelectField(f => f.HomeAddress)
                                                            .SelectField(f => f.WorkAddress)
                                                            .SelectField(f => f.PreferredArrivalTimeAtWork)
                                                            .SelectField(f => f.PreferredDepartureTimeFromWork)
                                                            .SelectField(f => f.Email)
                                                            .SelectField(f => f.Phone)
                                                            .SelectField(f => f.BusinessUnit)
                                                            .SelectField(f => f.HomeLatitude)
                                                            .SelectField(f => f.HomeLongitude)
                                                            .SelectField(f => f.WorkLatitude)
                                                            .SelectField(f => f.WorkLongitude)
                                                            );

                Console.WriteLine("Run query");
                OperationResult<IReadOnlyList<Employee>> queryResultEmployee = null;
                client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                      .Query(queryEmployee, out queryResultEmployee).ExecuteAsync().Wait();

                foreach (var i in queryResultEmployee.Result)
                {
                    if (i.HomeAddress != null)
                    {
                        Console.WriteLine($"Employee record {i.PrimaryId} '{i.FullName}', home address {i.HomeAddress.Line1}");
                    }
                    else
                    {
                        Console.WriteLine($"Employee record {i.PrimaryId} '{i.FullName}', home address NOT SUPPLIED");
                    }
                }

                Console.WriteLine("Query count for Employee: " + queryResultEmployee.Result.Count.ToString());


                Console.WriteLine("Create query");
                var queryDriver = client.GetRelationalEntitySet<Driver>()
                                  .CreateQueryBuilder()
                                  .Project(entity => entity
                                                            .SelectField(f => f.PrimaryId)
                                                            .SelectField(f => f.RouteTitle)
                                                            .SelectField(f => f.AverageMiles)
                                                            .SelectField(f => f.RatePerMile)
                                                            .SelectField(f => f.Name)
                                                            .SelectField(f => f.DisplayName)
                                                            .SelectField(f => f.Schedule)
                                                            .SelectField(f => f.HomeLatitude)
                                                            .SelectField(f => f.HomeLongitude)
                                                            .SelectField(f => f.Arrival)
                                                            .SelectField(f => f.Departure)
                                                            .SelectField(f => f.Rider1Name)
                                                            .SelectField(f => f.Rider1Status)
                                                            .SelectField(f => f.Rider2Name)
                                                            .SelectField(f => f.Rider2Status)
                                                            .SelectField(f => f.Rider3Name)
                                                            .SelectField(f => f.Rider3Status)
                                                            .SelectField(f => f.Rider4Name)
                                                            .SelectField(f => f.Rider4Status)
                                                            );

                Console.WriteLine("Run query");
                OperationResult<IReadOnlyList<Driver>> queryResultDriver = null;
                client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                      .Query(queryDriver, out queryResultDriver).ExecuteAsync().Wait();

                if (queryResultDriver.Result != null)
                {
                    foreach (var i in queryResultDriver.Result)
                    {
                        Console.WriteLine($"Driver record {i.PrimaryId} '{i.DisplayName}', average miles {i.AverageMiles.ToString()}");
                    }
                    Console.WriteLine("Query count for Driver: " + queryResultDriver.Result.Count.ToString());
                }


                Console.WriteLine("Create query");
                var queryRideShare = client.GetRelationalEntitySet<RideShare>()
                                  .CreateQueryBuilder()
                                  .Project(entity => entity
                                                            .SelectField(f => f.PrimaryId)
                                                            .SelectField(f => f.ExpenseReportSubmitted)
                                                            .SelectField(f => f.ExpenseReportApproved)
                                                            .SelectField(f => f.Passenger1OnRide)
                                                            .SelectField(f => f.Passenger2OnRide)
                                                            .SelectField(f => f.Passenger3OnRide)
                                                            .SelectField(f => f.Passenger4OnRide)
                                                            .SelectField(f => f.RideCompleted)
                                                            .SelectField(f => f.FinanceApprovalStatus)
                                                            .SelectField(f => f.Passenger1Name)
                                                            .SelectField(f => f.Passenger2Name)
                                                            .SelectField(f => f.Passenger3Name)
                                                            .SelectField(f => f.Passenger4Name)
                                                            .SelectField(f => f.DistanceString)
                                                            .SelectField(f => f.RideDateTime)
                                                            .SelectField(f => f.DriverName)
                                                            .SelectField(f => f.RouteDescription)
                                                            .SelectField(f => f.DistanceString)
                                                            .SelectField(f => f.Distance)
                                                            );

                Console.WriteLine("Run query");
                OperationResult<IReadOnlyList<RideShare>> queryResultRideShare = null;
                client.CreateRelationalBatchExecuter(RelationalBatchExecutionMode.Transactional)
                      .Query(queryRideShare, out queryResultRideShare).ExecuteAsync().Wait();

                if (queryResultRideShare.Result != null)
                {
                    foreach (var i in queryResultRideShare.Result)
                    {
                        Console.WriteLine($"RideShare record {i.PrimaryId}, distance {i.Distance.ToString()}");
                    }
                    Console.WriteLine("Query count for RideShare: " + queryResultRideShare.Result.Count.ToString());
                }
            }

            // Now make a Graph call using the same AAD Application Client ID
            // Get HTTP client and send request
            //GetUsers();

            Console.WriteLine("Press any key to continue ...");
            Console.ReadKey();
        }

        public static void GetUsers()
        {
            // Get HTTP client and send request
            var myHTTPClient = GetHttpClientAsync();
            var getMe = "https://graph.microsoft.com/beta/me";
            var response = myHTTPClient.GetAsync(getMe).Result;
            var me = JsonConvert.DeserializeObject<UserModel>(response.Content.ReadAsStringAsync().Result);
            // Console.WriteLine($"Status: '{response.StatusCode}'");
            // Console.WriteLine($"Contents: {response.Content.ReadAsStringAsync().Result}");
            Console.WriteLine($"Name of me in Microsoft Graph: '{me.DisplayName}'");
            var getUsers = "https://graph.microsoft.com/beta/users";
            response = myHTTPClient.GetAsync(getUsers).Result;
            // Console.WriteLine($"Status: '{response.StatusCode}'");
            // Console.WriteLine($"Contents: {response.Content.ReadAsStringAsync().Result}");
            var users = JsonConvert.DeserializeObject<Users>(response.Content.ReadAsStringAsync().Result);
            if (users.Value != null)
            {
                Console.WriteLine($"User count: '{users.Value.Count}'");
                foreach (var user in users.Value)
                {
                    Console.WriteLine($"User returned: '{user.DisplayName}'");
                }
            }
        }

        public static string ClientAppId = "7a6b341a-4db6-4e58-b7c0-d602792e5e5c";
        public static string RedirectUri = "http://localhost";
        public static string TenantNameOrId = "fabrikamco.onmicrosoft.com";
        public static string ResourceId = "https://graph.microsoft.com/";

        public static string Authority { get { return string.Format(AuthorityTemplate, TenantNameOrId); } }
        private const string AuthorityTemplate = "https://login.windows.net/{0}";
        public const string AuthorizationHeaderScheme = "Bearer";


        private static HttpClient GetHttpClientAsync()
        {
            Console.WriteLine($"Prompt for login and create security token...");
            // Prompt for login and create security token
            var authenticationContext = new AuthenticationContext(Authority);
            AuthenticationResult authenticationResult = authenticationContext.AcquireTokenAsync(ResourceId
                , ClientAppId
                , new Uri(RedirectUri)
                , new PlatformParameters(PromptBehavior.Auto)).Result;
            var securityTokenString = authenticationResult.CreateAuthorizationHeader();

            // Create and configure the HTTP client
            var client = new HttpClient();
            var authorizationHeaderParameter = securityTokenString.Replace(AuthorizationHeaderScheme + " ", string.Empty);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationHeaderScheme, authorizationHeaderParameter);
            return client;
        }

    }
}
