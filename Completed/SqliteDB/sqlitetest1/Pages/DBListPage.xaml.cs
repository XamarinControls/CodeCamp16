using System;
using System.Collections.Generic;

using Xamarin.Forms;

namespace sqlitetest1
{
	public partial class DBListPage : ContentPage
	{
		public DBListPage()
		{
			this.Title = "List";

			this.ToolbarItems.Add(new ToolbarItem
			{
				Text = "Load",
				Command = new Command(async (x) => {
					var data = await App.dbManager.AllRows<DB.Config>();
					this.list1.ItemsSource = null;
					this.list1.ItemsSource = data;
				})
			});

			InitializeComponent();
		}

	}
}
