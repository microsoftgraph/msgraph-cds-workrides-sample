using CarPool.Clients.Core.Views;
using CarPool.Clients.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NewMessageView), typeof(NoTabbedPageRenderer))]
namespace CarPool.Clients.iOS.Renderers
{
    public class NoTabbedPageRenderer : PageRenderer
    {
        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            HidesBottomBarWhenPushed = true;

            var tabBarController = ParentViewController.TabBarController;

            if (TabBarController is BadgedTabbedPageRenderer)
            {
                var customTabbedRenderer = TabBarController as BadgedTabbedPageRenderer;
                customTabbedRenderer.IsTabBarVisible = false;
            }
        }

        public override void ViewWillDisappear(bool animated)
        {
            base.ViewWillDisappear(animated);

            var tabBarController = ParentViewController.TabBarController;

            if (TabBarController is BadgedTabbedPageRenderer)
            {
                var customTabbedRenderer = TabBarController as BadgedTabbedPageRenderer;
                customTabbedRenderer.IsTabBarVisible = true;
            }
        }
    }
}