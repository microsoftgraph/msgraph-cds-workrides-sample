using System;
using Android.Widget;
using Xamarin.Forms;
using System.ComponentModel;
using CarPool.Clients.Core.Effects;
using System.Diagnostics;
using CarPool.Clients.Droid.Effects;
using Xamarin.Forms.Platform.Android;

[assembly: ExportEffect(typeof(EntryLineColorEffect), "EntryLineColorEffect")]
namespace CarPool.Clients.Droid.Effects
{
    public class EntryLineColorEffect : PlatformEffect
    {
        EditText control;

        protected override void OnAttached()
        {
            try
            {
                control = Control as EditText;
                UpdateLineColor();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Cannot set property on attached control. Error: ", ex.Message);
            }
        }

        protected override void OnDetached()
        {
            control = null;
        }

        protected override void OnElementPropertyChanged(PropertyChangedEventArgs args)
        {
            if (args.PropertyName == LineColorEffect.LineColorProperty.PropertyName)
            {
                UpdateLineColor();
            }
        }

        private void UpdateLineColor()
        {
            try
            {
                if (control != null)
                {
                    var color = LineColorEffect.GetLineColor(Element).ToAndroid();
                    control.Background.SetColorFilter(color, Android.Graphics.PorterDuff.Mode.SrcAtop);
                    control.SetBackgroundColor(color);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}