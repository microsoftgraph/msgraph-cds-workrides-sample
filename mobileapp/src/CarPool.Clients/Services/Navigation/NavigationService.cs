using CarPool.Clients.Core.ViewModels;
using CarPool.Clients.Core.ViewModels.Base;
using CarPool.Clients.Core.Views;
using Carpool.Clients.ViewModels.Base;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Carpool.Clients.Services.Navigation
{
    public partial class NavigationService : INavigationService
    {
        protected readonly Dictionary<Type, Type> _mappings;

        protected Application CurrentApplication
        {
            get
            {
                return Application.Current;
            }
        }

        public NavigationService()
        {
            _mappings = new Dictionary<Type, Type>();

            CreatePageViewModelMappings();
        }

        public Task InitializeAsync()
        {
            return NavigateToAsync<LoginViewModel>();
        }

        public Task NavigateToAsync<TViewModel>() where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), null);
        }

        public Task NavigateToAsync<TViewModel>(object parameter) where TViewModel : ViewModelBase
        {
            return InternalNavigateToAsync(typeof(TViewModel), parameter);
        }

        public Task NavigateToAsync(Type viewModelType)
        {
            return InternalNavigateToAsync(viewModelType, null);
        }

        public Task NavigateToAsync(Type viewModelType, object parameter)
        {
            return InternalNavigateToAsync(viewModelType, parameter);
        }

        public virtual async Task NavigateBackAsync()
        {
            if (CurrentApplication.MainPage is MainView)
            {
                var mainPage = CurrentApplication.MainPage as MainView;
                await mainPage.Detail.Navigation.PopAsync();
            }
            else if (CurrentApplication.MainPage != null)
            {
                await CurrentApplication.MainPage.Navigation.PopAsync();
            }
        }

        public virtual Task RemoveLastFromBackStackAsync()
        {
            var mainPage = CurrentApplication.MainPage as MainView;

            if (mainPage != null)
            {
                mainPage.Detail.Navigation.RemovePage(
                    mainPage.Detail.Navigation.NavigationStack[mainPage.Detail.Navigation.NavigationStack.Count - 2]);
            }

            return Task.FromResult(true);
        }

        protected virtual async Task InternalNavigateToAsync(Type viewModelType, object parameter)
        {
            Page page = CreateAndBindPage(viewModelType, parameter);
            SetNavigationIcon(page);

            if (page is MainView)
            {
                SetMainPage(page);
            }
            else if (page is LoginView)
            {
                SetMainPage(page);
            }
            else if (CurrentApplication.MainPage is MainView)
            {
                var mainPage = CurrentApplication.MainPage as MainView;
                var navigationPage = mainPage.Detail as CustomNavigationPage;

                if (navigationPage != null)
                {
                    await navigationPage.PushAsync(page);
                }
                else
                {
                    navigationPage = new CustomNavigationPage(page);
                    mainPage.Detail = navigationPage;
                }

                mainPage.IsPresented = false;
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
            }

            await (page.BindingContext as ViewModelBase).InitializeAsync(parameter);
        }

        protected Type GetPageTypeForViewModel(Type viewModelType)
        {
            if (!_mappings.ContainsKey(viewModelType))
            {
                throw new KeyNotFoundException($"No map for ${viewModelType} was found on navigation mappings");
            }

            return _mappings[viewModelType];
        }

        protected Page CreateAndBindPage(Type viewModelType, object parameter)
        {
            Type pageType = GetPageTypeForViewModel(viewModelType);

            if (pageType == null)
            {
                throw new Exception($"Mapping type for {viewModelType} is not a page");
            }

            var newPage = Activator.CreateInstance(pageType);
            Page page = newPage as Page;
            ViewModelBase viewModel = ViewModelLocator.Instance.Resolve(viewModelType) as ViewModelBase;
            page.BindingContext = viewModel;

            return page;
        }

        protected virtual void SetNavigationIcon(Page page)
        {
            NavigationPage.SetBackButtonTitle(page, "Back");
        }

        protected virtual void SetMainPage(Page page)
        {
            CurrentApplication.MainPage = page;
        }

        private void CreatePageViewModelMappings()
        {
            _mappings.Add(typeof(DriveViewModel), typeof(DriveView));
            _mappings.Add(typeof(FindRideViewModel), typeof(FindRideView));
            _mappings.Add(typeof(FindRideDetailViewModel), typeof(FindRideDetailView));
            _mappings.Add(typeof(InspectRiderRequestViewModel), typeof(InspectRiderRequestView));
            _mappings.Add(typeof(LoginViewModel), typeof(LoginView));
            _mappings.Add(typeof(MainViewModel), typeof(MainView));
            _mappings.Add(typeof(NewMessageViewModel), typeof(NewMessageView));
            _mappings.Add(typeof(ProfileViewModel), typeof(ProfileView));
            _mappings.Add(typeof(RidePreferencesViewModel), typeof(RidePreferencesView));
            _mappings.Add(typeof(DriverPreferencesViewModel), typeof(DriverPreferencesView));
            _mappings.Add(typeof(ScheduleViewModel), typeof(ScheduleView));
            _mappings.Add(typeof(ScheduleDetailViewModel), typeof(ScheduleDetailView));
            _mappings.Add(typeof(UserPreferencesViewModel), typeof(UserPreferencesView));
            _mappings.Add(typeof(DataSettingsViewModel), typeof(DataSettingsView));
        }
    }
}