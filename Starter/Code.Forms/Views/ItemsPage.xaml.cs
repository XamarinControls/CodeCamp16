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
			this.model = new ItemsViewModel(this.Navigation, this);

			this.BindingContext = model;
			InitializeComponent();
		}
	}
}
