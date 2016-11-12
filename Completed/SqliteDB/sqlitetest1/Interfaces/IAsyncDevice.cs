using System;
using System.Threading.Tasks;

namespace sqlitetest1.Interfaces
{
	public interface IAsyncDevice : IDisposable
	{
		/// <summary>
		/// Execute's an async task with a void return type
		/// from a synchronous context
		/// </summary>
		/// <param name="task">Task to execute</param>
		/// <param name="callback">Optional callback</param>
		void Run(Task task, Action<Task> callback = null);

		/// <summary>
		/// Execute's an async task with a T return type
		/// from a synchronous context
		/// </summary>
		/// <typeparam name="T">The type of the task</typeparam>
		/// <param name="task">Task to execute</param>
		/// <param name="callback">Optional callback</param>
		void Run<T>(Task<T> task, Action<Task<T>> callback = null);

		void Run<T>(Task<T> task, Action<T> callback);

		/// <summary>
		/// Execute's an async task with a void return type
		/// from a synchronous context
		/// </summary>
		/// <param name="task">Function that returns a Task</param>
		/// <param name="callback">Optional callback</param>
		void Run(Func<Task> task, Action<Task> callback = null);


		/// <summary>
		/// Execute's an async task with a T return type
		/// from a synchronous context
		/// </summary>
		/// <typeparam name="T">The type of the task</typeparam>
		/// <param name="task">Function that returns Task&lt;T&gt; to execute</param>
		/// <param name="callback">Optional callback</param>
		void Run<T>(Func<Task<T>> task, Action<Task<T>> callback = null);
	}
}
