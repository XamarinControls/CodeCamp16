using System;
using CodeCamp2016.Forms.Alerts;

namespace CodeCamp2016.Droid.Alerts
{
	public class AlertMessages : Java.Lang.Object, IAlertMessages
	{
		private AndroidHUD.MaskType _HUDDefaultMask
		{
			get
			{
				return AndroidHUD.MaskType.Black;
			}
		}

		private global::Android.Content.Context GetContext
		{
			get
			{
				return Xamarin.Forms.Forms.Context ?? Android.App.Application.Context;
			}
		}

		/// <summary>
		/// Run on Main UI Thread
		/// </summary>
		/// <param name="action">Action.</param>
		private void InvokeOnMain(Action action)
		{
			Xamarin.Forms.Device.BeginInvokeOnMainThread(action);
		}

		#region IStatusMessages implementation


		public void ShowSuccess(string message, Int32 timeoutMS = 1000, Action calllback = null)
		{
			AndroidHUD.AndHUD.Shared.ShowSuccess(GetContext, message, AndroidHUD.MaskType.Black,
				new TimeSpan(0, 0, 0, 0, timeoutMS), calllback, null);
		}


		public void Show(string message)
		{
			Show(message, -1);
		}

		public void Show(string message, int progress = -1)
		{
			var context = GetContext;
			if (context != null)
			{
				AndroidHUD.AndHUD.Shared.Show(context, message, -1, AndroidHUD.MaskType.Black, default(TimeSpan), null, true);
			}
		}

		public void Dismiss()
		{
			InvokeOnMain(() =>
			{
				AndroidHUD.AndHUD.Shared.Dismiss();
			});
		}

		/// <summary>
		/// Toast non blocking messages
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="showToastCentered">If set to <c>true</c> show toast centered.</param>
		/// <param name="timeoutMS">Timeout M.</param>
		public void ShowToast(string message, bool showToastCentered = true, Int32 timeoutMS = 1000)
		{
			var context = GetContext;
			if (context != null)
			{
				AndroidHUD.AndHUD.Shared.ShowToast(context, message, AndroidHUD.MaskType.None, new TimeSpan(0, 0, 0, 0, timeoutMS),
					showToastCentered, null, null);
			}

		}

		#endregion
	}
}

