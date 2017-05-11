using CarPool.Clients.Core.Maps.Model;
using CoreLocation;

namespace CarPool.Clients.iOS.Maps.Annotations
{
    public class CustomPinAnnotation : BaseAnnotation
    {
        public CustomPin CustomPin { get; private set; }

        public CustomPinAnnotation(CLLocationCoordinate2D coordinate, CustomPin pin)
            : base(coordinate)
        {
            CustomPin = pin;
        }
    }
}