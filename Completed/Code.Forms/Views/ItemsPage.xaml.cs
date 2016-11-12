using System;
using CodeCamp2016.Forms.ViewModels;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.Views
{
	public partial class ItemsPage : ContentPage
	{
		private ToolbarItem tool1;
		private readonly ItemsViewModel model;

		public ItemsPage()
		{
			// remove back
			NavigationPage.SetHasBackButton(this, false);
			
			this.model = new ItemsViewModel(this.Navigation, this);

			this.BindingContext = model;
			InitializeComponent();

			tool1 = new ToolbarItem();
			tool1.SetBinding(MenuItem.CommandProperty, "ShowItemsCommand");
		}

		protected override void OnAppearing()
		{
			base.OnAppearing();
			model.OnAppearing();
		}

		public void SetToolBarText(string text)
		{
			Device.BeginInvokeOnMainThread(() =>
			{
				this.ToolbarItems.Remove(tool1);
				this.tool1.Text = text;
				this.ToolbarItems.Add(tool1);
			});
		}
	}
}
