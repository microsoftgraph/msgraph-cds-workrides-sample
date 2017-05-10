using CoreLocation;
using MapKit;
using System;
using System.Linq;
using System.Collections.Generic;
using CarPool.Clients.Core.Maps.Pushpins;
using CarPool.Clients.Core.Maps.Controls;
using CarPool.Clients.Core.Maps.Model;
using System.Diagnostics;
using CarPool.Clients.iOS.Maps.Annotations;

namespace CarPool.Clients.iOS.Maps.Pushpins
{
    public class AnnotationManager : AbstractPushpinManager
    {
        private readonly MKMapView _nativeMap;

        public AnnotationManager(MKMapView nativeMap, CustomMap formsMap)
            : base(formsMap)
        { 
            _nativeMap = nativeMap;

            _nativeMap.DidSelectAnnotationView -= DidSelectAnnotationView;
            _nativeMap.DidSelectAnnotationView += DidSelectAnnotationView;
            _nativeMap.DidDeselectAnnotationView -= DidDeselectAnnotationView;
            _nativeMap.DidDeselectAnnotationView += DidDeselectAnnotationView;
        }


        public override void RemoveAllPushins()
        {
            var allAnnotations = _nativeMap.Annotations?.OfType<CustomPinAnnotation>()
                                                              .ToArray();

            if (allAnnotations?.Any() == true)
            {
                _nativeMap.RemoveAnnotations(allAnnotations);
            }
        }

        public MKAnnotationView GetViewForAnnotation(MKMapView mapView, IMKAnnotation annotation)
        {
            if (annotation is CustomPinAnnotation)
            {
                return GetViewForCustomPinAnnotation(annotation as CustomPinAnnotation);
            }
            else if (annotation is RiderAnnotation)
            {
                return GetViewForRiderAnnotation(annotation as RiderAnnotation);
            }
            else
            {
                return null;
            }
        }

        public override void ShowPushpinInformationPanel(CustomPin pin)
        {
            // search for the annotation in the map
            var annotation = _nativeMap?.Annotations?.OfType<CustomPinAnnotation>().Where(i => i.CustomPin.Id == pin.Id).FirstOrDefault();
            _nativeMap.SelectAnnotation(annotation, false);
        }

        public override void HidePushpinInformationPanel()
        {
            // close all selected annotations
            var annotations = _nativeMap?.SelectedAnnotations;
            if (annotations != null)
            {
                foreach (var annotation in annotations)
                {
                    _nativeMap.DeselectAnnotation(annotation, false);
                }
            }
        }

        public override void SetInteraction(bool active)
        {
            foreach (var annotation in _nativeMap.Annotations)
            {
                var view = _nativeMap.ViewForAnnotation(annotation);

                if (view != null)
                {
                    view.Enabled = active;
                }
            }
        }

        public override void RemovePushpins(IEnumerable<CustomPin> removedPushpins)
        {
            CustomPinAnnotation[] annotations =
                _nativeMap?.Annotations?.OfType<CustomPinAnnotation>()
                                        .Where(a => removedPushpins.Any(i => i.Id == a.CustomPin.Id))
                                        .ToArray();

            if (annotations != null)
            {
                _nativeMap.RemoveAnnotations(annotations);
            }
        }

        protected override void AddPushpinToMap(CustomPin pin)
        {
            try
            {
                var annotation = new CustomPinAnnotation(new  
                    CLLocationCoordinate2D
                {
                    Latitude = pin.Latitude,
                    Longitude = pin.Longitude
                }, 
                pin);

                _nativeMap.AddAnnotation(annotation);

                if (annotation.CustomPin.Duration.HasValue)
                {
                    ShowPushpinInformationPanel(annotation.CustomPin);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private MKAnnotationView GetViewForCustomPinAnnotation(CustomPinAnnotation annotation)
        {
            var annotationView = _nativeMap.DequeueReusableAnnotation(CustomPinAnnotationView.CustomReuseIdentifier) as CustomPinAnnotationView;

            if (annotationView == null)
            {
                annotationView = new CustomPinAnnotationView(annotation, annotation.CustomPin);
                annotationView.OnClose += OnIncidentInfoWindowClose;
                annotationView.OnNavigationRequested += OnIncidentInfoNavigationRequest;
            }
            else
            {
                annotationView.Pushpin = annotation.CustomPin;
            }

            return annotationView;
        }

        private MKAnnotationView GetViewForRiderAnnotation(RiderAnnotation annotation)
        {
            var annotationView = _nativeMap.DequeueReusableAnnotation(RiderAnnotationView.CustomReuseIdentifier) as RiderAnnotationView;

            if (annotationView == null)
            {
                annotationView = new RiderAnnotationView(annotation, annotation.Rider);
            }
            else
            {
                annotationView.Rider = annotation.Rider;
            }

            return annotationView;
        }

        private void OnIncidentInfoNavigationRequest(object sender, EventArgs e)
        {
            var pushpinView = sender as CustomPinAnnotationView;

            if (pushpinView != null)
            {
                OnNavigationRequested(pushpinView.Pushpin);
            }
        }

        private void OnIncidentInfoWindowClose(object sender, EventArgs e)
        {
            var annotationView = sender as CustomPinAnnotationView;

            if (annotationView != null)
            {
                _nativeMap.DeselectAnnotation(annotationView.Annotation, true);
            }

            FormsMap.CurrentPushpin = null;
        }

        private void DidSelectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            var pushpinAnnotation = e.View as CustomPinAnnotationView;

            if (pushpinAnnotation == null)
            {
                // We only care about Incident annotations
                return;
            }

            OnPushpinSelected(pushpinAnnotation.Pushpin);
        }

        private void DidDeselectAnnotationView(object sender, MKAnnotationViewEventArgs e)
        {
            OnPushpinUnselected(FormsMap.CurrentPushpin);
        }

        public override void RemoveAllRiders()
        {
            RiderAnnotation[] annotations =
                   _nativeMap?.Annotations?.OfType<RiderAnnotation>()
                                           .ToArray();

            if (annotations != null)
            {
                _nativeMap.RemoveAnnotations(annotations);
            }
        }

        protected override void AddRiderToMap(CustomRider rider)
        {
            var annotation = new RiderAnnotation(new
                     CLLocationCoordinate2D
            {
                Latitude = rider.Latitude,
                Longitude = rider.Longitude
            },
                rider);

            _nativeMap.AddAnnotation(annotation);
        }
    }
}