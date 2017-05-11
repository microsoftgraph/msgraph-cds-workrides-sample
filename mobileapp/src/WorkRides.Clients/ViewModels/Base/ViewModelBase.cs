using Carpool.Clients.Services.Dialog;
using Carpool.Clients.Services.Navigation;
using Carpool.Clients.ViewModels.Base;
using CarPool.Clients.Core.Models;
using Plugin.Connectivity;
using System.Threading.Tasks;

namespace CarPool.Clients.Core.ViewModels.Base
{
    public class ViewModelBase : ExtendedBindableObject
    {
        protected readonly IDialogService DialogService;
        protected readonly INavigationService NavigationService;

        private bool _isBusy;

        public ViewModelBase()
        {
            DialogService = ViewModelLocator.Instance.Resolve<IDialogService>();
            NavigationService = ViewModelLocator.Instance.Resolve<INavigationService>();
        }

        public bool IsBusy
        {
            get
            {
                return _isBusy;
            }

            set
            {
                _isBusy = value;
                RaisePropertyChanged(() => IsBusy);
            }
        }

        public GraphUser CurrentUser
        {
            get
            {
                return App.CurrentUser;
            }
        }

        public bool IsConnected => CrossConnectivity.Current.IsConnected;

        public virtual Task InitializeAsync(object navigationData)
        {
            return Task.FromResult(false);
        }
    }
}