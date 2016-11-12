using System;
using CodeCamp2016.Forms.Alerts;
using CodeCamp2016.Forms.Views;
using PropertyChanged;
using Xamarin.Forms;
using System.Linq;
using System.Threading.Tasks;

namespace CodeCamp2016.Forms.ViewModels
{
	[ImplementPropertyChanged]
	public class LoginViewModel
	{
		private Xamarin.Forms.Command _LoginCommand;
		private Xamarin.Forms.Command _RegisterCommand;

		private IAlertMessages _Alerts;
		private readonly INavigation _Nav;

		public LoginViewModel(INavigation navigation)
		{
			// save Navigation
			this._Nav = navigation;

			// get alerts
			this._Alerts = DependencyService.Get<IAlertMessages>();
		}

		public string UserName { get; set; }

		public string Password { get; set; }

		public bool IsSavedPassword { get; set; }

		public Xamarin.Forms.Command LoginCommand
		{
			get
			{
				return this._LoginCommand ?? (this._LoginCommand = new Xamarin.Forms.Command(async () =>
				{
					this._Alerts.Show("Validating username");
					await Task.Delay(900);

					var u = this.UserName == null ? "" : this.UserName.ToLower();
					var p = this.Password == null ? "" : this.Password.ToLower();

                    var loginResult = await Rest.ServiceCalls.Login(u, p);
					if (loginResult == "")
					{
						if (this.IsSavedPassword)
						{
							using (var realm = Realms.Realm.GetInstance())
							{
								var c = realm.All<Models.Login>().First();
								if (c == null)
								{
									realm.Write(() =>
									{
										var obj = realm.CreateObject<Models.Login>();
										obj.UserName = u;
										obj.SaveLogin = true;
									});
								}
								else
								{
									realm.Write(() =>
								   {
									   c.UserName = u;
									});
								}
							}
						}

						await this._Nav.PushAsync(new ItemsPage());
						this._Alerts.Dismiss();
					}
					else
					{
						this._Alerts.Dismiss();

						await Xamarin.Forms.Application.Current.MainPage.DisplayAlert("Error", loginResult, "OK");
						// clear password
						this.Password = "";
					}


				}, () => true));
			}
		}

		public Xamarin.Forms.Command RegisterCommand
		{
			get
			{
				return this._RegisterCommand ?? (this._RegisterCommand = new Xamarin.Forms.Command(async() =>
			   {
				   await this._Nav.PushAsync(new RegisterPage());

			   }, () => true));
			}
		}
	}
}
