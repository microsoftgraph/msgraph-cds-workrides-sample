using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.Views;
using Android.Widget;
using CarPool.Clients.Core.Models;

namespace CarPool.Clients.Droid.Maps.Adapters
{
    public class MapInfoWindowAdapter : Java.Lang.Object, GoogleMap.IInfoWindowAdapter
    {
        private LayoutInflater _inflater;

        public MapInfoWindowAdapter()
        {
            _inflater = LayoutInflater.From(Xamarin.Forms.Forms.Context);
        }

        public GraphUser CurrentUser { get; set; }

        public double? Duration { get; set; }

        public View GetInfoWindow(Marker marker)
        {
            if (_inflater != null && Duration.HasValue)
            {
                var iconView = _inflater.Inflate(Resource.Layout.duration_info_content, null);

                var titleDuration = iconView.FindViewById<TextView>(Resource.Id.duration_description_label);
                if (titleDuration != null)
                {
                    titleDuration.Text = string.Format("{0:0.#} min added", Duration.Value);
                }
                
                return iconView;
            }

            return null;
        }

        public View GetInfoContents(Marker marker)
        {
            return null;
        }
    }
}