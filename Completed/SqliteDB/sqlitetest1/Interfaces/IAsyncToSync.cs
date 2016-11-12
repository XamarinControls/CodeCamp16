using System;
using System.Threading.Tasks;

namespace sqlitetest1.Interfaces
{
	// ref: https://github.com/tejacques/AsyncBridge
	public interface IAsyncToSync
	{
		void FireAndForget(Func<Task> task, Action<Exception> handle = null);

		/// <summary>Runs the specified asynchronous function.</summary>
		/// <param name="func">The asynchronous function to execute.</param>
		void Run(Func<Task> func);

		/// <summary>
		/// Creates a new AsyncBridge. This should always be used in
		/// conjunction with the using statement, to ensure it is disposed
		/// </summary>
		IAsyncDevice Wait { get; }
	}
}
