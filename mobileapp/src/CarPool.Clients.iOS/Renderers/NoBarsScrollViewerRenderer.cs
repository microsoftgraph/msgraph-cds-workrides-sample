using CarPool.Clients.Core.Controls;
using CarPool.Clients.iOS.Renderers;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(NoBarsScrollViewer), typeof(NoBarsScrollViewerRenderer))]
namespace CarPool.Clients.iOS.Renderers
{
    public class NoBarsScrollViewerRenderer : ScrollViewRenderer
	{
		protected override void OnElementChanged(VisualElementChangedEventArgs e)
		{
			base.OnElementChanged(e);

			if (e.OldElement != null || this.Element == null)
			{
				return;
			}

			ShowsHorizontalScrollIndicator = false;
			ShowsVerticalScrollIndicator = false;
		}
	}
}
