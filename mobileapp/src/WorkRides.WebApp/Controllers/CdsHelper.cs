using Microsoft.CommonDataService;
using Microsoft.CommonDataService.Configuration;
using Microsoft.CommonDataService.ServiceClient.Security;
using System.Threading.Tasks;
using System.Net.Http;

namespace CarPool.WebApp.Controllers
{
    public class CdsHelper
    {
        public static async Task<Client> CreateClientAsync(HttpRequestMessage request)
        {
            //FabrikamCo domain - Details for Functions/WebAPI app
            var connection = new ConnectionSettings
            {
                Tenant = "fabrikamco.onmicrosoft.com",
                EnvironmentId = "d54d812d-efb0-4787-b3c2-69659fb4e77f",
                Credentials = new UserImpersonationCredentialsSettings
                {
                    ApplicationId = "1b54d472-a1b0-4844-984a-98703db55428",
                    ApplicationSecret = "zb9UYbs/Mp8kDH/78CaHnASwjA6KjskaMxrXa1ySS3k="
                }
            };


            var client = await connection.CreateClient(request);
            return client;
        }
    }
}