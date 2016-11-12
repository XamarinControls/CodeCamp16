using System;
using System.Threading.Tasks;
using Xamarin.Forms;
using System.Reflection;
using System.Linq.Expressions;
using System.Collections.Generic;
using sqlitetest1.Interfaces;
using Plugin.Messaging;
using System.Linq;

namespace sqlitetest1
{
	public class DBTest : ContentPage
	{
		public DBTest()
		{
			this.Title = "DBTest";

			var button2 = new Button
			{
				Text = "Copy DB",
				Command = new Command(async(obj) =>
				{
					App.WriteToLog($"Before CopyDB {DateTime.Now.ToString("T")}");
					var source = DependencyService.Get<IDirectory>(DependencyFetchTarget.NewInstance);
					source.RootDirectory = Folders.MyDocuments;
					source.SetPath("");
					var path = source.FullName;
					var exists = source.Exists;

					App.WriteToLog($"source path: {source.FullName} {DateTime.Now.ToString("T")}");

					var dest = DependencyService.Get<IDirectory>(DependencyFetchTarget.NewInstance);
					if (Device.OS == TargetPlatform.Android)
					{
						dest.RootDirectory = Folders.External;
					}
					else
					{
						dest.RootDirectory = Folders.MyDocuments;
					}
					dest.SetPath("pseaDB");

					// delete destinatio
					dest.Delete(true);

					// create
					dest.Create();

					var exists2 = dest.Exists;
					if (exists2 == false) dest.Create();

					App.WriteToLog($"dest path: {dest.FullName} {DateTime.Now.ToString("T")}");

					var paths = source.CopyTo(dest.RootDirectory, "pseaDB");
					App.WriteToLog($"After CopyDB {DateTime.Now.ToString("T")}");

					var email = Plugin.Messaging.CrossMessaging.Current.EmailMessenger;
					var builder = new EmailMessageBuilder();
					builder.To("amccormack@psea.org");

					await System.Threading.Tasks.Task.Delay(800);
					builder.WithAttachment(paths.FirstOrDefault(x => x.IndexOf(App.dbManager.FileName, StringComparison.Ordinal) != -1), "application/x-sqlite3");
					builder.Subject("database");
					builder.Body("sqlite database");
					email.SendEmail(builder.Build());

				})
			};

			var button3 = new Button
			{
				Text = "Write Rows",
				Command = new Command(async() =>
				{
					await App.dbManager.ClearTable<DB.Config>();
					var ran = new System.Random();
					var tempID = 10;
					for (var i = 0; i < 1000; i++)
					{
						//await Task.Delay(myID);
						await Task.Run(async () =>
						{
							App.WriteToLog($"Create Row {i} {DateTime.Now.ToString("T")}");
							var config = new DB.Config();
							config.LocalID = ran.Next(100, 300) + tempID++;
							config.MobileID = i;
							config.PeopleID = 10000 + tempID;
							config.UserID = $"{tempID}";

							await App.dbManager.Insert(config).ContinueWith((arg) =>
							{
								if (arg.IsFaulted)
								{
									System.Diagnostics.Debug.WriteLine(arg.Exception.Message);
								}
							});
						});
					}

					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert(null, "Finished Write Rows", "OK");
					});
				})
			};

