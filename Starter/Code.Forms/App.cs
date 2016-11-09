using System;
using Xamarin.Forms;
using System.Linq;

namespace CodeCamp2016.Forms
{
	public class App : Application
	{
		public App()
		{
            MainPage = new NavigationPage(new ContentPage
            {
                Title = "Code Camp 2016",
                Content = new StackLayout
                {
                    Children =
                    {
                        new Label { Text = "Welcome to Xamarin Forms Demo!" }
                    }
                }
            });
		}

		protected override void OnStart()
		{
			// Handle when your app starts
		}

		protected override void OnSleep()
		{
			// Handle when your app sleeps
		}

		protected override void OnResume()
		{
			// Handle when your app resumes
		}
	}
}
