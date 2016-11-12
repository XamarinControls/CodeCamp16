using System;
using SQLite.Net.Attributes;

namespace sqlitetest1.DB
{
	[Table("OneRow")]
	public class OneRow
	{
		public OneRow()
		{
		}

		[PrimaryKey]
		public Int32 ID { get; set; }

		public string FirstName { get; set; }

		public string LastName { get; set; }

		public Int32 DOB { get; set; }

		public string Username { get; set; }
	}
}
