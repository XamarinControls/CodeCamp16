using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace sqlitetest1
{
	public class UpdateTestPage : ContentPage
	{
		private Entry entry1, entry2, entry3;
		private bool RunBackground = false;

		private Int32 primaryKey;
		public UpdateTestPage()
		{
			App.StopOutput = false;


			this.Title = "Update Test";

			var label2 = new Label { Text = "No Background" };

			var button0 = new Button
			{
				Text = "Clear MyRow",
				Command = new Command(async(obj) =>
				{
					await App.dbManager.ClearTable<DB.MyRow>();
					this.primaryKey = -1;
				})
			};

			var button1 = new Button
			{
				Text = "Get Row",
				Command = new Command(async (obj) =>
				{
					var row = await App.dbManager.FirstRow<DB.MyRow>();
					if (row != null)
					{
						entry1.Text = row.MobileID;
						entry2.Text = row.TestAddress;
						entry3.Text = row.Username;

						primaryKey = row.ID;
					}
					else
					{
						entry1.Text = "";
						entry2.Text = "";
						entry3.Text = "";
						primaryKey = -1;
					}
				})
			};

			entry1 = new Entry();
			entry2 = new Entry();
			entry3 = new Entry();

			var button2 = new Button
			{
				Text = "Background Update",
				Command = new Command((obj) =>
				{
					this.RunBackground = true;
					FireTimer();
					label2.Text = "backgrounding";
				})
			};

			var button3 = new Button
			{
				Text = "Stop Background",
				Command = new Command((obj) =>
				{
					this.RunBackground = false;
					label2.Text = "No Background";
				})
			};

			Content = new ScrollView {
				Content = new StackLayout
				{
					Children = {
						label2,
						new Label { Text = "MobileID" },
						entry1,
						new Label { Text = "Address" },
						entry2,
						new Label { Text = "username" },
						entry3,
						new Button
						{
							Text = "Save",
							Command = new Command(async () => {
								//var afterFetch = true;
								//var item = new DB.MyRow
								//{
								//	MobileID = entry1.Text,
								//	TestAddress = entry2.Text,
								//	Username = entry3.Text
								//};

								//if (primaryKey != -1)
								//{
								//	afterFetch = false;
								//	item.ID = primaryKey;
								//}

								//await App.dbManager.ReInsert(item);
								//if (afterFetch)
								//{
									
								//	var row = await App.dbManager.FirstRow<DB.MyRow>();
								//	this.primaryKey = row.ID;
								//	App.WriteToLog($"after fetch primary key {row.ID}");
								//}
								//App.WriteToLog($"primary key {this.primaryKey}");


									await App.dbManager.RunInTransaction((obj) => {
										var row = obj.Table<DB.MyRow>().FirstOrDefault();
										if (row != null)
										{
											row.MobileID = entry1.Text;
											row.TestAddress = entry2.Text;
											row.Username = entry3.Text;
											obj.Update(row);
										}
										else
										{
											var item = new DB.MyRow
											{
												MobileID = entry1.Text,
												TestAddress = entry2.Text,
												Username = entry3.Text
											};
											this.primaryKey = obj.Insert(item);
										}
									});

								App.WriteToLog($"primary key {this.primaryKey}");
							})
						},
						button0, button1, button2, button3
					}
				}
			};
		}

		private void FireTimer()
		{
			Device.StartTimer(TimeSpan.FromSeconds(1), () =>
			{
				Task.Run(async () =>
				{
					while (this.RunBackground)
					{
						var row = await App.dbManager.FirstRow<DB.MyRow>();
						if (row != null)
						{
							await App.dbManager.ExecuteUpdate<DB.MyRow>(new string[] { Helpers.NameOf(() => new DB.MyRow().Username) }, new[] { System.Guid.NewGuid().ToString() });
							App.WriteToLog($"Update running - {DateTime.Now.ToString("T")}");
						}
						await Task.Delay(800);
					}
					App.WriteToLog("Background completed");

				});
				return false;
			});
		}
	}
}

