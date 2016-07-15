using GalleryMultiSelection.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Xamarin.Forms;

namespace GalleryMultiSelection
{
	public class App : Application
	{
        private IGalleryService galleryServ = DependencyService.Get<IGalleryService>();
        Image img;
        Button btn = new Button();
        public App ()
		{
            btn.Clicked += (s, o) =>
            {
                galleryServ.OpenGallery();
            };
            

            
            
            btn.Text = "Choose photos";

            img = new Image();
            img.Source = "facebook.jpg";
            MessagingCenter.Subscribe<IGestureListner, List<System.IO.Stream>>(this, "PostProject", (sender, args) =>
            {
                img.Source =ImageSource.FromStream(()=>args.ElementAt(0)) ;
            });
        // The root page of your application
        MainPage = new ContentPage {
                Content = new StackLayout {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    Orientation=StackOrientation.Vertical,
                    Children = {
                        new Label {
                            XAlign = TextAlignment.Center,
                            Text = "Welcome to Xamarin Forms!"
                        },
                        btn,
                        img,
					}
				}
			};
		}

		protected override void OnStart ()
		{
			// Handle when your app starts
		}

		protected override void OnSleep ()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume ()
		{
			// Handle when your app resumes
		}
	}
}
