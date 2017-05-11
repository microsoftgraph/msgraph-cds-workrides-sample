using System;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Services.OpenMap
{
    public class OpenMapService : IOpenMapService
    {
        public void OpenMapAppWithRoute(string origin, string destination)
        {
            string url = string.Empty;

            switch (Device.RuntimePlatform)
            {
                case Device.Android:
                    url = String.Format("http://maps.google.com/maps?saddr={0}&daddr={1}&directionsmode=transit", 
                        origin,
                        destination);
                    break;
                // More information: https://developer.apple.com/library/content/featuredarticles/iPhoneURLScheme_Reference/MapLinks/MapLinks.html
                case Device.iOS:
                    url = String.Format("http://maps.apple.com/maps?saddr={0}&daddr={1}", 
                        origin,
                        destination);
                    break;
                case Device.Windows:
                    url = string.Format("bingmaps:?cp={0}&q={1}",
                        origin,
                        destination);
                    break;
            }
            
            var uri = new Uri(url);
            Device.OpenUri(uri);
        }
    }
}