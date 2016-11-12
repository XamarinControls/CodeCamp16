using SQLite.Net.Attributes;
using System;

namespace sqlitetest1.DB
{
    [Table("Config")]
	public class Config 
    {
		public Config() 
		{
		}

		[PrimaryKey, AutoIncrement]
		public Int32 ID {get;set;}
        
        [MaxLength(20)]
        public Int32 MobileID { get; set; }
        
        public Int32 PeopleID { get; set; }
        
        [MaxLength(100)]
        public string UserID { get; set; }

		public Int32 LocalID { get; set; }
    }
} 


