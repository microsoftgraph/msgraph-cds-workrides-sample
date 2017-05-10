using CarPool.Clients.Core.ViewModels.Base;
using System;
using System.Linq;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Views
{
    public partial class iOSMainView : TabbedPage
    {
        public iOSMainView()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }

        public Page PreviousPage { get; set; }

        public void AddPage(Page page, string title, Color color, string badgeText)
        {
            var navigationPage = new CustomNavigationPage(page)
            {
                Title = title,
                Icon = GetIconForPage(page),
                BadgeColor = color,
                BadgeText = badgeText
            };

            if (PreviousPage == null)
            {
                PreviousPage = page;
            }

            Children.Add(navigationPage);
        }

        public void AddPage(Page page, string title)
        {
            var navigationPage = new CustomNavigationPage(page)
            {
                Title = title,
                Icon = GetIconForPage(page)
            };

            if (PreviousPage == null)
            {
                PreviousPage = page;
            }

            Children.Add(navigationPage);
        }

        public bool TrySetCurrentPage(Page requiredPage)
        {
            return TrySetCurrentPage(requiredPage.GetType());
        }

        public bool TrySetCurrentPage(Type requiredPageType)
        {
            CustomNavigationPage page = GetTabPageWithInitial(requiredPageType);

            if (page != null)
            {
                CurrentPage = null;
                CurrentPage = page;
            }

            return page != null;
        }

        private CustomNavigationPage GetTabPageWithInitial(Type type)
        {
            CustomNavigationPage page = Children.OfType<CustomNavigationPage>()
                .FirstOrDefault(p =>
                {
                    return p.CurrentPage.Navigation.NavigationStack.Count > 0
                        ? p.CurrentPage.Navigation.NavigationStack[0].GetType() == type
                        : false;
                });

            return page;
        }

        private string GetIconForPage(Page page)
        {
            string icon = string.Empty;

            if (page is ScheduleView)
            {
                icon = "menu_schedule";
            }
            else if (page is FindRideView)
            {
                icon = "menu_find";
            }
            else if (page is ProfileView)
            {
                icon = "menu_settings";
            }
            else if (page is DriveView)
            {
                icon = "menu_drive";
            }

            page.AutomationId = icon;

            return icon;
        }

        private void OnCurrentPageChanged(object sender, EventArgs e)
        {
            if (CurrentPage == null)
            {
                return;
            }

            if (!CurrentPage.IsEnabled)
            {
                CurrentPage = PreviousPage;
            }
            else
            {
                PreviousPage = CurrentPage;
                MessagingCenter.Send(this, MessengerKeys.iOSMainPageCurrentChanged);
            }
        }
    }
}