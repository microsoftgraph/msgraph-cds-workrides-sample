using System;
using System.Linq;
using System.Threading.Tasks;
using Android.App;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Xamarin.Forms;
using CarPool.Clients.Core.Helpers;

[assembly: Dependency(typeof(CarPool.Clients.Droid.Helpers.Authenticator))]
namespace CarPool.Clients.Droid.Helpers
{
    public class Authenticator : IAuthenticator
    {

        private AuthenticationContext authContext;

        public async Task<AuthenticationResult> Authenticate(string authority, string resource, string clientId, string returnUri)
        {
            authContext = new AuthenticationContext(authority);
            if (authContext.TokenCache.ReadItems().Any())
            {
                authContext = new AuthenticationContext(authContext.TokenCache.ReadItems().First().Authority);
            }
            var authResult = await authContext.AcquireTokenAsync(resource, clientId, new Uri(returnUri), new PlatformParameters((Activity)Forms.Context));
            return authResult;
        }

        public void Signout(string authority)
        {
            if (authContext == null)
            {
                authContext = new AuthenticationContext(authority);
            }
            authContext.TokenCache.Clear();
        }
    }
}