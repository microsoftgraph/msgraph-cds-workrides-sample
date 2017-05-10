using CarPool.Clients.Core.Models;
using CarPool.Clients.Core.Services.Graph;
using CarPool.Clients.Core.ViewModels.Base;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Forms;
using CarPool.Clients.Core.Services.Graph.User;
using System.IO;
using System.Linq;

namespace CarPool.Clients.Core.ViewModels
{
    public class MenuViewModel : ViewModelBase
    {
        private readonly IUserService _userService;
        private ObservableCollection<Models.MenuItem> _menuItems;

        public MenuViewModel(
            IUserService userService)
        {
            _userService = userService;
            _menuItems = new ObservableCollection<Models.MenuItem>();
            InitMenuItems();

            MessagingCenter.Subscribe<GraphUser>(this, MessengerKeys.UserReviewRiderRequest,
                UpdateRiderRequestsNotification);
        }

        public ObservableCollection<Models.MenuItem> MenuItems
        {
            get
            {
                return _menuItems;
            }
            set
            {
                _menuItems = value;
                RaisePropertyChanged(() => MenuItems);
            }
        }

        public ICommand ItemSelectedCommand => new Command<Models.MenuItem>(async (m) => await OnSelectItemAsync(m));

        public ICommand LogoutCommand => new Command(async () => await LogoutAsync());

        public override async Task InitializeAsync(object navigationData)
        {
            if (CurrentUser.PhotoStream == null)
            {
                var photo = await _userService.GetUserPhotoAsync(CurrentUser.Mail);
                if (photo != null)
                {
                    CurrentUser.PhotoStream = ImageSource.FromStream(() => new MemoryStream(photo));
                }
            }
        }

        private void InitMenuItems()
        {
            MenuItems.Add(new Models.MenuItem
            {
                Title = "Schedule",
                MenuItemType = MenuItemType.Schedule,
                ViewModelType = typeof(ScheduleViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new Models.MenuItem
            {
                Title = "Find a ride",
                MenuItemType = MenuItemType.FindRide,
                ViewModelType = typeof(FindRideViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new Models.MenuItem
            {
                Title = "Settings",
                MenuItemType = MenuItemType.MyProfile,
                ViewModelType = typeof(ProfileViewModel),
                IsEnabled = true
            });

            MenuItems.Add(new Models.MenuItem
            {
                Title = "Drive",
                HasNotification = App.CurrentUser.RiderRequests > 0,
                MenuItemType = MenuItemType.Drive,
                ViewModelType = typeof(DriveViewModel),
                IsEnabled = true
            });
        }

        private async Task OnSelectItemAsync(Models.MenuItem item)
        {
            if (item.IsEnabled)
            {
                await NavigationService.NavigateToAsync(item.ViewModelType, item);
            }
        }

        private async Task LogoutAsync()
        {
            GraphClient.Instance.SignOut();

            await NavigationService.NavigateToAsync<LoginViewModel>();
        }

        private void UpdateRiderRequestsNotification(GraphUser user)
        {
            var item = MenuItems.FirstOrDefault(m => m.MenuItemType == MenuItemType.Drive);
            item.HasNotification = user.RiderRequests > 0;
        }
    }
}