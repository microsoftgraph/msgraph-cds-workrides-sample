using UIKit;

namespace CarPool.Clients.iOS
{
    public class LinkerPleaseInclude
    {
        public void Include(UITabBarItem item)
        {
            item.BadgeColor = UIColor.Red;
            item.BadgeValue = "badge";
        }
    }
}