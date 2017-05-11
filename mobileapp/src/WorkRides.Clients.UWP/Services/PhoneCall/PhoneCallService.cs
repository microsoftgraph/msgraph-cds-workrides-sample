using CarPool.Clients.Core.Services.PhoneCall;
using CarPool.Clients.UWP.Services.PhoneCall;
using Xamarin.Forms;
using Windows.Foundation.Metadata;
using Windows.ApplicationModel.Calls;

[assembly: Dependency(typeof(PhoneCallService))]
namespace CarPool.Clients.UWP.Services.PhoneCall
{
    public class PhoneCallService : IPhoneCallService
    {
        public static void Init() { }

        public void MakeCall(string phoneNumber)
        {
            if (ApiInformation.IsTypePresent("Windows.ApplicationModel.Calls.PhoneCallManager"))
            {
                PhoneCallManager.ShowPhoneCallUI(phoneNumber, phoneNumber);
            }
        }
    }
}
