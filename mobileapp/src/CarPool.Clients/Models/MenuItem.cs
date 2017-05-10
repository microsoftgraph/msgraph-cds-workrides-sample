using CarPool.Clients.Core.ViewModels.Base;
using System;

namespace CarPool.Clients.Core.Models
{
    public class MenuItem : ExtendedBindableObject
    {
        private string _title;

        public string Title
        {
            get
            {
                return _title;
            }

            set
            {
                _title = value;
                RaisePropertyChanged(() => Title);
            }
        }

        private MenuItemType _menuItemType;

        public MenuItemType MenuItemType
        {
            get
            {
                return _menuItemType;
            }

            set
            {
                _menuItemType = value;
                RaisePropertyChanged(() => MenuItemType);
            }
        }

        private Type _viewModelType;

        public Type ViewModelType
        {
            get
            {
                return _viewModelType;
            }

            set
            {
                _viewModelType = value;
                RaisePropertyChanged(() => ViewModelType);
            }
        }

        private bool _isEnabled;

        public bool IsEnabled
        {
            get
            {
                return _isEnabled;
            }

            set
            {
                _isEnabled = value;
                RaisePropertyChanged(() => IsEnabled);
            }
        }

        private bool _hasNotification;

        public bool HasNotification
        {
            get
            {
                return _hasNotification;
            }

            set
            { 
                if (value != _hasNotification) {
                    _hasNotification = value;
                    RaisePropertyChanged(() => HasNotification);
                }
            }
        }
    }
}