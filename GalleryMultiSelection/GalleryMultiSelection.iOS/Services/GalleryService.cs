using ELCImagePicker;
using GalleryMultiSelection.iOS.Services;
using GalleryMultiSelection.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;
using Xamarin.Forms;

[assembly: Dependency(typeof(GalleryService))]
namespace GalleryMultiSelection.iOS.Services
{
    public class GalleryService : IGalleryService, IGestureListner
    {
        public void OpenGallery()
        {
            var picker = ELCImagePickerViewController.Instance;
            picker.MaximumImagesCount = 15;

            List<string> paths;

            picker.Completion.ContinueWith(t => {
                picker.BeginInvokeOnMainThread(() =>
                {
                    //dismiss the picker
                    picker.DismissViewController(true, null);

                    if (t.IsCanceled || t.Exception != null)
                    {
                    }
                    else
                    {
                        paths = new List<string>();
                        var st = new List<System.IO.Stream>();
                        var items = t.Result as List<AssetResult>;
                        foreach (var item in items)
                        {
                            st.Add(item.Image.AsPNG().AsStream());
                            paths.Add(item.Path);
                        }

                        MessagingCenter.Send<IGestureListner, List<string>>(this, "PostProject",paths);
                        MessagingCenter.Send<IGestureListner, List<System.IO.Stream>>(this, "PostProject", st);
                    }


                });
            });
            var topController = UIApplication.SharedApplication.KeyWindow.RootViewController;
            while (topController.PresentedViewController != null)
            {
                topController = topController.PresentedViewController;
            }


            topController.PresentViewController(picker, true, null);
        }
    }
}
