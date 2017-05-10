using System.ComponentModel;
using MapKit;
using UIKit;
using Xamarin.Forms.Platform.iOS;
using CarPool.Clients.Core.Maps.Model;

namespace CarPool.Clients.iOS.Maps.Annotations
{
    public class RiderAnnotationView : MKAnnotationView
    {
        public const string CustomReuseIdentifier = nameof(RiderAnnotationView);

        private ResponderIconView _iconView;

        private CustomRider _rider;

        public CustomRider Rider
        {
            get
            {
                return _rider;
            }

            set
            {
                _rider = value;
                UpdateData();
            }
        }

        public RiderAnnotationView(IMKAnnotation annotation, CustomRider rider)
            : base(annotation, CustomReuseIdentifier)
        {
            Rider = rider;
        }

        private void UpdateColor(object sender, PropertyChangedEventArgs e)
        {
            if (_iconView != null)
            {
                _iconView.BackgroundColor = (sender as CustomRider).Color.ToUIColor();
                _iconView.LoadResponderData(Rider);
            }
        }

        public override void MovedToSuperview()
        {
            base.MovedToSuperview();

            _iconView = ResponderIconView.Create();

			var width = _iconView.Frame.Size.Width;
            _iconView.Frame = new CoreGraphics.CGRect(0, 0, width, width);
            _iconView.Center = new CoreGraphics.CGPoint
            {
                X = Frame.Size.Width / 2,
                Y = Frame.Size.Height / 2
            };

            _iconView.Layer.CornerRadius = width / 2;
            _iconView.Layer.BorderWidth = 2.0f;
            _iconView.Layer.BorderColor = UIColor.White.CGColor;

			UpdateData();
            AddSubview(_iconView);
        }

        private void UpdateData()
        {
            if (_iconView != null)
            {
                _iconView.BackgroundColor = Rider.Color.ToUIColor();
                _iconView.LoadResponderData(Rider);
            }
        }
    }
}