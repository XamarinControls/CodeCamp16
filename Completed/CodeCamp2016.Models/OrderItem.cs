using System;
using System.ComponentModel;
using Realms;

namespace CodeCamp2016.Models
{
	public class OrderItem : Realms.RealmObject, INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler PropertyChanged;

		[PrimaryKey]
		public Int32 ID { get; set; }

		public string ItemDesc { get; set; }

		public Double ItemTotal { get; set; }

		[Indexed]
		public bool Hidden { get; set; }
	}
}
