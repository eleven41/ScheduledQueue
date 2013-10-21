using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public interface IDateTimeService
	{
		/// <summary>
		/// Returns the current time in UTC.
		/// </summary>
		/// <returns>DateTime object of the current UTC time.</returns>
		DateTime GetCurrentDateTime();
	}
}
