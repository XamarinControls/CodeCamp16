using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PropertyChanged;
using Xamarin.Forms;
using System.Linq;
using Realms;
using CodeCamp2016.Forms.Views;
using System.Collections.Generic;

namespace CodeCamp2016.Forms.ViewModels
{
	[ImplementPropertyChanged]
	public class ItemsViewModel
	{
		private readonly INavigation _Nav;
		private readonly ItemsPage _Page;

		public ItemsViewModel(INavigation navigation, ItemsPage page)
		{
			// save Navigation
			this._Nav = navigation;

			// save Page
			this._Page = page;		
		}

		public void OnAppearing()
		{
			
		}
	}
}
