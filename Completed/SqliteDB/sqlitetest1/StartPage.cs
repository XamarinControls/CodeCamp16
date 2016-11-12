using System;

using Xamarin.Forms;

namespace sqlitetest1
{
	public class StartPage : ContentPage
	{
		public StartPage()
		{
			this.Title = "Start :)";

			Content = new StackLayout
			{
				Children = {
					new Label{
						Text = "New Version 1"
					},
					new Button
					{
						Text = "Start Test",
						Command = new Command(async () => {
							await Navigation.PushAsync(new HomeTabPage());
						})
					}
				}
			};
		}
	}
}

