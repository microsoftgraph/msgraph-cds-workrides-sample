using CarPool.Clients.Core.Views;
using CarPool.Clients.Droid.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(LoginView), typeof(LoginViewRenderer))]
namespace CarPool.Clients.Droid.Renderers
{
    public class LoginViewRenderer : PageRenderer
    {
        LoginView _page;

        protected override void OnElementChanged(ElementChangedEventArgs<Page> e)
        {
            base.OnElementChanged(e);
            _page = e.NewElement as LoginView;
        }
    }
}