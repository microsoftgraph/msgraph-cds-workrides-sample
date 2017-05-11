using Carpool.Clients.Services.Data;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Models;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.Services.Graph.User
{
    public class UserService : IUserService
    {
        private readonly IRequestProvider _requestProvider;

        public UserService(IRequestProvider requestProvider)
        {
            _requestProvider = requestProvider;
        }

        public async Task<Microsoft.Graph.User> GetUserByPrincipalNameAsync(string userPrincipalName)
        {
            try
            {
                if (!string.IsNullOrEmpty(userPrincipalName))
                {
                    var result = await GraphClient.Instance.Beta.Users.Request()
                        .Filter($"userPrincipalName eq '{userPrincipalName}'")
                        .Select("id,mail,displayName,givenName,jobTitle,department,businessPhones,mobilePhone,postalCode,streetAddress,state,userPrincipalName,city")
                        .GetAsync();

                    if (result.Any())
                    {
                        return result.First();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<Microsoft.Graph.User> GetUserByDisplayNameAsync(string displayName)
        {
            try
            {
                if (!string.IsNullOrEmpty(displayName))
                {
                    var result = await GraphClient.Instance.Beta.Users.Request()
                        .Filter($"displayName eq '{displayName}'")
                        .Select("id,mail,displayName,givenName,jobTitle,department,businessPhones,mobilePhone,postalCode,streetAddress,state,userPrincipalName,city")
                        .GetAsync();

                    if (result.Any())
                    {
                        return result.First();
                    }
                }

                return null;
            }
            catch
            {
                return null;
            }
        }

        public async Task<byte[]> GetUserPhotoAsync(string mail)
        {
            if (!string.IsNullOrEmpty(mail))
            {
                var authToken = Settings.TokenForUser;

                if (string.IsNullOrEmpty(authToken))
                {
                    return null;
                }

                try
                {
                    UriBuilder builder = new UriBuilder(AppSettings.GraphApiEndpoint);
                    builder.Path = $"beta/users/{mail}/photo/$value";

                    string uri = builder.ToString();

                    return await _requestProvider.GetAsyncContent(uri, authToken);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }

        public async Task<List<Models.Driver>> FilterMyDeparmentUsersAsync(string department, IEnumerable<Models.Driver> users)
        {
            List<Models.Driver> result = new List<Models.Driver>();

            try
            {
                if (!string.IsNullOrEmpty(department))
                {
                    var coworkers = await GraphClient.Instance.Beta.Users.Request()
                        .Select("userPrincipalName")
                        // Escape url characters
                        .Filter($"Department eq '{WebUtility.UrlEncode(department)}'")
                        .GetAsync();

                    if (coworkers != null && coworkers.Any())
                    {
                        foreach (var coworker in coworkers)
                        {
                            if (!string.IsNullOrEmpty(coworker.UserPrincipalName))
                            {
                                var driver = users?.FirstOrDefault(u => coworker.UserPrincipalName.Equals(u.Name));
                                if (driver != null)
                                {
                                    result.Add(driver);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting My Organization Users {e}");
            }

            return result;
        }

        public async Task GetOutOfOfficeAsync(IEnumerable<GraphUser> users)
        {
            var authToken = Settings.TokenForUser;

            if (string.IsNullOrEmpty(authToken) || users == null || !users.Any())
            {
                return;
            }

            try
            {
                UriBuilder builder = new UriBuilder(AppSettings.GraphApiEndpoint);
                builder.Path = $"beta/me/getMailTips";

                string uri = builder.ToString();

                var mailTips = new MailTips
                {
                    EmailAddresses = users
                             .Where(x => !string.IsNullOrEmpty(x.Mail))
                             .Select(x => x.Mail)
                             .ToArray(),
                    MailTipsOptions = "automaticReplies"
                };

                var result = await _requestProvider
                    .PostAsync<MailTips, RootGraphObject>(uri, mailTips, authToken);

                foreach (var response in result?.MailTips)
                {
                    if (!string.IsNullOrEmpty(response?.AutomaticReplies?.Message) && !string.IsNullOrEmpty(response?.EmailAddress?.Address))
                    {
                        // if {response.EmailAddress.Address} Its out of office
                        var user = users.FirstOrDefault(x => response.EmailAddress.Address.Equals(x.Mail));
                        user.OOF = true;
                    }
                }
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting OOF {e}");
            }
        }

        public async Task<string> InitializeDeltaQueryAsync()
        {
            var authToken = Settings.TokenForUser;

            if (string.IsNullOrEmpty(authToken))
            {
                return string.Empty;
            }

            UriBuilder builder = new UriBuilder(AppSettings.GraphApiEndpoint);
            builder.Path = $"beta/users/delta";
            // Odata fetch only attrs used
            builder.Query = "$select=id,mail,displayName,givenName,jobTitle,department,businessPhones,mobilePhone,postalCode,streetAddress,state,userPrincipalName,city";

            var uri = builder.ToString();

            var page = await _requestProvider.GetAsync<GraphServiceUsersCollectionResponse>(uri, authToken);

            while (page?.AdditionalData != null && !page.AdditionalData.ContainsKey("@odata.deltaLink"))
            {
                if (page.AdditionalData.ContainsKey("@odata.nextLink"))
                {
                    // Fetch next page
                    var nextLink = (string)page.AdditionalData["@odata.nextLink"];
                    page = await _requestProvider.GetAsync<GraphServiceUsersCollectionResponse>(nextLink, authToken);
                }
            }

            if (page.AdditionalData.ContainsKey("@odata.deltaLink"))
            {
                return page.AdditionalData["@odata.deltaLink"] as string;
            }

            return string.Empty;
        }

        public async Task<GraphServiceUsersCollectionResponse> GetDeltaQueryChangesAsync(string deltaLink)
        {
            var authToken = Settings.TokenForUser;

            if (string.IsNullOrEmpty(authToken))
            {
                return null;
            }

            try
            {
                return await _requestProvider.GetAsync<GraphServiceUsersCollectionResponse>(deltaLink, authToken);
            }
            catch
            {
                return null;
            }
        }
    }
}
