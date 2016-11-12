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
using Xamarin.Forms.Platform.Android;
using Android.Util;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Threading;

namespace sqlitetest1.Droid
{
	public class GlobalSettings {

		public GlobalSettings()
		{

		}

		public static void RequestMainThread(Action action) {
			if (Android.App.Application.SynchronizationContext == SynchronizationContext.Current)
				action();
			else
				Android.App.Application.SynchronizationContext.Post(x => {
					try {
						action();
					}
					catch { }
				}, null);
		}

		public static async System.Threading.Tasks.Task InvokeOnMainThreadAndWait(Action act)
		{
			await System.Threading.Tasks.Task.Run (() => {
				using (var evt = new System.Threading.ManualResetEvent (false)) {
					(GlobalSettings.GetActivity).RunOnUiThread(()=> {
						if (act != null)
						{
							act.Invoke();
						}
						evt.Set();
					});

					evt.WaitOne();
				}
			});
		}

		public static void PostOnMainThread(Action act)
		{
			Android.App.Application.SynchronizationContext.Post ((x) => act.Invoke(), null);
		}


		public static void InvokeOnMainThread(Action act)
		{
			(GlobalSettings.GetActivity).RunOnUiThread(act);
		}

		public static Context GetContext
		{
			get 
			{ 
				try
				{
					return Xamarin.Forms.Forms.Context ?? Android.App.Application.Context as Context;
				}
				catch {
					return Android.App.Application.Context;
				}
			}
		}

		public static FormsAppCompatActivity GetActivity {
			get 
			{
				//var app = MainApplication.AppContext;
				//var app2 = app as FormsAppCompatActivity;
				//var app3 = app as Android.Support.V7.App.AppCompatActivity;
				return GetContext as FormsAppCompatActivity;
			}
		}

		public static Android.Support.V7.App.ActionBar GetActionBar {
			get 
			{
				var c = GetActivity;
				return c.SupportActionBar;
			}
		}

		/// <summary>
		/// Get Android System Service
		/// </summary>
		/// <returns>The service.</returns>
		/// <param name="name">Name.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T GetService<T>(string name) where T : class
		{
			return GetContext.GetSystemService(name) as T;
		}

		/// <summary>
		/// <code>
		///  var isCameraAvailable = AndroidGlobalSettings.GetContext.PackageManager.HasSystemFeature(PackageManager.FeatureCamera);
		/// </code>
		/// </summary>
		/// <returns><c>true</c> if has system feature the specified name; otherwise, <c>false</c>.</returns>
		/// <param name="name">Name.</param>
		public static bool HasSystemFeature(string name)
		{
			return GetContext.PackageManager.HasSystemFeature(name);
		}

		/// <summary>
		/// get the size of the Topbar
		/// </summary>
		/// <value>The size of the navigation bar.</value>
		public static Int32 TopBarSize
		{
			get
			{
				var temp = GlobalSettings.GetActivity;
				if (temp != null) {
					return temp.ActionBar.Height;
				}
				return 0;
			}
		}

		/// <summary>
		/// Has navigation bar. 
		/// </summary>-4
		/// <value><c>true</c> if has navigation bar; otherwise, <c>false</c>.</value>
		public static bool HasNavigationBar
		{
			get {
				try
				{
					var temp = GlobalSettings.GetActivity;
					if (temp != null) {
						return temp.ActionBar.IsShowing;
					}
					return false;
				}
				catch {
					return false;
				}
			}
		}

		/// <summary>
		/// returns the top level activity class name
		/// </summary>
		/// <value>The top activity.</value>
		public static string TopActivityClassName
		{
			get
			{
				var am = (ActivityManager)GetContext.GetSystemService(Android.Content.Context.ActivityService);
				var taskInfo = am.GetRunningTasks(1).First();
				return taskInfo.TopActivity.ClassName;
			}
		}

		/// <summary>
		/// Gets the Top View
		/// </summary>
		/// <value>The top view.</value>
		public static Android.Views.View TopView
		{
			get{
				return RootView;
			}
		}

		/// <summary>
		/// Gets the root view control.
		/// </summary>
		/// <value>The root view control.</value>
		public static Android.Views.View RootView {
			get 
			{				
				var a = (Activity)Forms.Context;

				//			var v2 = a.FindViewById(Android.Resource.Id.Content);
				//			var v3 = v2.RootView;
				//			var v =  a.CurrentFocus;
				//			return v;

				//http://stackoverflow.com/questions/4486034/get-root-view-from-current-activity
				//Android.Resource.Id
				var v = a.FindViewById(Android.Resource.Id.Content);
				//var v = a.FindViewById(Resource.Id.decor_content_parent);
				//var v = a.FindViewById(Resource.Id.content
				return v;
			}
		}
	}
}