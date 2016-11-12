using System;
using System.Collections.Generic;
using System.Linq;
using CodeCamp2016.Forms;
using Foundation;
using UIKit;


[assembly: Xamarin.Forms.Dependency(typeof(CodeCamp2016.iOS.Alerts.AlertMessages))]
[assembly: Xamarin.Forms.Dependency(typeof(CodeCamp2016.iOS.Rest.RestCoreClient))]
namespace CodeCamp2016.iOS
{
	[Register("AppDelegate")]
	public partial class AppDelegate : global::Xamarin.Forms.Platform.iOS.FormsApplicationDelegate
	{
		public override bool FinishedLaunching(UIApplication app, NSDictionary options)
		{
			global::Xamarin.Forms.Forms.Init();

			LoadApplication(new App());

			// default realm file location
			System.Diagnostics.Debug.WriteLine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments));

			return base.FinishedLaunching(app, options);
		}
	}
}
