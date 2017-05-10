using CarPool.Clients.Core.Helpers;
using Microsoft.Graph;
using System;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Services.Graph
{
    public class GraphClient
    {
        private static string TokenForUser = null;
        private static DateTimeOffset _expiration;

        private static GraphClient _graphClient { get; set; }

        public GraphServiceClient Beta { get; set; }
        
        private GraphClient()
        {
        }

        public static GraphClient Instance
        {
            get
            {
                if (_graphClient == null)
                {
                    _graphClient = new GraphClient();
                }

                return _graphClient;
            }
        }

        public async Task<Microsoft.Graph.User> Authenticate()
        {
            Beta = new GraphServiceClient(
                "https://graph.microsoft.com/beta",
                new DelegateAuthenticationProvider(
                    async (requestMessage) =>
                    {
                        var token = await GetTokenForUserAsync();
                        requestMessage.Headers.Authorization = new AuthenticationHeaderValue("bearer", token);
                    }));
            
            return await Instance.Beta.Me
                .Request()
                .Select("id,mail,displayName,givenName,jobTitle,department,businessPhones,mobilePhone,postalCode,streetAddress,state,userPrincipalName,city")
                .Expand("extensions")
                .GetAsync();
        }

        public bool IsAuthenticated()
        {
            if (string.IsNullOrEmpty(TokenForUser))
            {
                TokenForUser = Settings.TokenForUser;
            }

            if (_expiration == default(DateTimeOffset))
            {
                var expiration = Settings.Expiration;

                if (!string.IsNullOrEmpty(expiration))
                {
                    _expiration = DateTimeOffset.Parse(expiration);
                }
            }

            return !string.IsNullOrEmpty(TokenForUser) && _expiration >= DateTimeOffset.UtcNow;
        }

public void SignOut()
{
    DependencyService.Get<IAuthenticator>().Signout(AppSettings.GraphAuthorityUri);

    _graphClient = null;
    TokenForUser = null;
    Settings.TokenForUser = null;
    Settings.Expiration = null;
}

private async Task<string> GetTokenForUserAsync()
{
    if (string.IsNullOrEmpty(TokenForUser) || _expiration <= DateTimeOffset.UtcNow)
    {

        var data = await DependencyService.Get<IAuthenticator>()
            .Authenticate(AppSettings.GraphAuthorityUri, 
                AppSettings.GraphApiEndpoint, 
                AppSettings.CarpoolClientId, 
                AppSettings.GraphRedirectUri);

        TokenForUser = data.AccessToken;
        Settings.TokenForUser = TokenForUser;
        _expiration = data.ExpiresOn;
        Settings.Expiration = _expiration.ToString();
    }

    return TokenForUser;
}
    }
}