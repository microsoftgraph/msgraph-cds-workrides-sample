using System;
using Foundation;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using Xamarin.Forms;
using System.Threading.Tasks;
using CarPool.Clients.iOS.Renderers;
using CarPool.Clients.Core.Controls;
using CarPool.Clients.Core.Views;

[assembly: ExportRenderer(typeof(iOSMainView), typeof(BadgedTabbedPageRenderer))]
namespace CarPool.Clients.iOS.Renderers
{
    [Preserve]
    public class BadgedTabbedPageRenderer : TabbedRenderer
    {
        private const float CustomTabBarHeight = 60.0f;
        private const float CustomItemImageInsetsSize = 2.0f;

        private UIEdgeInsets _itemImageInsets =
            new UIEdgeInsets(CustomItemImageInsetsSize, 0, -CustomItemImageInsetsSize, 0);

        private IPageController PageController => Element as IPageController;

        public bool IsTabBarVisible
        {
            get
            {
                return !TabBar.Hidden;
            }

            set
            {
                TabBar.Hidden = !value;
                HandleTabBarVisibility(TabBar.Hidden);
            }
        }

        public override void ViewWillLayoutSubviews()
        {
            var tabBarFrame = TabBar.Frame;
            var diff = CustomTabBarHeight - tabBarFrame.Height;

            TabBar.Frame = new CoreGraphics.CGRect(tabBarFrame.X, tabBarFrame.Y - diff, tabBarFrame.Width, tabBarFrame.Height + diff);

            // Adjust tab bar item image insets
            foreach (UITabBarItem item in TabBar.Items)
            {
                item.ImageInsets = _itemImageInsets;
            }

            base.ViewWillLayoutSubviews();
        }

        private void HandleTabBarVisibility(bool hidden)
        {
            var frame = View.Frame;

            if (IsTabBarVisible)
            {
                var tabBarFrame = TabBar.Frame;
                PageController.ContainerArea =
                    new Rectangle(0, 0, frame.Width, frame.Height - tabBarFrame.Height);
            }
            else
            {
                PageController.ContainerArea =
                    new Rectangle(0, 0, frame.Width, frame.Height);
            }
        }

        public override void ViewWillAppear(bool animated)
        {
            base.ViewWillAppear(animated);

            if (UIDevice.CurrentDevice.CheckSystemVersion(10, 0))
            {
                TabBar.SelectedImageTintColor = Color.FromHex("#F1C40F").ToUIColor();
                TabBar.UnselectedItemTintColor = UIColor.White;
                TabBar.BarTintColor = Color.FromHex("#061F76").ToUIColor();
                TabBar.BackgroundColor = Color.FromHex("#061F76").ToUIColor();
            }

            Element.ChildAdded += OnTabAdded;
            Element.ChildRemoved += OnTabRemoved;
        }

        private void AddTabBadge(int tabIndex)
        {
            var element = Tabbed.Children[tabIndex];
            element.PropertyChanged += OnTabbedPagePropertyChanged;

            if (TabBar.Items.Length > tabIndex)
            {
                var tabBarItem = TabBar.Items[tabIndex];
                UpdateTabBadgeText(tabBarItem, element);
                UpdateTabBadgeColor(tabBarItem, element);
            }
        }

        private void UpdateTabBadgeText(UITabBarItem tabBarItem, Element element)
        {
            var text = TabbedBadgePage.GetBadgeText(element);

            tabBarItem.BadgeValue = string.IsNullOrEmpty(text) ? null : text;
        }

        private void UpdateTabBadgeColor(UITabBarItem tabBarItem, Element element)
        {
            if (!tabBarItem.RespondsToSelector(new ObjCRuntime.Selector("setBadgeColor:")))
            {
                // Method not available, ios < 10
                Console.WriteLine("Badge color only available starting with iOS 10.0.");
                return;
            }

            var tabColor = element as CustomNavigationPage;
            if (tabColor.BadgeColor != Color.Default)
            {
                tabBarItem.BadgeColor = tabColor.BadgeColor.ToUIColor();
                tabBarItem.BadgeValue = tabColor.BadgeText;
            }
        }

        private void OnTabbedPagePropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            var page = sender as Page;

            if (page == null)
                return;

            if (e.PropertyName == TabbedBadgePage.BadgeTextProperty.PropertyName)
            {
                var tabIndex = Tabbed.Children.IndexOf(page);
                if (tabIndex < TabBar.Items.Length)
                    UpdateTabBadgeText(TabBar.Items[tabIndex], page);
                return;
            }

            if (e.PropertyName == TabbedBadgePage.BadgeColorProperty.PropertyName)
            {
                var tabIndex = Tabbed.Children.IndexOf(page);
                if (tabIndex < TabBar.Items.Length)
                    UpdateTabBadgeColor(TabBar.Items[tabIndex], page);
            }
        }

        private async void OnTabAdded(object sender, ElementEventArgs e)
        {
            // Workaround for XF, tabbar is not updated at this point and we have no way of 
            // knowing for sure when it will be updated. so we have to wait ... 
            await Task.Delay(10);

            var page = e.Element as Page;

            if (page == null)
                return;

            var tabIndex = Tabbed.Children.IndexOf(page);
            AddTabBadge(tabIndex);
        }

        private void OnTabRemoved(object sender, ElementEventArgs e)
        {
            e.Element.PropertyChanged -= OnTabbedPagePropertyChanged;
        }

        protected override void Dispose(bool disposing)
        {
            if (Tabbed != null)
            {
                foreach (var tab in Tabbed.Children)
                {
                    tab.PropertyChanged -= OnTabbedPagePropertyChanged;
                }

                Tabbed.ChildAdded -= OnTabAdded;
                Tabbed.ChildRemoved -= OnTabRemoved;
            }

            base.Dispose(disposing);
        }
    }
}