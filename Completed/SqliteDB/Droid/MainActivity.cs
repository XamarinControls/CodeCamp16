using System;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;

[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.Droid.NativeDirectory))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.Droid.SQLiteDB))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.Droid.NativeFile))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.Droid.ResourceManager))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.droid.Library.AsyncHelper))]
namespace sqlitetest1.Droid
{
	[Activity(Label = "sqlitetest1.Droid", Icon = "@drawable/icon", Theme = "@style/MyTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
	public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
	{
		protected override void OnCreate(Bundle bundle)
		{
			TabLayoutResource = Resource.Layout.Tabbar;
			ToolbarResource = Resource.Layout.Toolbar;

			// init database

			base.OnCreate(bundle);


			global::Xamarin.Forms.Forms.Init(this, bundle);

			LoadApplication(new App());
		}
	}
}
