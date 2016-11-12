using System;
using System.Linq.Expressions;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Collections;
using SQLite.Net.Async;
using SQLite.Net;
using sqlitetest1.Interfaces;
using sqlitetest1.Library;

// http://blog.thomasbandt.de/39/2433/de/blog/performance-optimization-of-sqlite-on-ios-with-xamarin.html
namespace sqlitetest1
{
    public class DBManager : IDBManager
    {
		private readonly List<string> TableNameSpace;
		private List<Type> TableData;


		private ISQLiteNet SqliteDB => Xamarin.Forms.DependencyService.Get<ISQLiteNet>();

		/// <summary>
		/// database lock
		/// </summary>
		private static readonly AsyncLock ReInsertLockMutex = new AsyncLock();
		private static readonly AsyncLock AllowOneLockMutex = new AsyncLock();
		private static readonly AsyncLock DeleteLockMutex = new AsyncLock();

		/// <summary>
		/// global connection reference
		/// </summary>
		/// <value>The connection.</value>
		public SQLiteAsyncConnection Connection => SqliteDB.Connection;

		/// <summary>
		/// DBManager constructor
		/// </summary>
		/// <param name="tableNameSpace">Table name space.</param>
		public DBManager(params string[] tableNameSpace)
        {
			App.WriteToLog($"DBManager Constructor {DateTime.Now.ToString("T")}");
			TableNameSpace = new List<string> (tableNameSpace);

			App.WriteToLog($"DBManager Tables {String.Join(":", TableNameSpace.Select(a => a))} {DateTime.Now.ToString("T")}");
			TableData = null;
			this.IsInit = false;
        }

		public DBManager()
		{
			throw new InvalidOperationException("Must be new DBManager(new string[] {tableNameSpace}");
		}

        private void CheckDBManager()
        {
            if (this.IsInit == false)
            {
                throw new Exception("Call IsInit before any database operations");
            }
            else if (this.IsMigrating)
            {
                throw new Exception("Unable to call database function while the database is Migrating to a new version");
            }
        }

        private void CheckExternalCall()
        {
            if (this.IsInit == false)
            {
                throw new Exception("Call IsInit before any database operations");
            }
        }


        public bool IsMigrating { get; set; } = false;

        public bool IsInit { get; private set; }

		/// <summary>
		/// Handle Exception
		/// </summary>
		/// <returns>The exception.</returns>
		/// <param name="ex">Ex.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		private T HandleException<T>(Exception ex)
		{
			App.WriteToLog(ex.Message);
			return default(T);
		}

		/// <summary>
		/// Handle Exception
		/// </summary>
		/// <param name="ex">Ex.</param>
		private void HandleException(Exception ex)
		{
			HandleException<object> (ex);
		}

		public Action<Exception> DBManagerExceptionHandler { get; set; }

		public string Path => SqliteDB.DBPath;

		public string FileName => SqliteDB.FileName;


        #region "External Methods - need SqliteConnectionObject. Checks if database has been Init. Ignores all Other flags

        public async Task<Int32> RowCount<T>(SQLiteAsyncConnection SqlConnection, bool runSync = false, bool ignoreCheck = false) where T : class
        {
            try
            {
                if (ignoreCheck == false)
                {
                    // check if database IsInit called
                    CheckExternalCall();
                }

                var task = SqlConnection.Table<T>().CountAsync().ConfigureAwait(false);
                Int32 data = -1;
                if (runSync)
                {
                    using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
                    {
                        task2.Run<Int32>(async () =>
                        {
                            return await task;
                        }, (obj) => data = obj.Result);
                    }
                }
                else
                {
                    data = await task;
                }
                return data;
            }
            catch (Exception s)
            {
                HandleException<Int32>(s);
                return -1;
            }
        }

