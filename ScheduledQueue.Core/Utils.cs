using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public static class Utils
	{
		private const string Iso8601DateFormat = "yyyy-MM-dd\\THH:mm:ss.fff\\Z";

		/// <summary>
		/// Formats a DateTime object into ISO8601 format.
		/// </summary>
		/// <param name="date">Date to format.</param>
		/// <returns>Formatted string of the date.</returns>
		public static string FormatIso8601Date(DateTime date)
		{
			return date.ToUniversalTime().ToString(Iso8601DateFormat, CultureInfo.InvariantCulture);
		}

		/// <summary>
		/// Parses an ISO8601 date string into a date object.
		/// </summary>
		/// <param name="dateString">Formatted string to parse.</param>
		/// <returns>DateTime object in UTC.</returns>
		public static DateTime ParseIso8601Date(string dateString)
		{
			return DateTime.ParseExact(dateString, Iso8601DateFormat, CultureInfo.InvariantCulture);
		}
	}
}
