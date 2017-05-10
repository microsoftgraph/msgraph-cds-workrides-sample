using Xamarin.Forms;

namespace CarPool.Clients.Core.Views
{
    public partial class CustomNavigationPage : NavigationPage
    {
        public CustomNavigationPage() : base()
        {
            InitializeComponent();
        }

        public CustomNavigationPage(Page root) : base(root)
        {
            InitializeComponent();
        }

        public Color BadgeColor { get; set; }

        public string BadgeText { get; set; }
        
    }
}