        public bool TableExists(SQLiteAsyncConnection SqlConnection, string tableName)
        {
            string sql = String.Format("SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='{0}'", tableName);
            try
            {
                var result = Connection.ExecuteScalarAsync<Int32>(sql).ConfigureAwait(false).GetAwaiter().GetResult();
                return result == 1;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> DropTable<T>(SQLiteAsyncConnection SqlConnection, bool runSync = false, bool ignoreCheck = false) where T : class
        {
            try
            {
                if (ignoreCheck == false)
                {
                    // check if database IsInit called
                    CheckExternalCall();
                }

                Int32 data = 0;
                var table = typeof(T).Name;
                if (TableExists(SqlConnection, table))
                {
                    var task = SqlConnection.ExecuteAsync($"DROP TABLE {typeof(T).Name}", new object[] { }).ConfigureAwait(false);
                    if (runSync)
                    {
                        using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
                        {
                            task2.Run<Int32>(async () =>
                            {
                                return await task;
                            }, (x) => data = x.Result);
                        }
                    }
                    else
                    {
                        data = await task;
                    }
                }
                return data > 0; ;
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }

        public async Task<bool> ClearTable<T>(SQLiteAsyncConnection SqlConnection, bool runSync = false, bool ignoreCheck = false) where T : class
        {
            try
            {
                if (ignoreCheck == false)
                {
                    // check if database IsInit called
                    CheckExternalCall();
                }

                bool data = false;
                var result = new TaskCompletionSource<bool>();
                var task = SqlConnection.RunInTransactionAsync((SQLiteConnection conn) =>
                {
                    conn.DropTable<T>();
                    conn.CreateTable<T>();
                }).ContinueWith((arg) =>
                {
                    if (arg.IsFaulted)
                    {
                        HandleException<bool>(arg.Exception);
                        result.SetResult(false);
                    }
                    else
                    {
                        result.SetResult(true);
                    }
                }).ConfigureAwait(false);

                if (runSync)
                {
                    using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
                    {
                        task2.Run<bool>(async () =>
                        {
                            await result.Task;
                            return result.Task.Result;
                        }, (x) => data = x.Result);
                    }
                }
                else
                {
                    await result.Task;
                    data = result.Task.Result;
                }

                return data;
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }

        public async Task<bool> RunInTransaction(SQLiteAsyncConnection SqlConnection, Action<SQLiteConnection> transaction, bool ignoreCheck = false)
        {
            try
            {
                if (ignoreCheck == false)
                {
                    // check if database IsInit called
                    CheckExternalCall();
                }

                var result = new TaskCompletionSource<bool>();
                await SqlConnection.RunInTransactionAsync(transaction).ContinueWith((arg) =>
                {
                    if (arg.IsFaulted)
                    {
                        HandleException<bool>(arg.Exception);
                        result.SetResult(false);
                    }
                    else
                    {
                        result.SetResult(true);
                    }
                }).ConfigureAwait(false);
                await result.Task;
                return result.Task.Result;
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }


        #endregion

        #region "ASYNC"

        public async Task<Int32> RowCount<T>(bool runSync = false) where T : class
        {
            try
            {
                CheckDBManager();
                return await RowCount<T>(Connection, runSync, true);
            }
            catch (Exception ex)
            {
                return HandleException<Int32>(ex);
            }
        }

        public async Task<T> QueryFirst<T>(Expression<Func<T, bool>> query, bool runSync = false) where T : class
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				T data = default(T);
				var task = Connection.Table<T>().Where(query).FirstOrDefaultAsync().ConfigureAwait(false);
				if (runSync)
				{
					App.WriteToLog("RowCount SYNC");
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<T>(async () =>
						{
							return await task;
						}, (obj) => data = obj.Result);
					}
				}
				else
				{
					data = await task;
				}
				return data;

			}
			catch (Exception s)
			{
				return HandleException<T>(s);
			}
		}

		public async Task<T> FirstRow<T>(bool runSync = false) where T : class
        {
			try
			{
				// check if database IsInit called
				CheckDBManager();

				T data = default(T);
				var task = Connection.Table<T>().FirstOrDefaultAsync().ConfigureAwait(false);
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<T>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
                return data;
			}
			catch (Exception s) {
				return HandleException<T> (s);
			}
        }

		public async Task<List<T>> AllRows<T>() where T : class
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				return await Connection.Table<T>().ToListAsync().ConfigureAwait(false);
			}
			catch (Exception s)
			{
				return HandleException<List<T>>(s);
			}
		}

