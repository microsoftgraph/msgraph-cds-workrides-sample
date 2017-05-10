using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace SampleFunctionApplication.Client
{
    class Program
    {
        public const string TenantNameOrId = "fabrikamco.onmicrosoft.com";
        public const string ClientAppId = "adfdd950-ecd3-4b3e-9257-57acbcb3fcdf";
        public const string RedirectUri = "http://localhost";
        public const string AzureHostedResetUriString = "https://carpoolweb20170410123358.azurewebsites.net/api/UpdateProductCategory?code=FN1gR498Hap2MdmoZwQo5xbgQRaEF4SRsU4l4u9U1qkhZrmBTXHWNQ=="; // Azure hosted Function URI
        public const string FunctionResourceId = "1b54d472-a1b0-4844-984a-98703db55428";

        public const string AzureFunctionBaseAPI = "https://carpoolweb20170410123358.azurewebsites.net/api";

        public static string Authority { get { return string.Format(AuthorityTemplate, TenantNameOrId); } }
        private const string AuthorityTemplate = "https://login.windows.net/{0}";
        public const string LocalHostedUpdateUriString = "http://localhost:7071/api/UpdateProductCategory"; // Locally hosted Function URI
        public const string AuthorizationHeaderScheme = "Bearer";

        static void Main(string[] args)
        {
            // Run operation as async function
            //UpdateEntityAsync().Wait();
            GetDrivers().Wait();

            Console.WriteLine("Press any key to continue ...");
            Console.ReadLine();
        }

        private static async Task UpdateEntityAsync()
        {
            // Get HTTP client and send request
            var client = await GetHttpClientAsync();
            bool isAzureHosted = true; //true or false
            var updateUriString = isAzureHosted ? $"{AzureHostedResetUriString}&name=Surface" : $"{LocalHostedUpdateUriString}?name=Surface";
            var response = await client.PostAsJsonAsync<object>(updateUriString, null);
            Console.WriteLine($"Status: '{response.StatusCode}'");
            Console.WriteLine($"Contents: {await response.Content.ReadAsStringAsync()}");
        }
        
        private static async Task GetDrivers()
        {
            // Get HTTP client and send request
            var client = await GetHttpClientAsync();
            var getDrivers = AzureFunctionBaseAPI + "/driver";
            var response = await client.GetAsync(getDrivers);
            Console.WriteLine($"Status: '{response.StatusCode}'");
            Console.WriteLine($"Contents: {await response.Content.ReadAsStringAsync()}");
        }
        
        private static async Task<HttpClient> GetHttpClientAsync()
        {
            Console.WriteLine($"Prompt for login and create security token...");
            // Prompt for login and create security token
            var authenticationContext = new AuthenticationContext(Authority);
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(FunctionResourceId
                , ClientAppId
                , new Uri(RedirectUri)
                , new PlatformParameters(PromptBehavior.Always));
            var securityTokenString = authenticationResult.CreateAuthorizationHeader();

            // Create and configure the HTTP client
            var client = new HttpClient();
            var authorizationHeaderParameter = securityTokenString.Replace(AuthorizationHeaderScheme + " ", string.Empty);
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(AuthorizationHeaderScheme, authorizationHeaderParameter);
            return client;
        }
    }
}
