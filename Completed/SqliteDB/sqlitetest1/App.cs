using System;
using System.Collections.Generic;
using sqlitetest1.Interfaces;
using Xamarin.Forms;

namespace sqlitetest1
{
	public class App : Application
	{
		public static DBManager dbManager;

		public static List<string> Log;

		public static bool StopOutput = false;

		public App()
		{
			dbManager = new DBManager(new[] { "sqlitetest1.DB" });

			if (Log == null) Log = new List<string>();

			WriteToLog($"Before SqlInt {DateTime.Now.ToString("T")}");
			// create database
			DependencyService.Get<ISQLiteNet>(DependencyFetchTarget.GlobalInstance);
			App.dbManager.Init();

			//dbManager.ClearTable<DB.MyRow>(runSync: true);

			WriteToLog($"After SqlInt {DateTime.Now.ToString("T")}");

			MainPage = new NavigationPage(new StartPage());
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

		public static void WriteToLog(string message)
		{
			if (Log == null) Log = new List<string>();

			if (!StopOutput)
			{
				Log.Add(message);
				System.Diagnostics.Debug.WriteLine(message);
			}
		}
	}
}
