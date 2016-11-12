using System;

using Xamarin.Forms;

namespace sqlitetest1
{
	public class LogPage : ContentPage
	{
		private ListView list;
		public LogPage()
		{
			this.Title = "Log";

			var tool1 = new ToolbarItem();
			tool1.Text = "Clear";
			tool1.Command = new Command(() =>
			{
				App.Log = new System.Collections.Generic.List<string>();
				BindData();
			});

			this.ToolbarItems.Add(tool1);

			list = new ListView();

			list.HasUnevenRows = true;

			list.ItemTemplate = new DataTemplate(() =>
		   {
			   var label = new Label();
			   label.VerticalTextAlignment = TextAlignment.Center;
			   label.SetBinding(Label.TextProperty, ".");

			   return new ViewCell
			   {
				   View = label
				};
		   });

			Content = list;
		}
		protected override void OnAppearing()
		{
			base.OnAppearing();
			list.ItemsSource = null;
			list.ItemsSource = App.Log;
			BindData();
		}

		public void BindData()
		{
			Device.BeginInvokeOnMainThread(() =>
		   {
			   list.ItemsSource = App.Log;
		   });
		}
	}
}

