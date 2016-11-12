using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Reflection;
using System.Collections.Generic;
using Xamarin.Forms;
using System.Collections;
using System.Text;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Threading;
using System.Linq.Expressions;

namespace System
{
	public class DateTimeUtils
	{
		public static DateTime EpochMax
		{
			get {
				return new DateTime (2038, 1, 19, 0, 0, 0, DateTimeKind.Utc);
			}
		}

		protected internal static DateTime EpochMinumum
		{
			get {
				return new DateTime (1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			}
		}

		public static DateTime MaxDate
		{
			get {
				return new DateTime (2038, 1, 19, 0, 0, 0);
			}
		}

		public static DateTime MinDate
		{
			get {
				return new DateTime (2001, 1, 1, 0, 0, 0);
			}
		}
	}
}

namespace sqlitetest1.Exts
{
	public static class MyExts
	{
		/// <summary>
		/// Javas the long to date.
		/// </summary>
		/// <returns>The long to date.</returns>
		/// <param name="javaLong">Java long.</param>
		public static DateTime JavaLongToDate(this long javaLong)
		{
			DateTime unixYear0 = DateTimeUtils.EpochMinumum;
			long unixTimeStampInTicks = javaLong / 1000 * TimeSpan.TicksPerSecond;
			DateTime dtUnix = new DateTime(unixYear0.Ticks + unixTimeStampInTicks);
			return dtUnix;
		}

		/// <summary>
		/// DateTime to java long, Convert Time before calling this Extension method
		/// </summary>
		/// <returns>The java long.</returns>
		/// <param name="dateTime">Date time.</param>
		public static long ToJavaLong(this DateTime dateTime)
		{			
			DateTime Jan1st1970 = DateTimeUtils.EpochMinumum;        

			if (dateTime < Jan1st1970) {
				dateTime = Jan1st1970;
			}

			if (dateTime > DateTimeUtils.EpochMax) {
				dateTime = DateTimeUtils.EpochMax;
			}

			return (long)(dateTime - Jan1st1970).TotalMilliseconds;;
			

			//return (long)(dateTime.ToUniversalTime() - Jan1st1970).TotalMilliseconds;;

			//var epoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
			//return Convert.ToInt64((dateTime.ToUniversalTime() - Jan1st1970).TotalSeconds);
		}

		/// <summary>
		/// DateTime to MilliSeconds
		/// </summary>
		/// <param name="d">DateTime</param>
		/// <returns>MilliSeconds</returns>
		public static Int64 ToMilliSeconds(this DateTime d)
		{
			return (long)(d - DateTimeUtils.EpochMinumum).TotalMilliseconds;
		}

		/// <summary>
		/// Convert a DateTime to the End of the Day (23:59:59)
		/// </summary>
		/// <returns>DateTime</returns>
		/// <param name="d">DateTime</param>
		public static DateTime ToEndOfDay(this DateTime d)
		{
			return DateTime.Parse (d.Date.ToString("d").Trim () + " 23:59:59");
		}

		public static DateTime ToStartDay(this DateTime d)
		{
			return DateTime.Parse (d.Date.ToString ("d").Trim () + " 00:00:01");
		}


		public static T GetValue<T>(this BindableObject bindableObject, BindableProperty property)
		{
			return (T)bindableObject.GetValue(property);
		}

		/// <summary>
		/// Tos the observable collection.
		/// </summary>
		/// <returns>The observable collection.</returns>
		/// <param name="enumerableList">Enumerable list.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static ObservableCollection<T> ToObservableCollection<T>(this IEnumerable<T> enumerableList)
		{
			if (enumerableList != null)
			{
				// Create an emtpy observable collection object
				var observableCollection = new ObservableCollection<T>();

				enumerableList.ForEach ((item) => {
					observableCollection.Add(item);
				});

				// Return the populated observable collection
				return observableCollection;
			}
			return null;
		}

		/// <summary>
		/// Foreach for IEnumerable
		/// </summary>
		/// <param name="enumerable">Enumerable.</param>
		/// <param name="action">Action.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static void ForEach<T>(this IEnumerable<T> enumerable, Action<T> action)
		{
			foreach (T item in enumerable) action.Invoke(item);
		}

