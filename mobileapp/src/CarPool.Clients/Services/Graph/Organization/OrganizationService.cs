using Carpool.Clients.Services.Data;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models.Organization;
using System;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.Organization
{
    public class OrganizationService : IOrganizationService
    {
        private readonly IRequestProvider _requestProvider;

        public OrganizationService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<string> GetOrganizationLocationAsync()
        {
            var organization =  await GraphClient.Instance.Beta.Organization.Request().Expand("extensions").GetAsync();
            return (string)organization?.AdditionalData["com.contoso.ride2work"];
        }

        public async Task SaveOrganizationLocationAsync()
        {
            var authToken = Settings.TokenForUser;

            if (string.IsNullOrEmpty(authToken))
            {
                return;
            }

            try
            {
                UriBuilder builder = new UriBuilder(AppSettings.GraphApiEndpoint);
                builder.Path = $"beta/organization/98f4e585-4aaf-43cd-b5e7-d348e33dc12c/extensions";

                string uri = builder.ToString();

                var organizationLocation = new OrganizationLocation
                {
                    StreetName = "9825 Willows Road NE",
                    Country = "USA",
                    PostalCode = "98052",
                    State = "WA",
                    City = "Redmond"
                };

                await _requestProvider.PostAsync< OrganizationLocation, bool>(uri, organizationLocation, authToken);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error init OrganizationLocation: {e}");
            }
        }
    }
}
