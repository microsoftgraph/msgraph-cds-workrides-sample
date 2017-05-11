using Android.Gms.Maps.Model;
using CarPool.Clients.Core.Maps.Model;

namespace CarPool.Clients.Droid.Maps.Pushpins
{
    public class PushpinIcon : BaseIcon
    {
        public PushpinIcon(CustomPin pushpin) : base()
        {
            Pushpin = pushpin;

            Initialize();
        }

        public CustomPin Pushpin { get; }

        private void Initialize()
        {
            if (!string.IsNullOrEmpty(Pushpin.Icon))
            {
                MarkerOptions.SetIcon(BitmapDescriptorFactory.FromAsset(Pushpin.Icon));
            }
            else
            {
                MarkerOptions.SetIcon(BitmapDescriptorFactory.DefaultMarker());
            }
        }
    }
}