using Xamarin.Forms.Platform.Android;
using Xamarin.Forms;
using Android.Support.V4.View;
using CarPool.Clients.Core.Controls;
using CarPool.Clients.Droid.Renderers;

[assembly: ExportRendererAttribute(typeof(GridSplitter), typeof(GridSplitterRenderer))]
namespace CarPool.Clients.Droid.Renderers
{
    public class GridSplitterRenderer : VisualElementRenderer<GridSplitter>
    {
        private Point _lastPoint;
        
        public override bool OnTouchEvent(Android.Views.MotionEvent e)

        {
            int action = MotionEventCompat.GetActionMasked(e);

            switch (action)
            {
                case (int)Android.Views.MotionEventActions.Down:
                    {
                        _lastPoint = new Point(e.RawX, e.RawY);
                        break;
                    }

                case (int)Android.Views.MotionEventActions.Move:
                    {
                        Element.UpdateGrid(Context.FromPixels(e.RawX - _lastPoint.X), Context.FromPixels(e.RawY - _lastPoint.Y));
                        _lastPoint = new Point(e.RawX, e.RawY);
                        break;
                    }
            }

            return base.OnTouchEvent(e);
        }
    }
}