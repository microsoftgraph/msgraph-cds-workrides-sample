using CarPool.Clients.Core.Maps.Model;
using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace CarPool.Clients.iOS
{
    public partial class ResponderIconView : UIView
    {
        public ResponderIconView (IntPtr handle) : base (handle)
        {
        }

        public static ResponderIconView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("ResponderIconView", null, null);
            var v = Runtime.GetNSObject<ResponderIconView>(arr.ValueAt(0));

            return v;
        }

        public override void AwakeFromNib()
        {
        }

        public void LoadResponderData(CustomRider rider)
        {
            NameLabel.Text = rider.Acronym;
        }
    }
}