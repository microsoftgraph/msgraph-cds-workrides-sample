using CarPool.Clients.Core.Services.PhoneCall;
using CarPool.Clients.iOS.Services.PhoneCall;
using Foundation;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(PhoneCallService))]
namespace CarPool.Clients.iOS.Services.PhoneCall
{
    public class PhoneCallService : IPhoneCallService
    {
        public static void Init() { }

        public void MakeCall(string phoneNumber)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, "^(\\(?\\+?[0-9]*\\)?)?[0-9_\\- \\(\\)]*$"))
            {
                NSUrl url = new NSUrl(string.Format(@"telprompt://{0}", phoneNumber));
                UIApplication.SharedApplication.OpenUrl(url);
            }
        }
    }
}