using System;
using CodeCamp2016.Forms.Alerts;

namespace CodeCamp2016.iOS.Alerts
{
	public class AlertMessages : IAlertMessages
	{
		private readonly BigTed.ProgressHUD.MaskType HUDDefaultMask = BigTed.ProgressHUD.MaskType.Black;

		public AlertMessages()
		{
			InvokeOnMain(() => {
				BigTed.BTProgressHUD.ForceiOS6LookAndFeel = true;
			});
		}

		/// <summary>
		/// Run on Main UI Thread
		/// </summary>
		/// <param name="action">Action.</param>
		private void InvokeOnMain(Action action)
		{
			UIKit.UIApplication.SharedApplication.InvokeOnMainThread(action);
		}

        #region IStatusMessages implementation

        public void ShowSuccess(string message, int timeoutMS = 1000, Action calllback = null)
        {
			InvokeOnMain (() => {
				BigTed.BTProgressHUD.ShowSuccessWithStatus (message, timeoutMS);
			});
        }

        public void Show(string message)
        {
			Show (message, -1);
        }

        public void Show(string message, int progress = -1)
        {
			InvokeOnMain ( ()=> {
				BigTed.BTProgressHUD.Show(message, progress, BigTed.ProgressHUD.MaskType.Black);
			});
        }

        public void Dismiss()
		{
			InvokeOnMain (() => {
				BigTed.BTProgressHUD.Dismiss();
				//XHUD.HUD.Dismiss ();
			});
        }

        public void ShowToast(string message, bool showToastCentered = true, int timeoutMS = 1000)
        {		
			InvokeOnMain (() => {				
				BigTed.BTProgressHUD.ShowToast(message, showToastCentered, timeoutMS);
			});
        }

        #endregion

    }
}