using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using GalleryMultiSelection.Services.Interfaces;
using GalleryMultiSelection.Droid.Services;
using Xamarin.Forms;
using Android.Provider;
using Xamarin.Forms.Platform.Android;

[assembly: Dependency(typeof(GalleryService))]
namespace GalleryMultiSelection.Droid.Services
{
    public class GalleryService : FormsAppCompatActivity, IGalleryService, IGestureListner
    {
        List<string> paths;
        public GalleryService()
        {

        }
        public void OpenGallery()
        {
            Toast.MakeText(Xamarin.Forms.Forms.Context, "Select max 20 images", ToastLength.Long).Show();
            var imageIntent = new Intent(
                Intent.ActionPick);
            imageIntent.SetType("image/*");
            imageIntent.PutExtra(Intent.ExtraAllowMultiple, true);
            imageIntent.SetAction(Intent.ActionGetContent);
            ((Activity)Forms.Context).StartActivityForResult(Intent.CreateChooser(imageIntent, "Select photo"), 0);
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent intent)
        {
            base.OnActivityResult(requestCode, resultCode, intent);
            bool braked = false;
            string path = "";

            try
            {
                if (resultCode == Result.Ok)
                {
                    paths = new List<string>();
                    if (intent != null)
                    {
                        ClipData clipData = intent.ClipData;
                        if (clipData != null)
                        {

                            for (int i = 0; i < clipData.ItemCount; i++)
                            {

                                if (i > 19)
                                {
                                    braked = true;
                                    break;
                                }
                                ClipData.Item item = clipData.GetItemAt(i);
                                global::Android.Net.Uri uri = item.Uri;

                                //In case you need image's absolute path
                                path = GetPathToImage(uri);
                                paths.Add(path);
                            }

                        }
                        else
                        {
                            global::Android.Net.Uri uri = intent.Data;
                            path = GetPathToImage(uri);
                            paths.Add(path);
                        }

                        MessagingCenter.Send<IGestureListner, List<string>>(this, "PostProject", paths);

                        if (braked == true)
                        {
                            // Toast.MakeText (Xamarin.Forms.Forms.Context, "Only the top 20 images will be uploaded", ToastLength.Long).Show ();
                        }

                    }
                }
                //Send the paths to forms
            }
            catch (Exception ex)
            {

                //Toast.MakeText (Xamarin.Forms.Forms.Context, "Unable to open, error:" + ex.ToString(), ToastLength.Long).Show ();
            }
        }

        private string GetPathToImage(global::Android.Net.Uri uri)
        {
            string doc_id = "";
            using (var c1 = ContentResolver.Query(uri, null, null, null, null))
            {
                c1.MoveToFirst();
                String document_id = c1.GetString(0);
                doc_id = document_id.Substring(document_id.LastIndexOf(":") + 1);
            }

            string path = null;

            // The projection contains the columns we want to return in our query.
            string selection = MediaStore.Images.Media.InterfaceConsts.Id + " =? ";
            using (var cursor = ManagedQuery(MediaStore.Images.Media.ExternalContentUri, null, selection, new string[] { doc_id }, null))
            {
                if (cursor == null) return path;
                var columnIndex = cursor.GetColumnIndexOrThrow(MediaStore.Images.Media.InterfaceConsts.Data);
                cursor.MoveToFirst();
                path = cursor.GetString(columnIndex);
            }
            return path;
        }

    }
}