		public static string FirstValue(this Dictionary<string, string> item, string key)
		{
			var result = item.Where (x => x.Key == key).Select(x => x.Value).FirstOrDefault ();
			return result;
		}

		public static T FirstValue<T>(this Dictionary<string, string> item, string key)
		{
			var result = item.Where (x => x.Key == key).Select(x => x.Value).FirstOrDefault ();
			if (result == null) {
				return default(T);
			}
			return (T)Convert.ChangeType(result, typeof(T));
		}

		public static void Execute(this Delegate t, params object[] args)
		{
			if (t != null)
			{
				t.DynamicInvoke(args);
			}
		}

		/// <summary>
		/// get the Exception Message
		/// </summary>
		/// <param name="ex"></param>
		/// <returns></returns>
		public static string ExceptionMessage(this Exception ex)
		{
			string message = "";
			Exception err = ex;
			while(err != null)
			{
				message = err.Message;
				err = err.InnerException;
			}

			return message;
		}



		public static void AddChild(this Grid grid, View view, int row, int column, int rowspan = 1, int columnspan = 1)
		{
			if (row < 0)
				throw new ArgumentOutOfRangeException("row");
			if (column < 0)
				throw new ArgumentOutOfRangeException("column");
			if (rowspan <= 0)
				throw new ArgumentOutOfRangeException("rowspan");
			if (columnspan <= 0)
				throw new ArgumentOutOfRangeException("columnspan");
			if (view == null)
				throw new ArgumentNullException("view");
			Grid.SetRow((BindableObject)view, row);
			Grid.SetRowSpan((BindableObject) view, rowspan);
			Grid.SetColumn((BindableObject) view, column);
			Grid.SetColumnSpan((BindableObject) view, columnspan);
			grid.Children.Add(view);      
		}



		public static bool Equal(this double d, double d2)
		{
			return Math.Abs(d - d2) < 0.0001;
		}

		public static bool NotEqual(this double d, double d2)
		{
			return d.Equal (d2) == false;
		}

		/// <summary>
		/// Convert list items using the function provided
		/// </summary>
		/// <param name="Source">Source list</param>
		/// <param name="projection">Convert function</param>
		/// <typeparam name="T">type parameter.</typeparam>
		public static void ConvertInPlace<T>(this IList<T> Source, Func<T, T> projection)
		{
			for (int i = 0; i < Source.Count; i++)
			{
				Source[i] = projection(Source[i]);
			}
		}

		/// <summary>
		/// Converts an object to an Object Array with 1 element
		/// </summary>
		/// <returns>The array.</returns>
		/// <param name="obj">Object.</param>
		public static object[] ObjectArray(this object obj)
		{
			return new[] { obj };
		}

		/// <summary>
		/// Get the property Value for the Object
		/// </summary>
		/// <returns>The property Value</returns>
		/// <param name="obj">Object</param>
		/// <param name="propName">Property name</param>
		/// <typeparam name="T">return type to return (use Object for generic)</typeparam>
		public static bool HasProperty(this object obj, string propName)
		{
			if (obj != null && propName != null) {
				var result = obj.GetType ().GetRuntimeProperties ()
					.Where (prop => prop.Name.ToLower () == propName.ToLower ())
					.Select (v => v).FirstOrDefault ();

				return result != null;
			}
			return false;
		}	

		/// <summary>
		/// Execute Property and return value
		/// </summary>
		/// <returns>The property Value</returns>
		/// <param name="obj">Object</param>
		/// <param name="propertyName">property name</param>
		/// <param name="defaultValue">default value if property fails</param>
		/// <typeparam name="T">return type to return (use Object for generic)</typeparam>
		public static T ExecuteProperty<T>(this object obj, string propertyName, T defaultValue)
		{
			try
			{
				return (T)obj.GetType().GetRuntimeProperty(propertyName).GetValue(obj);
			}
			catch
			{
				return defaultValue;
			}
		}

		/// <summary>
		/// Get the property Value for the Object
		/// </summary>
		/// <returns>The property Value</returns>
		/// <param name="obj">Object</param>
		/// <param name="methodName">Method name</param>
		/// <typeparam name="T">return type to return (use Object for generic)</typeparam>
		public static bool HasMethod(this object obj, string methodName)
		{
			if (obj != null && methodName != null) {
				var result = obj.GetType ().GetRuntimeMethods ()
					.Where (prop => prop.Name.ToLower () == methodName.ToLower ())
					.Select (v => v).FirstOrDefault ();

				return result != null;
			}
			return false;
		}	

