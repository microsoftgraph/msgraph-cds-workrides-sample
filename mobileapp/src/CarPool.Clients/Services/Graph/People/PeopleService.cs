using Carpool.Clients.Services.Data;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.People
{
    public class PeopleService : IPeopleService
    {
        private readonly IRequestProvider _requestProvider;

        public PeopleService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }
        
        public async Task<List<CarPool.Clients.Core.Models.Driver>> FilterMyNearPeople(IEnumerable<CarPool.Clients.Core.Models.Driver> users)
        {
            List<CarPool.Clients.Core.Models.Driver> result = new List<CarPool.Clients.Core.Models.Driver>();

            var authToken = Settings.TokenForUser;

            if (string.IsNullOrEmpty(authToken) || users == null || !users.Any())
            {
                return result;
            }

            try
            {
                UriBuilder builder = new UriBuilder(AppSettings.GraphApiEndpoint);
                builder.Path = "beta/me/people";
                builder.Query = "$select=userPrincipalName&top=100";

                string uri = builder.ToString();

                var people = await _requestProvider.GetAsync<CarPool.Clients.Core.Models.People.RootGraphObject>(uri, authToken);

                if (people != null && people.People != null && people.People.Any())
                {
                    foreach (var person in people.People)
                    {
                        var driver = users.FirstOrDefault(u => u.Name.Equals(person.UserPrincipalName));
                        if (driver != null)
                        {
                            result.Add(driver);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting people {e}");
            }

            return result;
        }
    }
}