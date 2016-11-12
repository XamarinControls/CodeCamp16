using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace sqlitetest1.droid.Library
{
	using EventTask = Tuple<System.Threading.SendOrPostCallback, object>;
	using EventQueue = ConcurrentQueue<Tuple<System.Threading.SendOrPostCallback, object>>;
	using System.Threading;
	using sqlitetest1.Interfaces;

	/// <summary>
	/// A Helper class to run Asynchronous functions from synchronous ones
	/// </summary>
	public class AsyncHelper : IAsyncToSync
    {
        /// <summary>
        /// A class to bridge synchronous asynchronous methods/// 
        /// </summary>
		/// <example>
		///  using (var A = AsyncHelper.Wait)
    	/// {
        /// A.Run(AsyncString(), res => string1 = res);
        /// A.Run(AsyncString(), res => string2 = res);
    	/// }
		/// </example>
        public class AsyncBridge : IAsyncDevice
        {
            private ExclusiveSynchronizationContext CurrentContext;
            private SynchronizationContext OldContext;
            private int TaskCount;

            /// <summary>
            /// Constructs the AsyncBridge by capturing the current
            /// SynchronizationContext and replacing it with a new
            /// ExclusiveSynchronizationContext.
            /// </summary>
            internal AsyncBridge()
            {
                OldContext = SynchronizationContext.Current;
                CurrentContext =
                    new ExclusiveSynchronizationContext(OldContext);
                SynchronizationContext
                    .SetSynchronizationContext(CurrentContext);
            }

            /// <summary>
            /// Execute's an async task with a void return type
            /// from a synchronous context
            /// </summary>
            /// <param name="task">Task to execute</param>
            /// <param name="callback">Optional callback</param>
            public void Run(Task task, Action<Task> callback = null)
            {
                CurrentContext.Post(async _ =>
                {
                    try
                    {
                        Increment();
                        await task;

                        if (null != callback)
                        {
                            callback(task);
                        }
                    }
                    catch (Exception e)
                    {
                        CurrentContext.InnerException = e;
                    }
                    finally
                    {
                        Decrement();
                    }
                }, null);
            }

			/// <summary>
			/// Execute's an async task with a void return type
			/// from a synchronous context
			/// </summary>
			/// <param name="task">Function that returns a Task</param>
			/// <param name="callback">Optional callback</param>
			public void Run(Func<Task> task, Action<Task> callback = null)
			{
				CurrentContext.Post(async _ =>
				{
					try
					{
						Increment();
						await task.Invoke();

						if (null != callback)
						{
							callback(task.Invoke());
						}
					}
					catch (Exception e)
					{
						CurrentContext.InnerException = e;
					}
					finally
					{
						Decrement();
					}
				}, null);
			}

            /// <summary>
            /// Execute's an async task with a T return type
            /// from a synchronous context
            /// </summary>
            /// <typeparam name="T">The type of the task</typeparam>
            /// <param name="task">Task to execute</param>
            /// <param name="callback">Optional callback</param>
            public void Run<T>(Task<T> task, Action<Task<T>> callback = null)
            {
                if (null != callback)
                {
                    Run((Task)task, (finishedTask) =>
                        callback((Task<T>)finishedTask));
                }
                else
                {
                    Run((Task)task);
                }
            }

			/// <summary>
			/// Execute's an async task with a T return type
			/// from a synchronous context
			/// </summary>
			/// <typeparam name="T">The type of the task</typeparam>
			/// <param name="task">Function that returns Task&lt;T&gt; to execute</param>
			/// <param name="callback">Optional callback</param>
			public void Run<T>(Func<Task<T>> task, Action<Task<T>> callback = null)
			{
				if (null != callback)
				{
					Run((Task)task.Invoke(), (finishedTask) =>
						callback((Task<T>)finishedTask));
				}
				else
				{
					Run((Task)task.Invoke());
				}
			}

            /// <summary>
            /// Execute's an async task with a T return type
            /// from a synchronous context
            /// </summary>
            /// <typeparam name="T">The type of the task</typeparam>
            /// <param name="task">Task to execute</param>
            /// <param name="callback">
            /// The callback function that uses the result of the task
            /// </param>
            public void Run<T>(Task<T> task, Action<T> callback)
            {
                Run(task, (t) => callback(t.Result));
            }

            private void Increment()
            {
                Interlocked.Increment(ref TaskCount);
            }

            private void Decrement()
            {
                Interlocked.Decrement(ref TaskCount);
                if (TaskCount == 0)
                {
                    CurrentContext.EndMessageLoop();
                }
            }

            /// <summary>
            /// Disposes the object
            /// </summary>
            public void Dispose()
            {
                try
                {
                    CurrentContext.BeginMessageLoop();
                }
                catch (Exception e)
                {
                    throw e;
                }
                finally
                {
                    SynchronizationContext
                        .SetSynchronizationContext(OldContext);
                }
            }
        }

        /// <summary>
        /// Creates a new AsyncBridge. This should always be used in
        /// conjunction with the using statement, to ensure it is disposed
        /// </summary>
        public IAsyncDevice Wait
        {
            get { return new AsyncBridge(); }
        }

		/// <summary>Runs the specified asynchronous function.</summary>
		/// <param name="func">The asynchronous function to execute.</param>
		public void Run(Func<Task> func)
		{
			if (func == null) throw new ArgumentNullException("func");

			var prevCtx = SynchronizationContext.Current;
			try
			{
				// Establish the new context
				var syncCtx = new SingleThreadSynchronizationContext();
				SynchronizationContext.SetSynchronizationContext(syncCtx);

				// Invoke the function and alert the context to when it completes
				var t = func();
				if (t == null) throw new InvalidOperationException("No task provided.");
				t.ContinueWith(delegate { syncCtx.Complete(); }, TaskScheduler.Default);

				// Pump continuations and propagate any exceptions
				syncCtx.RunOnCurrentThread();
				t.GetAwaiter().GetResult();
			}
			finally { SynchronizationContext.SetSynchronizationContext(prevCtx); }
		}

        /// <summary>
        /// Runs a task with the "Fire and Forget" pattern using Task.Run,
        /// and unwraps and handles exceptions
        /// </summary>
        /// <param name="task">A function that returns the task to run</param>
        /// <param name="handle">Error handling action, null by default</param>
        public void FireAndForget(
            Func<Task> task,
            Action<Exception> handle = null)
        {

            Task.Run(
            () =>
            {
                ((Func<Task>)(async () =>
                {
                    try
                    {
                        await task();
                    }
                    catch (Exception e)
                    {
                        if (null != handle)
                        {
                            handle(e);
                        }
                    }
                }))();
            });
        }

		/// <summary>Provides a SynchronizationContext that's single-threaded.</summary>
		private sealed class SingleThreadSynchronizationContext : SynchronizationContext
		{
			/// <summary>The queue of work items.</summary>
			private readonly BlockingCollection<KeyValuePair<SendOrPostCallback, object>> m_queue =
				new BlockingCollection<KeyValuePair<SendOrPostCallback, object>>();
			/// <summary>The processing thread.</summary>
			private readonly Thread m_thread = Thread.CurrentThread;

			/// <summary>Dispatches an asynchronous message to the synchronization context.</summary>
			/// <param name="d">The System.Threading.SendOrPostCallback delegate to call.</param>
			/// <param name="state">The object passed to the delegate.</param>
			public override void Post(SendOrPostCallback d, object state)
			{
				if (d == null) throw new ArgumentNullException("d");
				m_queue.Add(new KeyValuePair<SendOrPostCallback, object>(d, state));
			}

			/// <summary>Not supported.</summary>
			public override void Send(SendOrPostCallback d, object state)
			{
				throw new NotSupportedException("Synchronously sending is not supported.");
			}

			/// <summary>Runs an loop to process all queued work items.</summary>
			public void RunOnCurrentThread()
			{
				foreach (var workItem in m_queue.GetConsumingEnumerable())
					workItem.Key(workItem.Value);
			}

			/// <summary>Notifies the context that no more work will arrive.</summary>
			public void Complete() { m_queue.CompleteAdding(); }
		}

        private class ExclusiveSynchronizationContext : SynchronizationContext
        {
            private readonly AutoResetEvent _workItemsWaiting =
                new AutoResetEvent(false);

            private bool _done;
            private EventQueue _items;

            public Exception InnerException { get; set; }

            public ExclusiveSynchronizationContext(SynchronizationContext old)
            {
                ExclusiveSynchronizationContext oldEx =
                    old as ExclusiveSynchronizationContext;

                if (null != oldEx)
                {
                    this._items = oldEx._items;
                }
                else
                {
                    this._items = new EventQueue();
                }
            }

            public override void Send(SendOrPostCallback d, object state)
            {
                throw new NotSupportedException(
                    "We cannot send to our same thread");
            }

            public override void Post(SendOrPostCallback d, object state)
            {
                _items.Enqueue(Tuple.Create(d, state));
                _workItemsWaiting.Set();
            }

            public void EndMessageLoop()
            {
                Post(_ => _done = true, null);
            }

            public void BeginMessageLoop()
            {
                while (!_done)
                {
                    EventTask task = null;

                    if (!_items.TryDequeue(out task))
                    {
                        task = null;
                    }

                    if (task != null)
                    {
                        task.Item1(task.Item2);
                        if (InnerException != null) // method threw an exeption
                        {
                            throw new AggregateException(
                                "AsyncBridge.Run method threw an exception.",
                                InnerException);
                        }
                    }
                    else
                    {
                        _workItemsWaiting.WaitOne();
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