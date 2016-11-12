using System;
using SQLite.Net.Attributes;

namespace sqlitetest1.DB
{
	[Table("MyRow")]
	public class MyRow
	{
		public MyRow()
		{
		}

		[PrimaryKey, AutoIncrement]
		public Int32 ID { get; set; }

		public string Username { get; set; }

		public string MobileID { get; set; }

		public string TestAddress { get; set; }
	}
}
