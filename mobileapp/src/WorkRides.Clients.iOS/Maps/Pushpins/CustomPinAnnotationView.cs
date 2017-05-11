using System;
using MapKit;
using UIKit;
using CarPool.Clients.Core.Maps.Model;
using CoreGraphics;

namespace CarPool.Clients.iOS.Maps.Pushpins
{
    public class CustomPinAnnotationView : MKAnnotationView
    {
        public const string CustomReuseIdentifier = nameof(CustomPinAnnotationView);

        public event EventHandler OnNavigationRequested;
        public event EventHandler OnClose;

        private MapIncidentInfoView _infoView;
        private CustomPin _pushpin;

        public CustomPin Pushpin
        {
            get
            {
                return _pushpin;
            }

            set
            {
                _pushpin = value;
                UpdateImage(_pushpin);
            }
        }

        public CustomPinAnnotationView(IMKAnnotation annotation, CustomPin pushpin)
            : base(annotation, CustomReuseIdentifier)
        {
            Pushpin = pushpin;
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