using CarPool.Clients.Core.Services.Graph.Calendar;
using CarPool.Clients.Core.Services.Graph.Mail;
using CarPool.Clients.Core.Services.Graph.People;
using CarPool.Clients.Core.Services.Graph.User;
using CarPool.Clients.Core.ViewModels;
using Carpool.Clients.Services.Dialog;
using Carpool.Clients.Services.Navigation;
using Microsoft.Practices.Unity;
using System;
using Carpool.Clients.Services.Data;
using CarPool.Clients.Core.Services.Graph.Driver;
using CarPool.Clients.Core.Services.Data;
using CarPool.Clients.Core.Services.OpenMap;
using CarPool.Clients.Core.Helpers;
using CarPool.Clients.Core.Services.Graph.Organization;

namespace Carpool.Clients.ViewModels.Base
{
    public class ViewModelLocator
    {
        private readonly IUnityContainer _unityContainer;

        private static readonly ViewModelLocator _instance = new ViewModelLocator();

        public static ViewModelLocator Instance
        {
            get
            {
                return _instance;
            }
        }

        protected ViewModelLocator()
        {
            _unityContainer = new UnityContainer();

            // Services   
            _unityContainer.RegisterType<IRequestProvider, RequestProvider>();
            _unityContainer.RegisterType<ICalendarService, CalendarService>();
            _unityContainer.RegisterType<IDialogService, DialogService>();
            _unityContainer.RegisterType<IMailService, MailService>();
            _unityContainer.RegisterType<IPeopleService, PeopleService>();
            _unityContainer.RegisterType<IUserService, UserService>();
            _unityContainer.RegisterType<IOrganizationService, OrganizationService>();
            _unityContainer.RegisterType<IDriverService, DriverService>();
            _unityContainer.RegisterType<IOpenMapService, OpenMapService>();
            RegisterSingleton<INavigationService, NavigationService>();

            //RegisterSingleton<IDataService, SQLiteService>();
            //RegisterSingleton<IDataService, AzureMobileAppService>();
            //RegisterSingleton<IDataService, CDSDataProvider>();
            RegisterSingleton<IDataService, CDSODataProvider>();

            // ViewModels
            _unityContainer.RegisterType<DriveViewModel>();
            _unityContainer.RegisterType<FindRideViewModel>();
            _unityContainer.RegisterType<FindRideDetailViewModel>();
            _unityContainer.RegisterType<MainViewModel>();
            _unityContainer.RegisterType<NewMessageViewModel>();
            _unityContainer.RegisterType<ProfileViewModel>();
            _unityContainer.RegisterType<RidePreferencesViewModel>();
            _unityContainer.RegisterType<ScheduleViewModel>();
            _unityContainer.RegisterType<ScheduleDetailViewModel>();
            _unityContainer.RegisterType<UserPreferencesViewModel>();
            _unityContainer.RegisterType<DataSettingsViewModel>();
            _unityContainer.RegisterType<MenuViewModel>();
            RegisterSingleton<LoginViewModel>();

            // Helpers
            _unityContainer.RegisterType<UserHelper>();
        }

        public T Resolve<T>()
        {
            return _unityContainer.Resolve<T>();
        }

        public object Resolve(Type type)
        {
            return _unityContainer.Resolve(type);
        }

        public void Register<T>(T instance)
        {
            _unityContainer.RegisterInstance<T>(instance);
        }

        public void Register<TInterface, T>() where T : TInterface
        {
            _unityContainer.RegisterType<TInterface, T>();
        }

        public void RegisterSingleton<TInterface, T>() where T : TInterface
        {
            _unityContainer.RegisterType<TInterface, T>(new ContainerControlledLifetimeManager());
        }

        public void RegisterSingleton<T>()
        {
            _unityContainer.RegisterType<T>(new ContainerControlledLifetimeManager());
        }
    }
}