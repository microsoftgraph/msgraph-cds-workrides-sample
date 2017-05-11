using CarPool.Clients.Core.Views;
using CarPool.Clients.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(LoginView), typeof(LoginViewRenderer))]
namespace CarPool.Clients.iOS.Renderers
{
    public class LoginViewRenderer : PageRenderer
    {
        private LoginView _page;

        protected override void OnElementChanged(VisualElementChangedEventArgs e)
        {
            base.OnElementChanged(e);
            _page = e.NewElement as LoginView;
        }
        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
        }
    }
}