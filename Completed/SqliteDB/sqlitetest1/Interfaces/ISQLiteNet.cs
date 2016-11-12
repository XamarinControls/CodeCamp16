using System;
using SQLite.Net;
using SQLite.Net.Async;

namespace sqlitetest1.Interfaces
{
	public interface ISQLiteNet
	{
		/// <summary>
		/// database path
		/// </summary>
		/// <value>The DB path.</value>
       string DBPath { get; }

		/// <summary>
		/// Database Connection
		/// </summary>
		/// <value>The conn.</value>
		SQLiteAsyncConnection Connection { get; }

		/// <summary>
		/// Database filename
		/// </summary>
		/// <value>The name of the file.</value>
        string FileName { get; }

		/// <summary>
		/// Close the connection
		/// </summary>
		void ResetConnection ();

		/// <summary>
		/// Close the Connection, Delete the Database, reInit database.
		/// </summary>
		void ReInitConnection();
	}
}

