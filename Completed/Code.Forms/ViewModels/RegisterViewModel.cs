using System;
using PropertyChanged;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.ViewModels
{
	[ImplementPropertyChanged]
	public class RegisterViewModel
	{
		private Xamarin.Forms.Command _SaveCommand;

		private readonly INavigation _Nav;

		public RegisterViewModel(INavigation navigation)
		{
			this._Nav = navigation;
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		public string Email { get; set; }

		public Xamarin.Forms.Command SaveCommand
		{
			get
			{
				return this._SaveCommand ?? (this._SaveCommand = new Xamarin.Forms.Command(async () =>
			   {
				   if (String.IsNullOrEmpty(this.UserName) || String.IsNullOrEmpty(this.Password))
				   {
						await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(null, "Enter a username and Password", "OK");
				   }
				   else
				   {
					   var registerResult = await Rest.ServiceCalls.Register(this.UserName, this.Password, this.Email);
					   if (registerResult == "")
					   {
						   await this._Nav.PopAsync();
					   }
					   else
					   {
						   await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(null, "Username in use, select another", "OK");
						   this.UserName = "";
					   }
				   }

			   }, () => true));
			}
		}
	}
}
