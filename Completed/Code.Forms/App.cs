using System;
using CodeCamp2016.Forms.Views;
using Xamarin.Forms;
using System.Linq;

namespace CodeCamp2016.Forms
{
	public class App : Application
	{
		public App()
		{
			var user = Realms.Realm.GetInstance();
			var item = user.All<Models.Login>().FirstOrDefault(x => x.SaveLogin == true);
			if (item != null)
			{
				MainPage = new NavigationPage(new ItemsPage());
			}
			else
			{
				MainPage = new NavigationPage(new LoginPage());
			}
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