		public static T InvokeMethod<T>(this object obj, string methodName, object[] parameters  = null)
		{
			if (obj != null && methodName != null) {
				var result = obj.GetType ().GetRuntimeMethods ()
					.Where (prop => prop.Name.ToLower () == methodName.ToLower ())
					.Select (v => v).FirstOrDefault ();

				if (result != null) {
					var output = result.Invoke (obj, parameters);
					return (T)Convert.ChangeType (output, typeof(T));
				}
			}
			return default(T);
		}	

		public static void InvokeMethod(this object obj, string methodName, object[] parameters)
		{
			if (obj != null && methodName != null) {
				var result = obj.GetType ().GetRuntimeMethods ()
					.Where (prop => prop.Name.ToLower () == methodName.ToLower ())
					.Select (v => v).FirstOrDefault ();

				if (result != null) {
					result.Invoke (obj, parameters);
				}
			}
		}

		/// <summary>
		/// Get the property Value for the Object
		/// </summary>
		/// <returns>The property Value</returns>
		/// <param name="obj">Object</param>
		/// <param name="propName">Property name</param>
		/// <typeparam name="T">return type to return (use Object for generic)</typeparam>
		public static T InvokeProperty<T>(this object obj, string propName)
		{
			if (obj != null && propName != null) {
				var result = obj.GetType ().GetRuntimeProperties ()
					.Where (prop => prop.Name.ToLower () == propName.ToLower ())
					.Select (v => v.GetValue (obj)).FirstOrDefault ();

				return (T)result;
			}
			return default(T);
		}

		/// <summary>
		/// Get the property Value for the Object
		/// </summary>
		/// <returns>The property Value</returns>
		/// <param name="obj">Object</param>
		/// <param name="propName">Property name</param>
		/// <typeparam name="T">return type to return (use Object for generic)</typeparam>
		public static void SetProperty<T>(this object obj, string propName, T value)
		{
			if (obj != null && propName != null) {

				var result = obj.GetType ().GetRuntimeProperties ()
					.Where (prop => prop.Name.ToLower () == propName.ToLower ())
					.Select (x => x).FirstOrDefault ();

				if (result != null) {
					try
					{
						//result.SetValue (obj, (T)Convert.ChangeType (value, typeof(T)), null);
						result.SetValue (obj, (T)value);
					}
					catch(Exception) {
						result.SetValue(obj, Convert.ChangeType(value, result.PropertyType), null);
					}
				}
			}
		}	

		#region "String"

		/// <summary>
		/// Convert a String to a Style
		/// </summary>
		/// <param name="text">Resource dictionary key</param>
		public static Xamarin.Forms.Style ToStyle(this string text)
		{
			return Xamarin.Forms.Application.Current.Resources [text] as Xamarin.Forms.Style;
		}

		public static string TitleCaseString(this string s)
		{
			if (s == null) return s;

			String[] words = s.Split(' ');
			for (int i = 0; i < words.Length; i++)
			{
				if (words[i].Length == 0) continue;

				Char firstChar = Char.ToUpper(words[i][0]); 
				String rest = "";
				if (words[i].Length > 1)
				{
					rest = words[i].Substring(1).ToLower();
				}
				words[i] = firstChar + rest;
			}
			return String.Join(" ", words);
		}

		public static string CharsOnly(this string value)
		{
			if (String.IsNullOrEmpty(value)) return "";

			var test = value.ToCharArray();
			return new string(Array.FindAll(test, (char obj) => char.IsLetter(obj)));
		}

		/// <summary>
		/// convert a string to the standard phone number 
		/// </summary>
		/// <returns>The phone.</returns>
		/// <param name="value">Value.</param>
		public static string ToPhone(this string value)
		{
			if (String.IsNullOrEmpty (value) == false) {
				value = value.Replace (new[] { '(', ')', '-', ' ' }, "");

				if (value.Length < 8) {
					return Regex.Replace (value, @"(\d{3})(\d{4})", "$1-$2");
				} else {
					return Regex.Replace (value, @"(\d{3})(\d{3})(\d{4})", "$1-$2-$3");
				}
			}

			return String.Empty;
		}

