using System;
using CodeCamp2016.Forms.Exts;
using CodeCamp2016.Forms.ViewModels;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.Views
{
	public class RegisterPage : ContentPage
	{
		public RegisterPage()
		{
			this.Title = "Register";

			this.BindingContext = new RegisterViewModel(this.Navigation);

			// add toolbar
			var tool1 = new ToolbarItem
			{
				Text = "Save"
			};

			tool1.SetBinding(MenuItem.CommandProperty, "SaveCommand");
			this.ToolbarItems.Add(tool1);

			// build a grid
			var grid = new Grid();
			grid.ColumnSpacing = 2;
			grid.Padding = new Thickness(5, 20, 5, 0);

			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });
			grid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(50) });

			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(100) });
			grid.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) });

			grid.AddChild(new Label
			{
				Text = "Email:",
				FontAttributes = FontAttributes.Bold,
			}, 0, 0);


			grid.AddChild(new Label
			{
				Text = "Username:",
				FontAttributes = FontAttributes.Bold,
			}, 1, 0);


			grid.AddChild(new Label
			{
				Text = "Password:",
				FontAttributes = FontAttributes.Bold,
			}, 2, 0);



			var emailEntry = new Entry { Placeholder = "Email Address" };
			var UserNameEntry = new Entry { Placeholder = "UserName" };
			var passEntry = new Entry { Placeholder = "Password" };
			passEntry.IsPassword = true;

			emailEntry.SetBinding(Entry.TextProperty, "Email");
			UserNameEntry.SetBinding(Entry.TextProperty, new Binding("UserName"));
			passEntry.SetBinding(Entry.TextProperty, "Password");

			// add
			grid.AddChild(emailEntry, 0, 1);
			grid.AddChild(UserNameEntry, 1, 1);
			grid.AddChild(passEntry, 2, 1);

			this.Content = grid;
		}
	}
}

