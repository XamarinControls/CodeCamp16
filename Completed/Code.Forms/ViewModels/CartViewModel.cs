using System;
using System.Collections.Generic;
using System.Linq;
using PropertyChanged;
using Realms;
using Xamarin.Forms;

namespace CodeCamp2016.Forms.ViewModels
{
	[ImplementPropertyChanged]
	public class CartViewModel
	{
		private readonly INavigation _Nav;
		private Realm _Realm;
		private Xamarin.Forms.Command<object> _DeleteCommand;

		public CartViewModel(INavigation navigation)
		{
			this._Nav = navigation;

			this._Realm = Realm.GetInstance();

			this.Items = this._Realm.All<Models.OrderItem>().Where(x => x.Hidden == true) as IEnumerable<Models.OrderItem>;

			// must be set global to capture change events
			this._Realm.All<Models.OrderItem>().SubscribeForNotifications(RealmObjectChanged);
		}

		private void RealmObjectChanged(RealmResults<Models.OrderItem> sender, RealmResults<Models.OrderItem>.ChangeSet changes, Exception error)
		{
			this.Items = sender.Where(x => x.Hidden == true);
		}

		public IEnumerable<Models.OrderItem> Items { get; set; }

		public Xamarin.Forms.Command<object> DeleteCommand
		{
			get
			{
				return this._DeleteCommand ?? (this._DeleteCommand = new Xamarin.Forms.Command<object>((args) =>
			   {
				   var id = Convert.ToInt32(args);
				   var item = this._Realm.All<Models.OrderItem>().Where(x => x.ID == id).First();
				   using (var transaction = this._Realm.BeginWrite())
				   {
					   item.Hidden = false;
					   transaction.Commit();
				   }
			   }, (x) => true));
			}
		}
	}
}
