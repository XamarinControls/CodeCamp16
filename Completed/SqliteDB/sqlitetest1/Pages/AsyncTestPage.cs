using System;
using Xamarin.Forms;

namespace sqlitetest1
{
	public class AsyncTestPage : ContentPage
	{
		public AsyncTestPage()
		{
			this.Title = "AsynSync";

			var label = new Label();
			var label1 = new Label();

			var button = new Button
			{
				Text = "Update UI",
				Command = new Command((obj) =>
				{
					label.Text = DateTime.Now.Ticks.ToString();
				})
			};

			var button2 = new Button
			{
				Text = "Run",
				Command = new Command(async (obj) =>
				{
					var result = await DisplayAlert(null, "Run Async", "YES", "NO");
					Int32 rows = 0;
					if (result)
					{
						Device.BeginInvokeOnMainThread( () => label.Text = "Sync - Locking");
						// sync
						rows = await App.dbManager.RowCount<DB.Config>(true);
					}
					else
					{
						Device.BeginInvokeOnMainThread(() => label.Text = "Async");
						// async
						rows = await App.dbManager.RowCount<DB.Config>(false);
					}
					await DisplayAlert(null, $"{rows}", "OK");
				})
			};

			this.Content = new StackLayout
			{
				Children =
				{
					label,
					label1,
					button, button2
				}
			};

		}

	}
}
