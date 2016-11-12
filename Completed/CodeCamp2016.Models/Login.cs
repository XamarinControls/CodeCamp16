using System;
using System.ComponentModel;

namespace CodeCamp2016.Models
{
	public class Login : Realms.RealmObject, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		public Login()
		{
		}

		public bool SaveLogin { get; set; }

		public string UserName { get; set; }
	}
}
