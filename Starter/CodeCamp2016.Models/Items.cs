using System;

namespace CodeCamp2016.Models
{
	/// <summary>
	/// Service Result Items
	/// </summary>
	public class Items
	{
		public Items()
		{
		}

		public Items(string desc, double total)
		{
			this.ItemDesc = desc;
			this.ItemTotal = total;
		}
		public string ItemDesc { get; set; }

		public double ItemTotal { get; set; }
	}
}
