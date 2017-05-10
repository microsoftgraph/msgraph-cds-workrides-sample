using System;
using CoreGraphics;
using MapKit;
using UIKit;
using CarPool.Clients.Core.Maps.Model;

namespace CarPool.Clients.iOS.Maps.Annotations
{
    public class CustomPinAnnotationView : MKAnnotationView
    {
        public const string CustomReuseIdentifier = nameof(CustomPinAnnotationView);

        public event EventHandler OnNavigationRequested;
        public event EventHandler OnClose;

        private MapIncidentInfoView _infoView;
        private CustomPin _pin;

        public CustomPin Pushpin
        {
            get
            {
                return _pin;
            }

            set
            {
                _pin = value;
                UpdateImage(_pin);
            }
        }

        public CustomPinAnnotationView(IMKAnnotation annotation, CustomPin pin)
            : base(annotation, CustomReuseIdentifier)
        {
            Pushpin = pin;
        }

        public override void SetSelected(bool selected, bool animated)
        {
            base.SetSelected(selected, animated);

            if (selected)
            {
                _infoView = MapIncidentInfoView.Create();
                _infoView.UserInteractionEnabled = true;

				var desiredSize = _infoView.Frame.Size;
                var positionXOffset = -desiredSize.Width / 2 + Frame.Width / 2;

                var desiredPosition = new CGPoint(positionXOffset, -desiredSize.Height);
                _infoView.Frame = new CGRect(desiredPosition, desiredSize);
                _infoView.OnClose -= OnInfoViewClose;
                _infoView.OnClose += OnInfoViewClose;
                _infoView.OnNavigationRequested -= OnInfoViewNavigationRequest;
                _infoView.OnNavigationRequested += OnInfoViewNavigationRequest;

                _infoView.LoadIncidentData(Pushpin);

                AddSubview(_infoView);
            }
            else
            {
                _infoView.OnClose -= OnInfoViewClose;
                _infoView.OnNavigationRequested -= OnInfoViewNavigationRequest;
                _infoView.RemoveFromSuperview();
            }
        }

        public override bool PointInside(CGPoint point, UIEvent uievent)
        {
            return base.PointInside(point, uievent) || _infoView?.Frame.Contains(point) == true;
        }

        public override UIView HitTest(CGPoint point, UIEvent uievent)
        {
            var view = base.HitTest(point, uievent);

            if (view == null)
            {
                if (_infoView?.Frame.Contains(point) == true)
                {
                    view = _infoView;
                }
            }

            return view;
        }

        private void OnInfoViewNavigationRequest(object sender, EventArgs e)
        {
            var handler = OnNavigationRequested;

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void OnInfoViewClose(object sender, EventArgs e)
        {
            var handler = OnClose;

            handler?.Invoke(this, EventArgs.Empty);
        }

        private void UpdateImage(CustomPin pushpin)
        {
            Image = UIImage.FromFile(pushpin.Icon);
        }
    }
}