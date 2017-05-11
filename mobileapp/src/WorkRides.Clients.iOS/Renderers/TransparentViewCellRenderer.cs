using UIKit;
using Xamarin.Forms.Platform.iOS;
using CarPool.Clients.iOS.Renderers;
using Xamarin.Forms;

[assembly: ExportRenderer(typeof(ViewCell), typeof(TransparentViewCellRenderer))]
namespace CarPool.Clients.iOS.Renderers
{
    public class TransparentViewCellRenderer : ViewCellRenderer
    {
        public TransparentViewCellRenderer()
        {

        }

        public override UITableViewCell GetCell(Cell item, UITableViewCell reusableCell, UITableView tv)
        {
            var cell = base.GetCell(item, reusableCell, tv);
            if (cell != null) cell.BackgroundColor = UIColor.Clear;
            return cell;
        }
    }
}