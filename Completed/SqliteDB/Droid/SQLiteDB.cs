using System;
using System.IO;
using System.Linq;
using SQLite.Net;
using SQLite.Net.Async;
using sqlitetest1.Interfaces;

namespace sqlitetest1.Droid
{
	public sealed class SQLiteDB : ISQLiteNet
    {
		private SQLiteConnectionWithLock _Conn;

		public SQLiteDB() 
        {
			App.WriteToLog($"SQLiteDB Constructor {DateTime.Now.ToString("T")}");
			ResetConnection();
			InitDB ();
        }

		/// <summary>
		/// Close the connection
		/// </summary>
		public void ResetConnection()
		{
			App.WriteToLog($"ResetConnection1 {DateTime.Now.ToString("T")}");
			if (_Conn != null) {
				App.WriteToLog($"ResetConnection2 {DateTime.Now.ToString("T")}");
				_Conn.Close ();
				_Conn.Dispose ();
			}
			App.WriteToLog($"ResetConnection3 {DateTime.Now.ToString("T")}");
			_Conn = null;
		}

		/// <summary>
		/// Close the Connection, Delete the Database, reInit database.
		/// </summary>
		public void ReInitConnection()
		{
			App.WriteToLog($"ReInitConnection1 {DateTime.Now.ToString("T")}");
			ResetConnection ();
			App.WriteToLog($"ReInitConnection2 {DateTime.Now.ToString("T")}");
			// delete the database
			var path = DBPath;
			if (System.IO.File.Exists(path))
			{
				App.WriteToLog($"ReInitConnection delete {DateTime.Now.ToString("T")}");
				System.IO.File.Delete(path);
			}

			App.WriteToLog($"ReInitConnection2 {DateTime.Now.ToString("T")}");
			App.WriteToLog($"db path: {path} {DateTime.Now.ToString("T")}");
			InitDB ();
		}

        public string FileName
        {
            get { return "pseadb.db3"; }
        }

        public string DBPath
        {
            get
            {
                var documents = System.Environment.GetFolderPath(System.Environment.SpecialFolder.Personal);
				var path = Path.Combine(documents, FileName);
				System.Diagnostics.Debug.WriteLine($"sql lite path:= {path}");
				return path;
            }
        }

		/// <summary>
		/// used internally by the service class
		/// </summary>
		/// <value>The SYNC conn.</value>
		public SQLiteAsyncConnection Connection
		{
			get {
				App.WriteToLog($"Connection1 {DateTime.Now.ToString("T")}");
				if (this._Conn == null)
				{
					App.WriteToLog($"this.Conn == null {DateTime.Now.ToString("T")}");
					var platform = new SQLite.Net.Platform.XamarinAndroid.SQLitePlatformAndroid();

					// for threads
					platform.SQLiteApi.Shutdown();
					platform.SQLiteApi.Config(SQLite.Net.Interop.ConfigOption.Serialized);
					platform.SQLiteApi.Initialize();

					this._Conn = new SQLite.Net.SQLiteConnectionWithLock(platform,
						   new SQLite.Net.SQLiteConnectionString(DBPath, false,
									  openFlags: SQLite.Net.Interop.SQLiteOpenFlags.ReadWrite | SQLite.Net.Interop.SQLiteOpenFlags.Create | SQLite.Net.Interop.SQLiteOpenFlags.FullMutex));
				}
				return new SQLiteAsyncConnection(() => this._Conn);
			}
		}

		/// <summary>
		/// Check if Table Exists
		/// </summary>
		/// <param name="conn">Conn.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public bool Exists<T>(SQLiteConnection conn) where T : class
		{
			try
			{
				if (conn != null) {
					var tableName = typeof(T).Name;
					return conn.GetTableInfo(tableName).Any();
				}
			}
			catch {
			}

			return false;
		}

			
		private void InitDB()
		{
			if (this._Conn == null)
			{
				var path = DBPath;
				App.WriteToLog($"InitDB path: {path} {DateTime.Now.ToString("T")}");
				if (!System.IO.File.Exists (path)) {
					try {
						App.WriteToLog($"InitDB not found! {DateTime.Now.ToString("T")}");
						var s = GlobalSettings.GetContext.Resources.OpenRawResource (Resource.Raw.pseadb);  // RESOURCE NAME ###
						// create a write stream
						FileStream writeStream = new FileStream (path, FileMode.OpenOrCreate, FileAccess.Write);
						// write to the stream
						ReadWriteStream (s, writeStream);
						App.WriteToLog($"InitDB Completed {DateTime.Now.ToString("T")}");
					} catch (Exception ex){
						App.WriteToLog($"InitDB Exception {DateTime.Now.ToString("T")}");
						throw ex;
					}
				}
			}
		}

        /// <summary>
        /// helper method to get the database out of /raw/ and into the user filesystem
        /// </summary>
		private void ReadWriteStream(Stream readStream, Stream writeStream)
        {
            int Length = 256;
            Byte[] buffer = new Byte[Length];
            int bytesRead = readStream.Read(buffer, 0, Length);
            // write the required bytes
            while (bytesRead > 0)
            {
                writeStream.Write(buffer, 0, bytesRead);
                bytesRead = readStream.Read(buffer, 0, Length);
            }
            readStream.Close();
            writeStream.Close();
        }
    }
}