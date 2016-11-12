using System;

using Xamarin.Forms;

namespace sqlitetest1
{
	public class HomeTabPage : TabbedPage
	{
		public HomeTabPage()
		{
			this.Children.Add(new LogPage());
			this.Children.Add(new DBTest());
			this.Children.Add(new DBListPage());
			this.Children.Add(new UpdateTestPage());
			this.Children.Add(new AsyncTestPage());
		}
	}
}

