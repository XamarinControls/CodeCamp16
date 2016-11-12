using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.iOS.NativeDirectory))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.iOS.SQLiteDB))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.iOS.NativeFile))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.iOS.ResourceManager))]
[assembly: Xamarin.Forms.Dependency(typeof(sqlitetest1.iOS.Library.AsyncHelper))]
namespace sqlitetest1.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			return base.FinishedLaunching(app, options);
		}
	}
}
