using System;
using System.ComponentModel;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Views
{
    public partial class UserPreferencesView : ContentPage
    {
        public UserPreferencesView()
        {
            NavigationPage.SetHasNavigationBar(this, false);
            InitializeComponent();
        }
    }
}