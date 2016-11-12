using System;

namespace CodeCamp2016.Forms.Alerts
{
	public interface IAlertMessages
	{
		/// <summary>
		/// Shows the success.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="timeoutMS">Timeout M.</param>
		/// <param name="calllback">Calllback.</param>
		void ShowSuccess(string message,  Int32 timeoutMS = 1000, Action calllback = null);

		/// <summary>
		/// Show the specified message.
		/// </summary>
		/// <param name="message">Message.</param>
        void Show(string message);

		/// <summary>
		/// Show the specified message, progress and maskType.
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="progress">Progress.</param>
        void Show (string message, int progress = -1);

		/// <summary>
		/// Dismiss all Notifications
		/// </summary>
        void Dismiss ();

		/// <summary>
		/// Toast Non Blocking (use Mask Clear)
		/// </summary>
		/// <param name="message">Message.</param>
		/// <param name="showToastCentered">If set to <c>true</c> show toast centered.</param>
		/// <param name="timeoutMS">Timeout M.</param>
        void ShowToast(string message, bool showToastCentered = true, Int32 timeoutMS = 1000);
	}
}

