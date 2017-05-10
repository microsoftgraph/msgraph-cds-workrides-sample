using Android.Gms.Maps.Model;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Xamarin.Forms.Platform.Android;
using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.Droid.Extensions;

namespace CarPool.Clients.Droid.Maps.Icons
{
    public class RiderIcon : BaseIcon
    {
        private LayoutInflater _inflater;
        private View _riderIconView;

        public RiderIcon(CustomRider rider)
            : base()
        {
            Rider = rider;

            _inflater = LayoutInflater.From(Xamarin.Forms.Forms.Context);
            _riderIconView = _inflater.Inflate(Resource.Layout.rider_icon_content, null);

            var responderType = _riderIconView.FindViewById<TextView>(Resource.Id.responder_type);
            responderType.Text = Rider.Acronym;
            GradientDrawable drawable = (GradientDrawable)responderType.Background;
            drawable.SetColor(Rider.Color.ToAndroid());

            MarkerOptions.SetTitle(Rider.Acronym);
            MarkerOptions.SetSnippet(Rider.Address);

            Bitmap icon = _riderIconView.AsBitmap(Xamarin.Forms.Forms.Context, 60, 60);
            MarkerOptions.SetIcon(BitmapDescriptorFactory.FromBitmap(icon));
        }

        public CustomRider Rider { get; }
    }
}