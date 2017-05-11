using Android.Content;
using Android.Graphics;
using Android.Util;
using CarPool.Clients.Core.Controls;
using CarPool.Clients.Droid.Controls;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;

[assembly: ExportRenderer(typeof(ContentViewRoundedCorners), typeof(ContentViewRoundedCornersRenderer))]
namespace CarPool.Clients.Droid.Controls
{
    public class ContentViewRoundedCornersRenderer : VisualElementRenderer<ContentView>
    {
        private float _cornerRadius;
        private RectF _bounds;
        private Path _path;

        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (e.OldElement != null)
            {
                return;
            }

            var element = (ContentViewRoundedCorners)Element;

            _cornerRadius = TypedValue.ApplyDimension(ComplexUnitType.Dip, element.CornerRadius,
                Context.Resources.DisplayMetrics);
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            if (w != oldw && h != oldh)
            {
                _bounds = new RectF(0, 0, w, h);
            }

            _path = new Path();
            _path.Reset();
            _path.AddRoundRect(_bounds, _cornerRadius, _cornerRadius, Path.Direction.Cw);
            _path.Close();
        }

        public override void Draw(Canvas canvas)
        {
            canvas.Save();
            canvas.ClipPath(_path);
            base.Draw(canvas);
            canvas.Restore();
        }
    }
}