		public async Task<List<T>> AllRows<T>(string sql, bool runSync = false) where T : class
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.QueryAsync<T>(sql).ConfigureAwait(false);
				List<T> data = new List<T>();
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<List<T>>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
				return data;
			}
			catch (Exception s) {
				return HandleException<List<T>> (s);
			}
		}

		public async Task<Int32> Execute(string sql, bool runSync = false) 
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.ExecuteAsync(sql).ConfigureAwait(false);
				Int32 data = 0;
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
				return data;
			}
			catch (Exception s)
			{
				return HandleException<Int32>(s);
			}

		}

		public async Task<Int32> ExecuteScalar(string sql, bool runSync = false)
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.ExecuteScalarAsync<Int32>(sql);
				Int32 data = 0;
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
				return data;
			}
			catch (Exception s)
			{
				return HandleException<Int32>(s);
			}
		}

		public async Task<List<T>> Query<T>(Expression<Func<T, bool>> query) where T : class
        {
			try
			{
				return await Connection.Table<T>().Where(query).ToListAsync().ConfigureAwait(false);
			}
			catch (Exception s) {
				HandleException(s);
				return null;
			}
        }

		public async Task<bool> Update<T>(T obj, bool runSync = false) where T : class
        {
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.UpdateAsync(obj).ConfigureAwait(false);
				Int32 data = 0;
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
	            return data > 0;
			}
			catch (Exception s) {
				return HandleException<bool> (s);
			}
        }

		public async Task<int> UpdateAll(IEnumerable obj, bool runSync = false)
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.UpdateAllAsync(obj);
				Int32 data = 0;
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
				return data;
			}
			catch (Exception s) {
				return HandleException<int> (s);
			}
		}

        public async Task<bool> Insert<T>(T obj, bool runSync = false) where T : class
        {
			try
			{
				// check if database IsInit called
				CheckDBManager();

				App.WriteToLog($"Db.Insert {DateTime.Now.ToString("T")}");
				var task = Connection.InsertAsync(obj).ConfigureAwait(false);
				Int32 data = 0;
				if (runSync)
				{
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}

                return data > 0;
			}
			catch (Exception s) {
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool> (s);
			}
        }

		public async Task<bool> InsertAll<T>(List<T> obj, bool runSync = false) where T : class
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.InsertAllAsync(obj).ConfigureAwait(false);;
				Int32 data = 0;
				if (runSync){
					
					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				} 
				return data > 0;
			}
			catch (Exception s) {
				return HandleException<bool> (s);
			}
		}

        public async Task<bool> AllowOneRow<T>(T obj, Func<T, bool> query, 
		                                       Action<T, T> ObjectUpdater, Action<List<T>, T> ErrorHandler) where T : class
        {
			try
			{
				// only one thread able to run this.  Must only allow one row to be updated or inserted
				using (var releaser = await AllowOneLockMutex.LockAsync().ConfigureAwait(false))
				{
					// check if database IsInit called
					CheckDBManager();

					// only one thread able to run this.  Must only allow one row to be updated or inserted
					var result = new TaskCompletionSource<bool>();
					await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
					{
						var table = conn.Table<T>();
						var rows = table.Count();
                        if (rows > 1)
                        {
                            ErrorHandler?.Invoke(table.ToList(), obj);
                            conn.DropTable<T>();
                            conn.CreateTable<T>();
                        }

						if (rows == 1)
						{
							var objFound = conn.Table<T>().FirstOrDefault(query);
							ObjectUpdater?.Invoke(obj, objFound);
						}
						if (rows == 1 && result != null)
						{
							conn.Update(obj);
						}
						else {
							conn.Insert(obj);
						}
					}).ContinueWith((arg) =>
					{
						if (arg.IsFaulted)
						{
							HandleException<bool>(arg.Exception);
							result.SetResult(false);
						}
						else
						{
							result.SetResult(true);
						}
					}).ConfigureAwait(false);

					await result.Task;
					return result.Task.Result;
				}

			}
			catch (Exception s) {
				HandleException(s);
			}
			return false;
        }

		public async Task<bool> AllowOneRow<T>(string insertSql, string UpdateSql) where T : class
		{
			try
			{
				// only one thread able to run this.  Must only allow one row to be updated or inserte
				using (var releaser = await AllowOneLockMutex.LockAsync().ConfigureAwait(false))
				{
					// check if database IsInit called
					CheckDBManager();

					// only one thread able to run this.  Must only allow one row to be updated or inserted
					var result = new TaskCompletionSource<bool>();
					await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
					{
						var table = conn.Table<T>();
						var rows = table.Count();
						if (rows > 1)
						{
							conn.DropTable<T>();
							conn.CreateTable<T>();
						}
						else
						{
							var count = conn.Execute(UpdateSql);
							if (count == 0)
							{
								conn.Execute(insertSql);
							}
						}

					}).ContinueWith((arg) =>
					{
						if (arg.IsFaulted)
						{
							HandleException<bool>(arg.Exception);
							result.SetResult(false);
						}
						else
						{
							result.SetResult(true);
						}
					}).ConfigureAwait(false);

					await result.Task;
					return result.Task.Result;
				}
			}
			catch (Exception s)
			{
				HandleException(s);
			}
			return false;
		}

        public async Task<bool> ReInsert<T>(T obj) where T : class
        {
			try
			{
				using (var releaser = await ReInsertLockMutex.LockAsync().ConfigureAwait(false))
				{
					// check if database IsInit called
					CheckDBManager();
					Int32 rows = -1;
					var result = new TaskCompletionSource<Int32>();
					await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
					{
						rows = conn.Update(obj);
						if (rows <= 0)
						{
							App.WriteToLog($"ReInsert not found, Insert instead");
							rows = conn.Insert(obj);
						}
						else
						{
							App.WriteToLog($"ReInsert = update");
						}
					}).ContinueWith((arg) =>
						{
							if (arg.IsFaulted)
							{
								HandleException<bool>(arg.Exception);
								result.SetResult(-1);
							}
							else
							{
								result.SetResult(rows);
							}
						}).ConfigureAwait(false);

					await result.Task;
					return result.Task.Result > 0;
				}
			}
			catch (Exception s)
			{
				return HandleException<bool>(s);
			}
        }

        public async Task<bool> ReInsert<T>(T obj, bool runSync = false) where T : class
        {
            try
            {
                using (var releaser = await ReInsertLockMutex.LockAsync().ConfigureAwait(false))
                {

                    // check if database IsInit called
                    CheckDBManager();

                    bool data = false;
                    Int32 rows = -1;
                    var result = new TaskCompletionSource<bool>();
                    var task = Connection.RunInTransactionAsync((SQLiteConnection conn) =>
                    {
                        rows = conn.Update(obj);
                        if (rows <= 0)
                        {
                            rows = conn.Insert(obj);
                        }
                    }).ContinueWith((arg) =>
                    {
                        if (arg.IsFaulted)
                        {
                            HandleException<bool>(arg.Exception);
                            result.SetResult(false);
                        }
                        else
                        {
                            result.SetResult(true);
                        }
                    }).ConfigureAwait(false);

                    if (runSync)
                    {
                        using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
                        {
                            task2.Run<bool>(async () =>
                            {
                                await result.Task;
                                return result.Task.Result;
                            }, (x) => data = x.Result);
                        }
                    }
                    else
                    {
                        await result.Task;
                        data = result.Task.Result;
                    }
                    return data;
                }
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }

        public async Task<bool> Delete<T>(Expression<Func<T, bool>> query) where T : class
        {
			try
			{
				using (var releaser = await DeleteLockMutex.LockAsync().ConfigureAwait(false))
				{
					// check if database IsInit called
					CheckDBManager();

					var result = new TaskCompletionSource<bool>();
					await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
					{
						var rows = conn.Table<T>().Where(query);
						foreach (var item in rows)
						{
							conn.Delete(item);
						}
					}).ContinueWith((arg) =>
					{
						if (arg.IsFaulted)
						{
							HandleException<bool>(arg.Exception);
							result.SetResult(false);
						}
						else
						{
							result.SetResult(true);
						}
					}).ConfigureAwait(false);

					await result.Task;
					return result.Task.Result;
				}
			}
			catch (Exception s) {
				return HandleException<bool> (s);
			}
        }

		public async Task<bool> Delete<T>(T obj, bool runSync = false) where T : class
        {
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var task = Connection.DeleteAsync(obj).ConfigureAwait(false);
				Int32 data = 0;
				if (runSync)
				{

					using (var task2 = AsyncHelpers.PlatformAsyncToSync.Wait)
					{
						task2.Run<Int32>(async () =>
						{
							return await task;
						}, (x) => data = x.Result);
					}
				}
				else
				{
					data = await task;
				}
	           	return data > 0;
			}
			catch (Exception s) {
				return HandleException<bool> (s);
			}
        }

        public async Task<bool> ClearTable<T>(bool runSync = false) where T : class
        {
            try
            {
                // check if database IsInit called
                CheckDBManager();

                return await ClearTable<T>(Connection, runSync, true);
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }

        public async Task<bool> DropTable<T>(bool runSync = false) where T : class
        {
            try
            {
                CheckDBManager();
                return await DropTable<T>(Connection, runSync, true);
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }

        public async Task<bool> RunInTransaction(Action<SQLiteConnection> transaction)
        {
            try
            {
                // check if database IsInit called
                CheckDBManager();

                return await RunInTransaction(Connection, transaction, true);
            }
            catch (Exception s)
            {
                return HandleException<bool>(s);
            }
        }

        public async Task<bool> ExecuteUpdate(string sqlStatment)
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var result = new TaskCompletionSource<bool>();
				await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
				{
					conn.Execute(sqlStatment);
				}).ContinueWith((arg) =>
				{
					if (arg.IsFaulted)
					{
						HandleException<bool>(arg.Exception);
						result.SetResult(false);
					}
					else
					{
						result.SetResult(true);
					}
				}).ConfigureAwait(false);

				await result.Task;
				return result.Task.Result;
			}
			catch (Exception s)
			{
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool>(s);
			}
		}

		public async Task<bool> ExecuteUpdate<T>(string[] fields, object[] values, string whereClause = null) where T : class
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var sql = UpdateStatementBuilder<T>(fields, values, whereClause);
				var result = new TaskCompletionSource<bool>();
				await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
				{
					conn.Execute(sql);
				}).ContinueWith((arg) =>
				{
					if (arg.IsFaulted)
					{
						HandleException<bool>(arg.Exception);
						result.SetResult(false);
					}
					else
					{
						result.SetResult(true);
					}
				}).ConfigureAwait(false);

				await result.Task;
				return result.Task.Result;
			}
			catch (Exception s)
			{
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool>(s);
			}
		}

		public async Task<bool> ExecuteUpdate(string table, string[] fields, object[] values, string whereClause = null)
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var sql = UpdateStatementBuilder(table, fields, values, whereClause);
				var result = new TaskCompletionSource<bool>();
				await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
				{
					conn.Execute(sql);
				}).ContinueWith((arg) =>
				{
					if (arg.IsFaulted)
					{
						HandleException<bool>(arg.Exception);
						result.SetResult(false);
					}
					else
					{
						result.SetResult(true);
					}
				}).ConfigureAwait(false);

				await result.Task;
				return result.Task.Result;
			}
			catch (Exception s)
			{
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool>(s);
			}
		}

		public async Task<bool> ExecuteInsert(string sqlStatment)
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var result = new TaskCompletionSource<bool>();
				await Connection.RunInTransactionAsync((SQLiteConnection conn) =>
				{
					conn.Execute(sqlStatment);
				}).ContinueWith((arg) =>
				{
					if (arg.IsFaulted)
					{
						HandleException<bool>(arg.Exception);
						result.SetResult(false);
					}
					else
					{
						result.SetResult(true);
					}
				}).ConfigureAwait(false);

				await result.Task;
				return result.Task.Result;
			}
			catch (Exception s)
			{
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool>(s);
			}
		}

		public async Task<bool> ExecuteInsert<T>(string[] fields, object[] values) where T : class
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var sql = InsertStatementBuilder<T>(fields, values);
				var result = new TaskCompletionSource<bool>();
				await Connection.RunInTransactionAsync((SQLiteConnection obj) =>
				{
					obj.Execute(sql);
				}).ContinueWith((arg) =>
				{
					if (arg.IsFaulted)
					{
						HandleException<bool>(arg.Exception);
						result.SetResult(false);
					}
					else
					{
						result.SetResult(true);
					}
				}).ConfigureAwait(false);

				await result.Task;
				return result.Task.Result;
			}
			catch (Exception s)
			{
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool>(s);
			}
		}

		public async Task<bool> ExecuteInsert(string table, string[] fields, object[] values)
		{
			try
			{
				// check if database IsInit called
				CheckDBManager();

				var sql = InsertStatementBuilder(table, fields, values);
				var result = new TaskCompletionSource<bool>();
				await Connection.RunInTransactionAsync((SQLiteConnection obj) =>
				{
					obj.Execute(sql);
				}).ContinueWith((arg) =>
				{
					if (arg.IsFaulted)
					{
						HandleException<bool>(arg.Exception);
						result.SetResult(false);
					}
					else
					{
						result.SetResult(true);
					}
				}).ConfigureAwait(false);

				await result.Task;
				return result.Task.Result;
			}
			catch (Exception s)
			{
				App.WriteToLog($"Db.Insert Exception {DateTime.Now.ToString("T")}");
				return HandleException<bool>(s);
			}
		}


        #endregion

        #region "builders"

        public string InsertStatementBuilder<T>(string[] fields, object[] values) where T : class
		{
			return InsertStatementBuilder(typeof(T).Name, fields, values);
		}

		public string InsertStatementBuilder<T>(string field, object value) where T : class
		{
			return InsertStatementBuilder(typeof(T).Name, new[] { field }, new[] { value });
		}

		public string InsertStatementBuilder(string table, string field, object value)
		{
			return InsertStatementBuilder(table, new[] { field }, new[] { value });
		}

		public string InsertStatementBuilder(string table, string[] fields, object[] values)
		{
			if (fields == null || (fields.Length != values.Length))
				throw new ArgumentException("fields can not be null and fields and values must have the same length");

			if (fields.Length <= 1)
				throw new ArgumentException("fields must contain at least one valid fieldname");

			//Insert into CommentSubjects ([Subject]) values('abc1')
			var valueSql = " ";
			var fieldSql = "";
			for (int i = 0; i < fields.Length; i++)
			{
				var f = fields[i];
				var v = values[i];

				fieldSql += $"[{f}], ";

				if (v is string || v is char)
				{
					valueSql += $"'{v}', ";
				}
				else if (v is DateTime)
				{
					valueSql += $"'{((DateTime)v).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}', ";
				}
				else
				{
					valueSql += $"{v}, ";
				}
			}
			fieldSql = fieldSql.TrimEnd(new char[] { ' ', ',' }) + " ";
			valueSql = valueSql.TrimEnd(new char[] { ' ', ',' }) + " ";

			return $@"insert into {table} ({fieldSql}) values ({valueSql});";
		}

		public string UpdateStatementBuilder<T>(string[] fields, object[] values, string whereClause = null) where T : class
		{
			return UpdateStatementBuilder(typeof(T).Name, fields, values, whereClause);
		}

		public string UpdateStatementBuilder<T>(string field, object value, string whereClause = null) where T : class
		{
			return UpdateStatementBuilder(typeof(T).Name, new[] { field }, new[] { value }, whereClause);
		}

		public string UpdateStatementBuilder(string table, string field, object value, string whereClause = null)
		{
			return UpdateStatementBuilder(table, new[] { field }, new[] { value }, whereClause);
		}

		public string UpdateStatementBuilder(string table, string[] fields, object[] values, string whereClause = null)
		{
			if (fields == null || (fields.Length != values.Length))
				throw new ArgumentException("fields can not be null and fields and values must have the same length");

			var sql = "set ";
			for (int i = 0; i < fields.Length; i++)
			{
				var f = fields[i];
				var v = values[i];

				if (v is string || v is char)
				{
					sql += $"{f} ='{v}', ";
				}
				else if (v is DateTime)
				{
					sql += $"{f} = '{((DateTime)v).ToUniversalTime().ToString("yyyy-MM-dd HH:mm:ss")}', ";
				}
				else
				{
					sql += $"{f} = {v}, ";
				}
			}
			sql = sql.TrimEnd(new char[] { ' ', ',' }) + " ";

			return $@"update {table} {sql} {whereClause}; ";
		}

		#endregion

		public void Reset()
		{
			this.IsInit = false;
		}

		public void ResetDatabaseState()
		{
			Reset();
			this.IsInit = false;
			Init();
		}

		public void Init()
        {
            try
            {
				if (this.IsInit == false)
				{
					this.IsInit = true;	
	                try
	                {
						App.WriteToLog($"In Init 1 {DateTime.Now.ToString("T")}");
						if (TableData == null)
						{
							TableData = Xamarin.Forms.DependencyService.Get<IResourceManager>().FindNamespace(TableNameSpace.ToArray()).ToList();
						}

						// set the IsInit flag
						IsInit = AsyncHelpers.RunSync<bool>(async() =>
                        {
							try
							{
								App.WriteToLog($"In Init Create {DateTime.Now.ToString("T")}");
								await Connection.CreateTablesAsync(TableData.ToArray());
								return true;
							}
							catch(Exception ex)
							{
								App.WriteToLog($"In Init Exception {ex.Message} {DateTime.Now.ToString("T")}");
							   HandleException(ex);
							   return false;
							}
                        });
	                }
	                catch (Exception ex)
	                {
						this.IsInit = false;
						App.WriteToLog($"In Init Exception outside {ex.Message} {DateTime.Now.ToString("T")}");
						HandleException(ex);
	                }
				}
            }
            catch (Exception ex)
            {
				this.IsInit = false;
				HandleException(ex);
            }
        }
    }
}

