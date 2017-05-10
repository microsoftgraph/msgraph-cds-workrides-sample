using System;
using Android.Content;
using CarPool.Clients.Core.Services.PhoneCall;
using Xamarin.Forms;
using CarPool.Clients.Droid.Services.PhoneCall;

[assembly: Dependency(typeof(PhoneCallService))]
namespace CarPool.Clients.Droid.Services.PhoneCall
{
    public class PhoneCallService : IPhoneCallService
    {
        public static void Init() { }

        public void MakeCall(string phoneNumber)
        {
            if (System.Text.RegularExpressions.Regex.IsMatch(phoneNumber, "^(\\(?\\+?[0-9]*\\)?)?[0-9_\\- \\(\\)]*$"))
            {
                var uri = Android.Net.Uri.Parse(String.Format("tel:{0}", phoneNumber));
                var intent = new Intent(Intent.ActionView, uri);
                Forms.Context.StartActivity(intent);
            }
        }
    }
}