using CarPool.Clients.Core.Controls;
using CarPool.Clients.Droid.Controls;
using CarPool.Clients.Droid.Extensions;
using System.ComponentModel;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using View = Android.Views.View;

[assembly:
ExportRenderer(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]
namespace CarPool.Clients.Droid.Controls
{
    public class RoundedBoxViewRenderer : ViewRenderer<RoundedBoxView, View>
    {
        public static void Init()
        {
        }

        private RoundedBoxView _formControl
        {
            get { return Element; }
        }

        protected override void OnElementChanged(ElementChangedEventArgs<RoundedBoxView> e)
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