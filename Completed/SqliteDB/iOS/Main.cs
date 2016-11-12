using System;
using System.Collections.Generic;
using System.Linq;

using Foundation;
using UIKit;

namespace sqlitetest1.iOS
{
	public class Application
	{
		// This is the main entry point of the application.
		static void Main(string[] args)
		{
			var path = System.Environment.GetFolderPath(Environment.SpecialFolder.Personal);
			System.Diagnostics.Debug.WriteLine(path);
			using (var writer = new System.IO.StreamWriter(System.IO.Path.Combine(path, "log.txt"), true))
			{
				writer.Write("test");
				writer.Flush();
				writer.Close();
			}
			// if you want to use a different Application Delegate class from "AppDelegate"
			// you can specify it here.
			UIApplication.Main(args, null, "AppDelegate");
		}
	}
}
