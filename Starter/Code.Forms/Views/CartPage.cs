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
			
			
		}
	}
}

