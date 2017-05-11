using Android.Gms.Maps.Model;

namespace CarPool.Clients.Droid.Maps.Pushpins
{
    public abstract class BaseIcon
    {
        public MarkerOptions MarkerOptions { get; }

        protected BaseIcon()
        {
            MarkerOptions = new MarkerOptions();
            MarkerOptions.Draggable(false);
            MarkerOptions.Anchor(0.5f, 0.5f);
        }
    }
}