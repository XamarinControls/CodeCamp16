using CodeCamp2016.Forms.ViewModels;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.Views
{
	public partial class LoginPage : ContentPage
	{
		public LoginPage()
		{
			this.BindingContext = new LoginViewModel(this.Navigation);
			InitializeComponent();
		}
	}
}
