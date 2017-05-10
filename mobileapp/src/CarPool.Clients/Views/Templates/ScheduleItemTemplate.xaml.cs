using System.Windows.Input;
using Xamarin.Forms;

namespace CarPool.Clients.Core.Views
{
    public partial class ScheduleItemTemplate : ContentView
    {
        public ScheduleItemTemplate()
        {
            InitializeComponent();
        }

        public static readonly BindableProperty TapCommandProperty =
            BindableProperty.Create(nameof(TapCommand), typeof(ICommand),
                typeof(ScheduleItemTemplate), default(ICommand));

        public ICommand TapCommand
        {
            get { return (ICommand)GetValue(TapCommandProperty); }
            set { SetValue(TapCommandProperty, value); }
        }
    }
}