		/// <summary>
		/// Extract only Numbers from Strings
		/// </summary>
		/// <returns>The number.</returns>
		/// <param name="value">Original.</param>
		public static string ExtractNumbers(this string value)
		{
			return new string(value.ToCharArray().Where(c => Char.IsDigit(c)).ToArray());
		}

		/// <summary>
		/// convert a string to an enum
		/// </summary>
		/// <returns>The enum.</returns>
		/// <param name="text">Value type.</param>
		/// <typeparam name="T">The 1st type parameter.</typeparam>
		public static T ToEnum<T>(this string text) where T : struct
		{
			return (T) Enum.Parse( typeof(T), text, true );
		}

		/// <summary>
		/// Convert a String (characters by character) To an Integer
		/// </summary>
		/// <returns>The integer.</returns>
		/// <param name="obj">Object.</param>
		public static Int32 ToInteger(this string obj)
		{
			if (String.IsNullOrEmpty (obj) == false) {
				return (from x in obj.ToCharArray ()
					select (int)x).Sum ();
			}

			return 0;
		}

		public static string Replace(this string s, char[] separators, string newVal)
		{
			return String.Join(newVal, s.Split(separators, StringSplitOptions.RemoveEmptyEntries));
		}

		/// <summary>
		/// extract all the numbers from a string
		/// </summary>
		/// <param name="input">Input.</param>
		public static string Numbers(this string input)
		{
			return new String(input.ToCharArray().Where(Char.IsDigit).ToArray());
		}

		#endregion


		public static T ViewTo<T>(this object v) where T : class
		{
			return v as T;
		}

		/// <summary>
		/// You can use the CleanInput method defined in this example to strip potentially harmful 
		/// characters that have been entered into a text field that accepts user input. 
		/// In this case, CleanInput strips out all nonalphanumeric 
		/// characters except periods (.), at symbols (@), and hyphens (-), and 
		/// returns the remaining string. 
		/// However, you can modify the regular expression pattern so that it strips out any 
		/// characters that should not be included in an input string.
		/// </summary>
		/// <returns>The input.</returns>
		/// <param name="strIn">String in.</param>
		public static string CleanInput(this string strIn)
		{
			// Replace invalid characters with empty strings. 
			try
			{
				return Regex.Replace(strIn, @"[^\w\.@-]", "", RegexOptions.None, TimeSpan.FromSeconds(1));
			}
			// If we timeout when replacing invalid characters,  
			// we should return Empty. 
			catch (RegexMatchTimeoutException)
			{
				return String.Empty;
			}
		}

		public static async Task<bool> TimeoutAfter(this Task task, TimeSpan timeout)
		{
			var timeoutCancellationTokenSource = new CancellationTokenSource();

			var completedTask = await Task.WhenAny(task, Task.Delay(timeout, timeoutCancellationTokenSource.Token)).ConfigureAwait(false);
			if (completedTask == task)
			{
				timeoutCancellationTokenSource.Cancel();
				await task.ConfigureAwait(false);
				return true;
			}
			else
			{
				return false;
			}
		}

		/// <summary>
		/// var task = Task.Delay(TimeSpan.FromSeconds(15));

		//task.AwaitWithTimeoutCallback(TimeSpan.FromSeconds(5), () =>
		//{
		//	    System.Diagnostics.Debug.WriteLine("Waiting...");
		//	    return Task.FromResult(true);
		//	})
		//	.Wait();
		/// </summary>
		/// <returns>The with timeout callback.</returns>
		/// <param name="task">Task.</param>
		/// <param name="timeout">Timeout.</param>
		/// <param name="onTimeout">On timeout.</param>
		public static async Task AwaitWithTimeoutCallback(this Task task, TimeSpan timeout, Action onTimeout)
		{
			while (true)
			{
				var completed = await task.TimeoutAfter(timeout).ConfigureAwait(false);
				if (completed)
				{
					break;
				}

				onTimeout();
			}

			await task.ConfigureAwait(false);
		}
	}
}

