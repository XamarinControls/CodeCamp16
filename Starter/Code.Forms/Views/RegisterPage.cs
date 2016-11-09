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

		}
	}
}

