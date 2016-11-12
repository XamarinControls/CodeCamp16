using System;
using CodeCamp2016.Forms.ViewModels;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.Views
{
	public class CartPage : ContentPage
	{
		public CartPage()
		{
			this.Title = "Cart";
			
			this.BindingContext = new CartViewModel(this.Navigation);
			
			var list = new ListView();
			list.HasUnevenRows = true;
			list.SetBinding(ItemsView<Cell>.ItemsSourceProperty, "Items");


			var template = new DataTemplate(() =>
			{
				var view = new ViewCell();

				var layout = new StackLayout();
				layout.VerticalOptions = LayoutOptions.Start;
				layout.Orientation = StackOrientation.Horizontal;
				layout.Padding = new Thickness(4, 2, 4, 2);

				var label = new Label();
				label.SetBinding(Label.TextProperty, "ItemDesc");
				label.VerticalOptions = LayoutOptions.Center;
				label.HorizontalOptions = LayoutOptions.StartAndExpand;

				var button = new Button();
				button.Text = "Delete";
				button.BackgroundColor = Color.Red;
				button.SetBinding(Button.CommandParameterProperty, "ID");
				button.TextColor = Color.White;
				button.SetBinding(Button.CommandProperty, new Binding("DeleteCommand", source: this.BindingContext));

				layout.Children.Add(label);
				layout.Children.Add(button);

				view.View = layout;
				return view;
			});

			list.ItemTemplate = template;

			this.Content = list;
		}
	}
}

