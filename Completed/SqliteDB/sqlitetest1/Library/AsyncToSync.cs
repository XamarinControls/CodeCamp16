using System;
using System.Threading.Tasks;
using System.Threading;
using System.Collections.Generic;
using sqlitetest1.Interfaces;

/*
 			this.btnRunTask1 = new Button { Text = "Run Task no Wait" };
			this.btnRunTask2 = new Button { Text = "Run Task (result) Wait (10000)" };
			this.btnRunTask3 = new Button { Text = "Run Task Wait (5000)" };
			this.btnRunTask4 = new Button { Text = "Run Task SYNC?" };

			this.btnRunTask1.Clicked += (object sender, EventArgs e) => {
				MyTask = DoMyTask();
				DisplayAlert("Done", "Task Done", "OK");
			};

			this.btnRunTask2.Clicked += (object sender, EventArgs e) => {
				var result = AsyncToSyncHelpers.RunSync<Int32>(DoMyTask);
				DisplayAlert("Done", "Task Done 10000 delay (" + result + ")", "OK");
			};

			this.btnRunTask3.Clicked += (object sender, EventArgs e) => {
				AsyncToSyncHelpers.RunSync(DoMyTask2);
				DisplayAlert("Done", "Task Done 5000 delay", "OK");
			};

			this.btnRunTask4.Clicked += (object sender, EventArgs e) => {
				DoMyTask2().RunSynchronously();
				DisplayAlert("Done", "Task Done 5000 delay", "OK");
			};

			var people = AsyncToSyncHelpers.RunSync(() => new CommonService ().GetPeople (peopleID));
			this.UserMessage = people.Item.ShortName;
*/
namespace sqlitetest1.Library
{
    /// <summary>
	/// customerList = AsyncToSync.RunSync<List<Customer>>(() => GetCustomers());
	/// </summary>
	public static class AsyncHelpers
	{
		/// <summary>
		/// Execute async to async using Platform code
		/// </summary>
		/// <remarks>
		/// <para>Must be declared in Droid, iOS and WinPhone Project</para>
		/// [assembly: Xamarin.Forms.Dependency (typeof ([namespace].[ClassFile]))]
		/// </remarks>
		/// <value>The device async to sync.</value>
		public static IAsyncToSync PlatformAsyncToSync => Xamarin.Forms.DependencyService.Get<IAsyncToSync>(Xamarin.Forms.DependencyFetchTarget.NewInstance);

		/// <summary>
		/// Execute's an async Task<T> method which has a void return value synchronously
		/// </summary>
		/// <param name="task">Task<T> method to execute</param>
		public static void RunSync(Func<System.Threading.Tasks.Task> task, Int32 millisecondDelay)
		{
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			synch.Post(async _ =>
				{
					try
					{
						await task().ContinueWith(async(t) => {
							await Task.Delay(millisecondDelay);
						});
					}
					catch (Exception e)
					{
						synch.InnerException = e;
						throw;
					}
					finally
					{
						synch.EndMessageLoop();
					}
				}, null);
			synch.BeginMessageLoop();

			SynchronizationContext.SetSynchronizationContext(oldContext);
		}

		/// <summary>
		/// Execute's an async Task<T> method which has a void return value synchronously
		/// </summary>
		/// <param name="task">Task<T> method to execute</param>
		public static void RunSync(Func<System.Threading.Tasks.Task> task)
		{
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			synch.Post(async _ =>
				{
					try
					{
						await task();
					}
					catch (Exception e)
					{
						synch.InnerException = e;
						throw;
					}
					finally
					{
						synch.EndMessageLoop();
					}
				}, null);
			synch.BeginMessageLoop();

			SynchronizationContext.SetSynchronizationContext(oldContext);
		}

		/// <summary>
		/// Execute's an async Task<T> method which has a T return type synchronously
		/// </summary>
		/// <typeparam name="T">Return Type</typeparam>
		/// <param name="task">Task<T> method to execute</param>
		/// <returns></returns>
		public static T RunSync<T>(Func<Task<T>> task)
		{
			var oldContext = SynchronizationContext.Current;
			var synch = new ExclusiveSynchronizationContext();
			SynchronizationContext.SetSynchronizationContext(synch);
			T ret = default(T);
			synch.Post(async _ =>
				{
					try
					{
						ret = await task();
					}
					catch (Exception e)
					{
						synch.InnerException = e;
						throw;
					}
					finally
					{
						synch.EndMessageLoop();
					}
				}, null);
			synch.BeginMessageLoop();
			SynchronizationContext.SetSynchronizationContext(oldContext);
			return ret;
		}

		/// <summary>
		/// Run and forget
		/// </summary>
		/// <param name="task">Task.</param>
		public static void Run(Action task)
		{
			RunSync (() => {
				var t = Task.Run( () => task.Invoke());
				return t;
			});
		}

		private class ExclusiveSynchronizationContext : SynchronizationContext
		{
			private bool done;
			public Exception InnerException { get; set; }
			readonly AutoResetEvent workItemsWaiting = new AutoResetEvent(false);
			readonly Queue<Tuple<SendOrPostCallback, object>> items =
				new Queue<Tuple<SendOrPostCallback, object>>();

			public override void Send(SendOrPostCallback d, object state)
			{
				throw new NotSupportedException("We cannot send to our same thread");
			}

			public override void Post(SendOrPostCallback d, object state)
			{
				lock (items)
				{
					items.Enqueue(Tuple.Create(d, state));
				}
				workItemsWaiting.Set();
			}

			public void EndMessageLoop()
			{
				Post(_ => done = true, null);
			}

			public void BeginMessageLoop()
			{
				while (!done)
				{
					Tuple<SendOrPostCallback, object> task = null;
					lock (items)
					{
						if (items.Count > 0)
						{
							task = items.Dequeue();
						}
					}
					if (task != null)
					{
						task.Item1(task.Item2);
						if (InnerException != null) // the method threw an exeption
						{
							throw new AggregateException("AsyncHelpers.Run method threw an exception.", InnerException);
						}
					}
					else
					{
						workItemsWaiting.WaitOne();
					}
				}
			}

			public override SynchronizationContext CreateCopy()
			{
				return this;
			}
		}
	}
}

