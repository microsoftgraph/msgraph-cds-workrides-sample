using CarPool.Clients.Core.Controls;
using CarPool.Clients.iOS.Controls.RoundedBoxView;
using System;
using System.ComponentModel;
using System.Diagnostics;
using Xamarin.Forms;
using Xamarin.Forms.Platform.iOS;

[assembly: ExportRenderer(typeof(ContentViewRoundedCorners), typeof(ContentViewRoundedCornersRenderer))]
namespace CarPool.Clients.iOS.Controls.RoundedBoxView
{
    public class ContentViewRoundedCornersRenderer : VisualElementRenderer<ContentView>
    {
        protected override void OnElementChanged(ElementChangedEventArgs<ContentView> e)
        {
            base.OnElementChanged(e);

            if (Element != null)
            {
                Layer.MasksToBounds = true;
                UpdateCornerRadius(Element as ContentViewRoundedCorners);
            }
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == ContentViewRoundedCorners.CornerRadiusProperty.PropertyName)
            {
                UpdateCornerRadius(Element as ContentViewRoundedCorners);
            }
        }

        private void UpdateCornerRadius(ContentViewRoundedCorners content)
        {
            try
            {
                if (content == null)
                {
                    return;
                }

                Layer.CornerRadius = content.CornerRadius;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}