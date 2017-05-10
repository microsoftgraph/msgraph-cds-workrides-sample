
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace CarPool.Clients.Core.Maps.Model
{
    public class CustomPin
    {
        public string Id { get; set; }

        public double Longitude { get; set; }

        public double Latitude { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Address { get; set; }

        public double? Duration { get; set; }

        public string Icon { get; set; }

        public Position Position => new Position (Latitude, Longitude);
    }
}
