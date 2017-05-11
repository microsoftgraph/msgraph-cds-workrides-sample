using CarPool.Clients.Core.Controls;
using CarPool.Clients.iOS.Controls.RoundedBoxView;
using CarPool.Clients.iOS.Extensions;
using System;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]
namespace CarPool.Clients.iOS.Controls.RoundedBoxView
{
    public class RoundedBoxViewRenderer : BoxRenderer
    {
        public static void Init()
        {
            var temp = DateTime.Now;
        }

        private Core.Controls.RoundedBoxView _formControl
        {
            get { return Element as Core.Controls.RoundedBoxView; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<BoxView> e)
        {
            base.OnElementChanged(e);

            this.InitializeFrom(_formControl);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            this.UpdateFrom(_formControl, e.PropertyName);
        }
    }
}