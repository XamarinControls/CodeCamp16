using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Net.Http;
using ModernHttpClient;
using Xamarin.Android.Net;
using CodeCamp2016.Forms.Rest;

namespace CodeCamp2016.Droid.Rest
{
	/// <summary>
	/// The rest core client.
	/// </summary>
	public class RestCoreClient : IRestClient
	{
		private Dictionary<string, string> _ResponseHeaders;

		public Dictionary<string, string> ResponseHeaders
		{
			get
			{
				return _ResponseHeaders;
			}
			private set
			{
				_ResponseHeaders = value;
			}
		}

		public Func<byte[], object> GetResult { get; set; }

		/// <summary>
		/// JSON String returned
		/// </summary>
		/// <value>The raw json.</value>
		public string RawJson { get; private set; }


		/// <summary>
		/// The Http client.
		/// </summary>
		protected HttpClient Client;

		public RestCoreClient()
		{
			if (Android.OS.Build.VERSION.SdkInt > Android.OS.BuildVersionCodes.Lollipop)
			{
				this.Client = new HttpClient(new AndroidClientHandler());
			}
			else
			{
				this.Client = new HttpClient(new NativeMessageHandler());
			}
		}

		/// <summary>
		/// Gets or sets timeout in milliseconds
		/// </summary>
		public TimeSpan Timeout
		{
			get
			{
				return this.Client.Timeout;
			}

			set
			{
				this.Client.Timeout = value;
			}
		}

		/// <summary>
		/// Gets the string content type.
		/// </summary>
		protected string StringContentType
		{
			get
			{
				return "application/json";
			}
		}

		/// <summary>
		/// Add request header.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		/// <param name="value">
		/// The value.
		/// </param>
		public void AddHeader(string key, string value)
		{
			this.Client.DefaultRequestHeaders.Add(key, value);
		}


		/// <summary>
		/// Remove request header.
		/// </summary>
		/// <param name="key">
		/// The key.
		/// </param>
		public void RemoveHeader(string key)
		{
			this.Client.DefaultRequestHeaders.Remove(key);
		}


		/// <summary>
		/// Gets the response from Http response message
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <param name="response">Http response message</param>
		/// <returns>The async task.</returns>
		private async Task<T> GetResponse<T>(HttpResponseMessage response)
		{
			await CheckResponse(response);

			if (this.GetResult != null)
			{
				return (T)this.GetResult(await response.Content.ReadAsByteArrayAsync());
			}
			else {
				//var t2 = typeof(T);
				var content = await response.Content.ReadAsStringAsync();
				return Newtonsoft.Json.JsonConvert.DeserializeObject<T>(content);
			}
		}

		/// <summary>
		/// Gets the response from Http response message
		/// </summary>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		/// <param name="response">Http response message</param>
		/// <param name="ToType">Type convert</param>
		/// <returns>The async task.</returns>
		private async Task<T> GetResponse<T>(HttpResponseMessage response, Type ToType)
		{
			await CheckResponse(response);

			if (this.GetResult != null)
			{
				return (T)this.GetResult(await response.Content.ReadAsByteArrayAsync());
			}
			else {
				//var t2 = typeof(T);
				var content = await response.Content.ReadAsStringAsync();
				return (T)Newtonsoft.Json.JsonConvert.DeserializeObject(content, ToType);
			}
		}

		private async Task<HttpResponseMessage> CheckResponse(HttpResponseMessage response)
		{
			this._ResponseHeaders = new Dictionary<string, string>();
			foreach (var item in response.Headers)
			{
				this._ResponseHeaders.Add(item.Key, item.Value.FirstOrDefault());
			}

			if (!response.IsSuccessStatusCode)
			{
				string msg = "";
				try
				{
					msg = await response.Content.ReadAsStringAsync();
				}
				catch { }
				throw new WebResponseException(response.StatusCode, String.IsNullOrEmpty(msg) ? response.ReasonPhrase : msg);
			}

			return response;
		}



		public void Dispose()
		{
			if (this.Client != null)
			{
				try
				{
					this.Client.Dispose();
				}
				catch (MethodAccessException)
				{
				}
				catch (MissingMethodException)
				{
				}
				finally
				{
					this.Client = null;
				}
			}

			if (this._ResponseHeaders != null)
			{
				this._ResponseHeaders.Clear();
			}
		}

		/// <summary>
		/// Deletes the async.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address.</param>
		public async Task DeleteAsync(string address)
		{
			var response = await this.Client.DeleteAsync(address);
			await this.CheckResponse(response);
		}


		/// <summary>
		/// Deletes the async.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public async Task<T> DeleteAsync<T>(string address, Type OutputType)
		{
			var response = await this.Client.DeleteAsync(address);
			return (T)await GetResponse<object>(response, OutputType);
		}


		/// <summary>
		/// Async POST method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <param name="dto">DTO to post.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		public async Task<T> PostAsync<T>(string address, object dto, Type OutputType)
		{
			var content = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
			var response = await this.Client.PostAsync(
				address,
				new StringContent(content, Encoding.UTF8, this.StringContentType));
			return (T)await GetResponse<object>(response, OutputType);
		}

		/// <summary>
		/// Posts the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="address">Address.</param>
		/// <param name="dto">Dto.</param>
		public async Task PostAsync(string address, object dto)
		{
			var content = Newtonsoft.Json.JsonConvert.SerializeObject(dto);
			var response = await this.Client.PostAsync(address, new StringContent(content, Encoding.UTF8, this.StringContentType));
			await this.CheckResponse(response);
		}

		/// <summary>
		/// Posts the async.
		/// </summary>
		/// <returns>The async.</returns>
		/// <param name="address">Address.</param>
		/// <param name="OutputType">Output type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public async Task<T> PostAsync<T>(string address, Type OutputType)
		{
			var response = await this.Client.PostAsync(
				address,
				new StringContent("", Encoding.UTF8, this.StringContentType));

			return (T)await GetResponse<object>(response, OutputType);
		}

		/// <summary>
		/// Async PUT method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <param name="dto">DTO to put.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		public async Task<T> PutAsync<T>(string address, object dto)
		{
			var content = Newtonsoft.Json.JsonConvert.SerializeObject(dto);

			var response = await this.Client.PutAsync(
				address,
				new StringContent(content, Encoding.UTF8, this.StringContentType));

			return (T)await GetResponse<object>(response);
		}

		/// <summary>
		/// Async GET method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		public async Task<T> GetAsync<T>(string address, Type OutputType)
		{
			var response = await this.Client.GetAsync(address);
			return (T)await GetResponse<object>(response, OutputType);
		}

		/// <summary>
		/// Async GET method.
		/// </summary>
		/// <returns>The async task.</returns>
		/// <param name="address">Address of the service.</param>
		/// <typeparam name="T">The type of object to be returned.</typeparam>
		public async Task<T> GetAsync<T>(string address)
		{
			var response = await this.Client.GetAsync(address);
			return await GetResponse<T>(response);
		}
	}
}

