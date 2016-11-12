using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeCamp2016.Forms.Rest
{
	public class ServiceCalls
	{
		private const string ServiceUrl = "http://192.168.202.6/CodeCamp2016Service/api/";

		/// <summary>
		/// Login
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		/// <returns>"" = success</returns>
		public static async Task<string> Login(string username, string password)
		{
			IRestClient client = null;
			try
			{
				client = Xamarin.Forms.DependencyService.Get<IRestClient>(Xamarin.Forms.DependencyFetchTarget.NewInstance);
				client.AddHeader("Authorization",
				                 "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}")));

				var result = await client.GetAsync<string>($"{ServiceUrl}Login");
				return result;

			}
			catch (WebResponseException ex)
			{
				return ex.Message;
			}
			catch (Exception ex)
			{
				return ex.Message;
			}
			finally
			{
				if (client != null)
				{
					client.Dispose();
					client = null;
				}
			}
		}

		/// <summary>
		/// Register the specified username, password and emailAddress.
		/// </summary>
		/// <param name="username">Username.</param>
		/// <param name="password">Password.</param>
		/// <param name="emailAddress">Email address.</param>
		/// <returns>"" = success</returns>
		public static async Task<string> Register(string username, string password, string emailAddress)
		{
			IRestClient client = null;
			try
			{
				client = Xamarin.Forms.DependencyService.Get<IRestClient>(Xamarin.Forms.DependencyFetchTarget.NewInstance);
				client.AddHeader("Authorization",
								 "Basic " + Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes($"{username}:{password}")));

				var body = new CodeCamp2016.Models.Register ();
				body.EmailAddress = emailAddress;

				var result = await client.PutAsync<string>($"{ServiceUrl}Login/Register", body);
				return result;

			}
			catch (WebResponseException ex)
			{
				return ex.Message;
			}
			catch (Exception ex)
			{
				return "Unknown Error";
			}
			finally
			{
				if (client != null)
				{
					client.Dispose();
					client = null;
				}
			}
		}


		/// <summary>
		/// Get Car Items
		/// </summary>
		/// <returns>The items.</returns>
		public static async Task<List<CodeCamp2016.Models.Items>> GetItems()
		{
			IRestClient client = null;
			try
			{
				client = Xamarin.Forms.DependencyService.Get<IRestClient>(Xamarin.Forms.DependencyFetchTarget.NewInstance);

				var result = await client.GetAsync<List<CodeCamp2016.Models.Items>>($"{ServiceUrl}Items/ForSale");
				return result;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				if (client != null)
				{
					client.Dispose();
					client = null;
				}
			}
		}
	}
}
