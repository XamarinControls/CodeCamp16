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

		private Xamarin.Forms.Command _ShowItemsCommand;
		private Xamarin.Forms.Command<object> _OrderItemCommand;
		private bool _IsFirstLoad = true;
		private Realm _Realm;

		public ItemsViewModel(INavigation navigation, ItemsPage page)
		{
			// save Navigation
			this._Nav = navigation;

			// save Page
			this._Page = page;

			this._Realm = Realm.GetInstance();

			// clear items
			this._Realm.Write(() => this._Realm.RemoveAll<Models.OrderItem>());

			// must be set global to capture change events
			this._Realm.All<Models.OrderItem>().SubscribeForNotifications(RealmObjectChanged);
		}

		public void OnAppearing()
		{
			if (this.IsBusy == false)
			{
				this.IsBusy = true;
				if (this._IsFirstLoad)
				{
					LoadData();
				}
			}
		}

		public void LoadData()
		{
			var task = Task.Run(async () =>
			{
				return await Rest.ServiceCalls.GetItems();
			});

			task.ContinueWith((arg) =>
			{
				Device.BeginInvokeOnMainThread(async () =>
			   {
				   var result = await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(null, "Error Loading", "Re-Try", "Cancel");
				   if (result) LoadData();
			   });

			}, TaskContinuationOptions.OnlyOnFaulted);

			task.ContinueWith((arg) =>
			{
				try
				{
					var instance = Realm.GetInstance();
					using (var transaction = instance.BeginWrite())
					{
						var id = 0;
						foreach (var item in arg.Result)
						{
							var orderItem = instance.CreateObject<Models.OrderItem>();
							orderItem.ID = ++id;
							orderItem.Hidden = false;
							orderItem.ItemDesc = item.ItemDesc;
							orderItem.ItemTotal = item.ItemTotal;
						}
						transaction.Commit();
					}

					SetItemCount();

					// reset statuses
					this.IsBusy = false;
					this._IsFirstLoad = false;
				}
				catch (Exception)
				{
					Device.BeginInvokeOnMainThread(async() =>
				   {
					   var result = await Xamarin.Forms.Application.Current.MainPage.DisplayAlert(null, "Error Loading", "Re-Try", "Cancel");
					   if (result) LoadData();
					});
				}
			}, TaskContinuationOptions.OnlyOnRanToCompletion);
		}


		public bool IsBusy { get; set; }

		public IEnumerable<Models.OrderItem> Items { get; set; }

		private void SetItemCount()
		{
			var c = this._Realm.All<Models.OrderItem>().Where(x => x.Hidden == true).Count();
			this._Page.SetToolBarText($"Items {c}");
		}

		private void RealmObjectChanged(RealmResults<Models.OrderItem> sender, RealmResults<Models.OrderItem>.ChangeSet changes, Exception error)
		{
			this.Items = sender.Where(x => x.Hidden == false);
			SetItemCount();
		}

		public Xamarin.Forms.Command ShowItemsCommand
		{
			get
			{
				return this._ShowItemsCommand ?? (this._ShowItemsCommand = new Xamarin.Forms.Command(async () =>
			   {
					var c = this._Realm.All<Models.OrderItem>().Where(x => x.Hidden == true).Count();
				    if (c > 0)
				    {
					   await this._Nav.PushAsync(new CartPage());
					}
			   }, () => true));
			}
		}

		public Xamarin.Forms.Command<object> OrderItemCommand
		{
			get
			{
				return this._OrderItemCommand ?? (this._OrderItemCommand = new Xamarin.Forms.Command<object>( (args) =>
			   {
				   var id = Convert.ToInt32(args);
				   var item = this._Realm.All<Models.OrderItem>().Where(x => x.ID == id).First();
				   using (var transaction = this._Realm.BeginWrite())
				   {
					   item.Hidden = true;
					   transaction.Commit();
					}

			   }, (t) => true));
			}
		}
	}
}
