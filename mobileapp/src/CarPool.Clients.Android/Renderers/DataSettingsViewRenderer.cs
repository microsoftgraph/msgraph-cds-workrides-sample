using Android.App;
using CarPool.Clients.Core.Views;
using CarPool.Clients.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using Xamarin.Forms.Platform.Android.AppCompat;

[assembly: ExportRenderer(typeof(DataSettingsView), typeof(DataSettingsViewRenderer))]
namespace CarPool.Clients.Droid.Renderers
{
    public class DataSettingsViewRenderer : TabbedPageRenderer
    {
        DataSettingsView _page;

        protected override void OnElementChanged(ElementChangedEventArgs<TabbedPage> e)
        {
            base.OnElementChanged(e);
            _page = e.NewElement as DataSettingsView;
            var activity = this.Context as Activity;
        }
    }
}