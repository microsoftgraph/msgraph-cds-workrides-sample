// WARNING
//
// This file has been generated automatically by Xamarin Studio from the outlets and
// actions declared in your storyboard file.
// Manual changes to this file will not be maintained.
//
using Foundation;
using System;
using System.CodeDom.Compiler;
using UIKit;

namespace CarPool.Clients.iOS
{
    [Register ("MapIncidentInfoView")]
    partial class MapIncidentInfoView
    {
        [Outlet]
        UIKit.UILabel DescriptionLabel { get; set; }

        [Outlet]
        UIKit.UIImageView DialogContainer { get; set; }

        void ReleaseDesignerOutlets ()
        {
            if (DescriptionLabel != null) {
                DescriptionLabel.Dispose ();
                DescriptionLabel = null;
            }

            if (DialogContainer != null) {
                DialogContainer.Dispose ();
                DialogContainer = null;
            }
        }
    }
}