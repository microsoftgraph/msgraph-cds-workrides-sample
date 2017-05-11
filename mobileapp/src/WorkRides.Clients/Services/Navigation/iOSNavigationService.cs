using CarPool.Clients;
using CarPool.Clients.Core.Controls;
using CarPool.Clients.Core.Services.Graph.Driver;
using CarPool.Clients.Core.ViewModels;
using CarPool.Clients.Core.ViewModels.Base;
using CarPool.Clients.Core.Views;
using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Carpool.Clients.Services.Navigation
{
    public partial class iOSNavigationService : NavigationService
    {
        private Type _requestedPageType;
        private object _requestedNavigationParameter;

        private IDriverService _driverService;

        public iOSNavigationService(IDriverService driverService)
            : base()
        {
            _driverService = driverService;

            CreatePageViewModelMappings();

            MessagingCenter.Subscribe<iOSMainView>(this, MessengerKeys.iOSMainPageCurrentChanged, 
                OnMainViewCurrentChanged);
        }

        public override Task RemoveLastFromBackStackAsync()
        {
            if (CurrentApplication.MainPage is iOSMainView)
            {
                var mainView = CurrentApplication.MainPage as iOSMainView;
                var currentPage = mainView.CurrentPage as CustomNavigationPage;

                if (currentPage != null && currentPage.Navigation.NavigationStack.Count > 1)
                {
                    currentPage.Navigation.RemovePage(
                        currentPage.Navigation.NavigationStack[currentPage.Navigation.NavigationStack.Count - 2]);
                }
            }

            return Task.FromResult(true);
        }

        public override async Task NavigateBackAsync()
        {
            if (CurrentApplication.MainPage is iOSMainView)
            {
                var mainPage = CurrentApplication.MainPage as iOSMainView;
                var currentPage = mainPage.CurrentPage as CustomNavigationPage;

                await currentPage?.Navigation.PopAsync();
            }
            else if (CurrentApplication.MainPage is CustomNavigationPage)
            {
                var currentPage = CurrentApplication.MainPage as CustomNavigationPage;
                await currentPage.Navigation.PopAsync();
            }
        }

        protected override async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);
            _requestedPageType = page.GetType();
            _requestedNavigationParameter = parameter;

            if (page is iOSMainView)
            {
                InitalizeMainPage(page as iOSMainView);
                await InitializeTabPageCurrentPageViewModelAsync(parameter);
                await InitializeMainPageViewModelAsync(parameter);
            }
            else if (page is LoginView)
            {
                var mainView = CurrentApplication.MainPage as iOSMainView;

                if (mainView != null)
                {
                    mainView.CurrentPageChanged -= MainViewPageChanged;
                }

                SetMainPage(new CustomNavigationPage(page));
                await InitializePageViewModelAsync(page, parameter);
            }
            else if (CurrentApplication.MainPage is iOSMainView)
            {
                var mainPage = CurrentApplication.MainPage as iOSMainView;
                bool tabPageFound = mainPage.TrySetCurrentPage(page);
                SetNavigationIcon(page);

                if (tabPageFound)
                {
                    var navigationPage = mainPage.CurrentPage as CustomNavigationPage;

                    if (navigationPage != null)
                    {
                        await InitializePageViewModelAsync(navigationPage.CurrentPage, parameter);
                    }
                }
                else
                {
                    await mainPage.CurrentPage.Navigation.PushAsync(page);
                    await InitializePageViewModelAsync(page, parameter);
                }
            }
            else
            {
                var navigationPage = CurrentApplication.MainPage as CustomNavigationPage;

                if (navigationPage != null)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    SetMainPage(new CustomNavigationPage(page));
                }

                await InitializePageViewModelAsync(page, parameter);
            }
        }

        private Task InitializePageViewModelAsync(Page page, object parameter)
        {
            return (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        private Task InitializeTabPageCurrentPageViewModelAsync(object parameter)
        {
            var mainPage = CurrentApplication.MainPage as iOSMainView;
            return ((mainPage.CurrentPage as CustomNavigationPage).CurrentPage.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        private Task InitializeMainPageViewModelAsync(object parameter)
        {
            var mainPage = CurrentApplication.MainPage as iOSMainView;
            return (mainPage.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        private void InitalizeMainPage(iOSMainView mainPage)
        {
            SetMainPage(mainPage);

            var schedulePage = CreateAndBindPage(typeof(ScheduleViewModel), null);
            SetNavigationIcon(schedulePage);
            mainPage.AddPage(schedulePage, "Schedule");

            var findPage = CreateAndBindPage(typeof(FindRideViewModel), null);
            SetNavigationIcon(findPage);
            mainPage.AddPage(findPage, "Find a ride");

            var myProfilePage = CreateAndBindPage(typeof(ProfileViewModel), null);
            SetNavigationIcon(myProfilePage);
            mainPage.AddPage(myProfilePage, "Settings");

            var drivePage = CreateAndBindPage(typeof(DriveViewModel), null);
            SetNavigationIcon(drivePage);
            if (App.CurrentUser.RiderRequests > 0)
            {
                mainPage.AddPage(drivePage, "Drive", Color.Red, App.CurrentUser.RiderRequests.ToString());
            }
            else
            {
                mainPage.AddPage(drivePage, "Drive");
            }

            mainPage.CurrentPageChanged += MainViewPageChanged;
        }

        private void CreatePageViewModelMappings()
        {
            if (_mappings.ContainsKey(typeof(MainViewModel)))
            {
                _mappings[typeof(MainViewModel)] = typeof(iOSMainView);
            }
            else
            {
                _mappings.Add(typeof(MainViewModel), typeof(iOSMainView));
            }
        }

        private void MainViewPageChanged(object sender, EventArgs e)
        {
            var mainPage = sender as iOSMainView;

            if (mainPage == null) return;

            if (mainPage.CurrentPage == null)
            {
                return;
            }

            if (!mainPage.CurrentPage.IsEnabled)
            {
                mainPage.CurrentPage = mainPage.PreviousPage;
            }
            else
            {
                mainPage.PreviousPage = mainPage.CurrentPage;
            }

            InitializeTabPageCurrentPageViewModelAsync(null);
        }

        private async void OnMainViewCurrentChanged(iOSMainView mainView)
        {
            object parameter = null;

            CustomNavigationPage navigation = mainView.CurrentPage as CustomNavigationPage;

            if (navigation.CurrentPage != null)
            {
                parameter = _requestedNavigationParameter;
                _requestedNavigationParameter = null;
                _requestedPageType = null;
            }

            if (parameter != null)
            {
                await InitializeMainPageViewModelAsync(parameter);
            }
        }
    }
}