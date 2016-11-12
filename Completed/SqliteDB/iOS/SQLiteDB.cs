using System;
using Foundation;
using UIKit;
using SQLite.Net;
using SQLite.Net.Async;
using SQLite.Net.Platform.XamarinIOS;
using sqlitetest1.Interfaces;

namespace sqlitetest1.iOS
{
	public sealed class SQLiteDB : ISQLiteNet
    {
		private SQLiteConnectionWithLock _Conn;

		public SQLiteDB()
		{
			App.WriteToLog($"SQLiteDB Constructor {DateTime.Now.ToString("T")}");
			// added for notification support.  make sure the connect is closed and disposed of before openning another
			// this class is a Singleton.  Only one instance should exist ever.
			// class is registered with the IoC Container
			ResetConnection();
			InitDB();
		}

        public string FileName
        {
            get { return "pseadb.db3"; }
        }

		public string DBPath
        {
            get
			{
				string path = this.DocsDir() + "/" + FileName;
				System.Diagnostics.Debug.WriteLine($"sql lite path:= {path}");
				return path;

            }
        }
			
		/// <summary>
		/// close the Connection
		/// </summary>
		public void ResetConnection ()
		{
			App.WriteToLog($"ResetConnection1 {DateTime.Now.ToString("T")}");
			if (_Conn != null)
			{
				App.WriteToLog($"ResetConnection2 {DateTime.Now.ToString("T")}");
				_Conn.Close();
				_Conn.Dispose();
			}
			App.WriteToLog($"ResetConnection3 {DateTime.Now.ToString("T")}");
			_Conn = null;
		}

		/// <summary>
		/// Close the Connection, Delete the Database, reInit database.
		/// </summary>
		public void ReInitConnection ()
		{
			App.WriteToLog($"ReInitConnection1 {DateTime.Now.ToString("T")}");
			ResetConnection();
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
			InitDB();
		}


		public string DocsDir ()
		{
			var version = int.Parse(UIDevice.CurrentDevice.SystemVersion.Split('.')[0]);
			string docsDir = "";
			if (version>=8) {
				var docs = NSFileManager.DefaultManager.GetUrls (NSSearchPathDirectory.DocumentDirectory, NSSearchPathDomain.User) [0];
				docsDir = docs.Path;
			} else {
				docsDir = Environment.GetFolderPath (Environment.SpecialFolder.MyDocuments);
			}
			return docsDir;
		}

		public SQLiteAsyncConnection Connection
		{
			get
			{
				App.WriteToLog($"Connection1 {DateTime.Now.ToString("T")}");
				if (this._Conn == null)
				{
					App.WriteToLog($"this.Conn == null {DateTime.Now.ToString("T")}");
					var platform = new SQLitePlatformIOS();
					platform.SQLiteApi.Config(SQLite.Net.Interop.ConfigOption.Serialized);

					this._Conn = new SQLite.Net.SQLiteConnectionWithLock(platform,
						new SQLite.Net.SQLiteConnectionString(DBPath, false, 
					        openFlags: SQLite.Net.Interop.SQLiteOpenFlags.ReadWrite | SQLite.Net.Interop.SQLiteOpenFlags.Create | SQLite.Net.Interop.SQLiteOpenFlags.FullMutex));
				}
				App.WriteToLog($"this.Conn OK {DateTime.Now.ToString("T")}");
				return new SQLiteAsyncConnection(() => this._Conn);
			}
		}

        private void InitDB()
        {
			if (this._Conn == null)
			{
				var path = DBPath;
				App.WriteToLog($"InitDB path: {path} {DateTime.Now.ToString("T")}");
				if (!System.IO.File.Exists(path))
				{
					App.WriteToLog($"InitDB not found! {DateTime.Now.ToString("T")}");
					var Copypath = System.IO.Path.Combine(NSBundle.MainBundle.ResourcePath, "DB");
					Copypath = System.IO.Path.Combine(Copypath, FileName);

					App.WriteToLog($"InitDB copy Path: {Copypath} {DateTime.Now.ToString("T")}");
					NSFileManager.SetSkipBackupAttribute(path, true);
					System.IO.File.Delete(path);
					System.IO.File.Copy(Copypath, path);
					App.WriteToLog($"InitDB Completed {DateTime.Now.ToString("T")}");
				}
			}
        }
    }
}