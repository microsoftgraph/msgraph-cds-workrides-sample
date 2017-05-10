using CarPool.Clients.Core.Controls;
using CarPool.Clients.UWP.Extensions;
using CarPool.Clients.UWP.Renderers;
using System.ComponentModel;
using Windows.UI.Xaml.Controls;
using Xamarin.Forms.Platform.UWP;

[assembly: ExportRenderer(typeof(RoundedBoxView), typeof(RoundedBoxViewRenderer))]
namespace CarPool.Clients.UWP.Renderers
{
    public class RoundedBoxViewRenderer : ViewRenderer<RoundedBoxView, Border>
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

            var border = new Border();

            if (_formControl != null)
            {
                border.InitializeFrom(_formControl);

                SetNativeControl(border);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            Control.UpdateFrom(_formControl, e.PropertyName);
        }
    }
}