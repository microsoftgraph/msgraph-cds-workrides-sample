using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Views
{
    public partial class ScheduleDriverItemTemplate : ContentView
    {
        public static readonly BindableProperty SendEmailCommandProperty =
            BindableProperty.Create(nameof(SendEmailCommand), typeof(ICommand), 
                typeof(ScheduleDriverItemTemplate), default(ICommand));

        public ICommand SendEmailCommand
        {
            get { return (ICommand)GetValue(SendEmailCommandProperty); }
            set { SetValue(SendEmailCommandProperty, value); }
        }

        public static readonly BindableProperty PhoneCallCommandProperty =
            BindableProperty.Create(nameof(PhoneCallCommand), typeof(ICommand), 
                typeof(ScheduleDriverItemTemplate), default(ICommand));

        public ICommand PhoneCallCommand
        {
            get { return (ICommand)GetValue(PhoneCallCommandProperty); }
            set { SetValue(PhoneCallCommandProperty, value); }
        }
        
        public ScheduleDriverItemTemplate()
        {
            InitializeComponent();
        }
    }
}