			var button4 = new Button
			{
				Text = "Stress Write Rows",
				Command = new Command(async () =>
				{
					await App.dbManager.ClearTable<DB.Config>();

					watch = new System.Diagnostics.Stopwatch();
					watch.Start();
					App.StopOutput = true;
					FireTimer();
					var ran = new System.Random();
					var tempID = 10;
					for (var i = 0; i < 1000; i++)
					{
						//await Task.Delay(myID);
						await Task.Run(async () =>
						{
							var config = new DB.Config();
							config.LocalID = ran.Next(100, 300) + tempID++;
							config.MobileID = i;
							config.PeopleID = 10000 + tempID;
							config.UserID = $"{tempID}";

							await App.dbManager.Insert(config).ContinueWith((arg) =>
							{
								if (arg.IsFaulted)
								{
									System.Diagnostics.Debug.WriteLine(arg.Exception.Message);
								}
							});
						});
					}

					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert(null, "Finished Stress Write Rows", "OK");
					});
				})
			};

			var button5 = new Button
			{
				Text = "Stress Write Trans Rows",
				Command = new Command(async () =>
				{
					await App.dbManager.ClearTable<DB.Config>();

					watch = new System.Diagnostics.Stopwatch();
					watch.Start();

					App.StopOutput = true;
					FireTimer();

					var ran = new System.Random();
					var tempID = 10;
					Action<SQLite.Net.SQLiteConnection> transaction = (conn) =>
					{
						for (var i = 0; i < 1000; i++)
						{
							var config = new DB.Config();
							config.LocalID = ran.Next(100, 300) + tempID++;
							config.MobileID = i;
							config.PeopleID = 10000 + tempID;
							config.UserID = $"{tempID}";
							conn.Insert(config);
						}
					};

					await App.dbManager.RunInTransaction(transaction);
					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert(null, "Finished Stress Write Trans Rows", "OK");
					});
				})
			};

			var button6 = new Button
			{
				Text = "Test Sql",
				Command = new Command(async() =>
				{

					await App.dbManager.ClearTable<DB.MyRow>();

					var sql = App.dbManager.UpdateStatementBuilder<DB.Config>(Helpers.NameOf(() => new DB.MyRow().MobileID), 1, "");
					App.WriteToLog(sql);

					sql = App.dbManager.UpdateStatementBuilder<DB.Config>(Helpers.NameOf(() => new DB.MyRow().MobileID), 1.3990, "");
					App.WriteToLog(sql);

					sql = App.dbManager.UpdateStatementBuilder<DB.Config>(Helpers.NameOf(() => new DB.MyRow().MobileID), DateTime.Now, "");
					App.WriteToLog(sql);


					sql = App.dbManager.UpdateStatementBuilder<DB.Config>(new[] {
						Helpers.NameOf(() => new DB.MyRow().MobileID),
						Helpers.NameOf( () => new DB.MyRow().Username)
					}, new object[] { 1, "mccorma" }, "");
					App.WriteToLog(sql);

					sql = App.dbManager.UpdateStatementBuilder(nameof(DB.Config), Helpers.NameOf(() => new DB.MyRow().MobileID), DateTime.Now, "");
					App.WriteToLog(sql);

					var myRow = new DB.Config();
					sql = App.dbManager.InsertStatementBuilder<DB.Config>(
						new[] {
							Helpers.NameOf(() => new DB.MyRow().MobileID),
							Helpers.NameOf(() => myRow.LocalID),
							Helpers.NameOf(() => myRow.PeopleID)
					}, new object[] { 1, 2, 4 });
					App.WriteToLog(sql);



					// test single insert
					await App.dbManager.ExecuteInsert<DB.MyRow>(new[] { Helpers.NameOf(() => new DB.MyRow().MobileID), Helpers.NameOf(() => new DB.MyRow().Username) },
										new object[] { 999, 1234 });

					// multiple insert
					var sql1 = App.dbManager.InsertStatementBuilder<DB.MyRow>(new[] { Helpers.NameOf(() => new DB.MyRow().MobileID), Helpers.NameOf(() => new DB.MyRow().Username) },
																			  new object[] { 999, "myuser1" });
					var sql2 = App.dbManager.InsertStatementBuilder<DB.MyRow>(new[] { Helpers.NameOf(() => new DB.MyRow().MobileID), Helpers.NameOf(() => new DB.MyRow().Username) },
																		  new object[] { 009, "myuser2" });

					await App.dbManager.ExecuteInsert($"{sql1}{sql2}");

					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert(null, "Finished Test Sql", "OK");
					});

				})
			};

			var button7 = new Button
			{
				Text = "InsertAll v Transaction",
				Command = new Command(async () =>
				{
					await App.dbManager.ClearTable<DB.Config>();

					watch = new System.Diagnostics.Stopwatch();
					watch.Start();

					var ran = new System.Random();
					Action<SQLite.Net.SQLiteConnection> transaction = (conn) =>
					{
						for (var i = 2000; i < 3000; i++)
						{
							var config = new DB.Config();
							config.LocalID = ran.Next(100, 300) + i;
							config.MobileID = i;
							config.PeopleID = i;
							config.UserID = $"{i}";

							conn.Insert(config);
						}
					};

					await App.dbManager.RunInTransaction(transaction);
					var el = watch.Elapsed;
					System.Diagnostics.Debug.WriteLine($"Time for 1000 rows {el.Minutes}:{el.Seconds}:{el.Milliseconds}");

					var items = new List<DB.Config>();
					for (var i = 3000; i < 4000; i++)
					{
						var config = new DB.Config();
						config.LocalID = ran.Next(100, 300) + i;
						config.MobileID = i;
						config.PeopleID = i;
						config.UserID = $"{i}";

						items.Add(config);
					}
					watch.Stop();
					watch.Start();

					await App.dbManager.InsertAll(items);
					el = watch.Elapsed;
					System.Diagnostics.Debug.WriteLine($"Time for 1000 rows {el.Minutes}:{el.Seconds}:{el.Milliseconds}");

					Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert(null, $"Time for 1000 rows {el.Minutes}:{el.Seconds}:{el.Milliseconds}", "OK");
					});
				})
			};

			var button8 = new Button
			{
				Text = "OneRow<T> Test",
				Command = new Command(async () =>
				{
					var result = await DisplayAlert(null, "Clear OneRow before test?", "YES", "NO");
					if (result)
					{
						await App.dbManager.ClearTable<DB.OneRow>();
					}

					//var row3 = new DB.OneRow();
					//row3.FirstName = "Andrew";
					//row3.LastName = "McCormack";
					//row3.ID = 10;
					//await App.dbManager.Insert(row3);

					//var row2 = new DB.OneRow();
					//row2.FirstName = "Andrew";
					//row2.LastName = "McCormack";
					//row2.ID = 11;
					//await App.dbManager.Insert(row2);

					//var result1 = await App.dbManager.Execute("insert into OneRow (ID, FirstName, LastName, DOB, Username) values(0, 'drew', 'mcc', 10, 'user1')");
					//App.WriteToLog($"Insert OneRow result:= {result1}");
					///var result = await App.dbManager.Execute("update OneRow set FirstName='test'");
					//App.WriteToLog($"update OneRow result:= {result}");

					//var after = 10;

					var row = new DB.OneRow();
					row.FirstName = "Andrew";
					row.LastName = "McCormack";
					row.DOB = new System.Random().Next(100, 500);
						await App.dbManager.AllowOneRow<DB.OneRow>(row, (x) => x.ID == 0, (arg1, arg2) =>
						{
							if (arg2 != null)
							{
									// found row;
									arg1.ID = arg2.ID;
									App.WriteToLog("found row, assigning primary key");
							}

						}, (List<DB.OneRow> arg1, DB.OneRow arg2) => {
							// Error Handler for more than one row
							App.WriteToLog("more than one row found, will be purged");
						});


						Device.BeginInvokeOnMainThread(() =>
						{
							DisplayAlert(null, $"Finished OneRow<T> Test", "OK");
						});

				})
			};


			var button9 = new Button
			{
				Text = "OneRow sql Test",
				Command = new Command(async () =>
				{
					var result = await DisplayAlert(null, "Clear OneRow before test?", "YES", "NO");
					if (result)
					{
						await App.dbManager.ClearTable<DB.OneRow>();
					}

					//var row3 = new DB.OneRow();
					//row3.FirstName = "Andrew";
					//row3.LastName = "McCormack";
					//row3.ID = 10;
					//await App.dbManager.Insert(row3);

					//var row2 = new DB.OneRow();
					//row2.FirstName = "Andrew";
					//row2.LastName = "McCormack";
					//row2.ID = 11;
					//await App.dbManager.Insert(row2);

					//var result1 = await App.dbManager.Execute("insert into OneRow (ID, FirstName, LastName, DOB, Username) values(0, 'drew', 'mcc', 10, 'user1')");
					//App.WriteToLog($"Insert OneRow result:= {result1}");
					///var result = await App.dbManager.Execute("update OneRow set FirstName='test'");
					//App.WriteToLog($"update OneRow result:= {result}");

					//var after = 10;

					var row = new DB.OneRow();
					var firstName = "Andrew";
					var LastName = "McCormack";
					var DOB = new System.Random().Next(100, 500);

					var updateSql = App.dbManager.UpdateStatementBuilder<DB.OneRow>(
						new[] { Helpers.NameOf(() => row.FirstName), Helpers.NameOf(() => row.LastName), Helpers.NameOf(() => row.DOB) },
						new object[] { firstName, LastName, DOB }, 
						"where FirstName='Andrew'"
					);

					var insertSql = App.dbManager.InsertStatementBuilder<DB.OneRow>(
						new[] { Helpers.NameOf( ()=> row.ID), Helpers.NameOf(() => row.FirstName), Helpers.NameOf(() => row.LastName), Helpers.NameOf(() => row.DOB) },
						new object[] { 10, firstName, LastName, DOB }
					);
					
						var ok = await App.dbManager.AllowOneRow<DB.OneRow>(insertSql, updateSql);
					App.WriteToLog($"OneRow sql Test result {ok}");
						Device.BeginInvokeOnMainThread(() =>
					{
						DisplayAlert(null, $"OneRow sql Test", "OK");
					});
				})
			};

			Content = new ScrollView
			{
				Content = new StackLayout
				{
					Children = {
						new Button
						{
							Text = "Test UI"
						},
						button2,
						button3,
						button4,
						button5,
						button6,
						button7,
						button8,
						button9
					}
				}
			};
		}



		private System.Diagnostics.Stopwatch watch;

		private void FireTimer()
		{
			Device.StartTimer(TimeSpan.FromSeconds(1), () =>
			{
				Task.Run(async () =>
				{
					var count = 0;
					while (count < 1000)
					{
						count = await App.dbManager.RowCount<DB.Config>(false);
						await Task.Delay(500);
					}

					App.StopOutput = false;
					watch.Stop();
					var el = watch.Elapsed;
					System.Diagnostics.Debug.WriteLine($"Time for 1000 rows {el.Minutes}:{el.Seconds}:{el.Milliseconds}");
					Device.BeginInvokeOnMainThread(async () =>
					{
						await DisplayAlert(null, "1000 records", "OK");
					});
				});
				return false;
			});
		}
	}
}

