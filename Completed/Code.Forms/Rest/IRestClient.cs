using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCamp2016.Forms.Rest
{
	/// <summary>
	/// Rest Client interface.
	/// </summary>
	public interface IRestClient
	{
		/// <summary>
		/// JSON STRING returned
		/// </summary>
		/// <value>The raw json.</value>
		string RawJson { get; }

		Func<byte[], object> GetResult { get; set; }

		/// <summary>
		/// Returned HttpHeaders
		/// </summary>
		/// <value>The response headers.</value>
		Dictionary<string, string> ResponseHeaders { get; }

		/// <summary>
		/// Gets or sets timeout
		/// </summary>
		TimeSpan Timeout { get; set; }

		/// <summary>
		/// Add request header.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		void AddHeader(string key, string value);

		/// <summary>
		/// Remove request header.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		void RemoveHeader(string key);

		/// <summary>
		/// Async POST method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <param name="dto">DTO to post.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		Task<T> PostAsync<T>(string address, object dto, Type OutputType);


		/// <summary>
		/// Async GET method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		Task<T> GetAsync<T>(string address);

		/// <summary>
		/// Async GET method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		Task<T> GetAsync<T>(string address, Type OutputType);


		/// <summary>
		/// Deletes the async.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		Task<T> DeleteAsync<T>(string address, Type OutputType);

		/// <summary>
		/// Async POST method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <param name="dto">DTO to post.</param>
		Task PostAsync(string address, object dto);

		/// <summary>
		/// Deletes the async.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address.</param>
		Task DeleteAsync(string address);

		/// <summary>
		/// Async POST method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		Task<T> PostAsync<T>(string address, Type OutputType);


		/// <summary>
		/// Async PUT method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <param name="dto">DTO to put.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		Task<T> PutAsync<T>(string address, object dto);

		void Dispose();
	}
}

