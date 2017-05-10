using CarPool.Clients.Core.Maps.Model;
using CarPool.Clients.iOS.Maps.Annotations;
using CoreLocation;

namespace CarPool.Clients.iOS.Maps.Annotations
{
    public class RiderAnnotation : BaseAnnotation
    {
        public CustomRider Rider { get; private set; }

        public RiderAnnotation(CLLocationCoordinate2D coordinate, CustomRider rider)
            : base(coordinate)
        {
            Rider = rider;
        }
    }
}