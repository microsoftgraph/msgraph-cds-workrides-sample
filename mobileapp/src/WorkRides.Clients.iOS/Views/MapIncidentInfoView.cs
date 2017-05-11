using CarPool.Clients.Core.Maps.Model;
using Foundation;
using ObjCRuntime;
using System;
using UIKit;

namespace CarPool.Clients.iOS
{
    public partial class MapIncidentInfoView : UIView
    {
        public event EventHandler OnNavigationRequested;
        public event EventHandler OnClose;

        public MapIncidentInfoView (IntPtr handle) : base (handle)
        {
        }

        public static MapIncidentInfoView Create()
        {
            var arr = NSBundle.MainBundle.LoadNib("MapIncidentInfoView", null, null);
            var v = Runtime.GetNSObject<MapIncidentInfoView>(arr.ValueAt(0));

            return v;
        }

        public override void AwakeFromNib()
        {
        }

        public void LoadIncidentData(CustomPin pin)
        {
            if (pin.Duration.HasValue)
            {
                DescriptionLabel.Text = string.Format("{0:0.#} min added", pin.Duration.Value);
                DescriptionLabel.LineBreakMode = UILineBreakMode.TailTruncation;
                DescriptionLabel.Lines = 2;
            }
        }

        private void OnCloseTapped()
        {
            var handler = OnClose;

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnNavigateTapped()
        {
            var handler = OnNavigationRequested;

            handler?.Invoke(this, EventArgs.Empty);
        }
    }
}