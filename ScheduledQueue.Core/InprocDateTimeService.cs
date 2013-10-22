using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class InProcDateTimeService : IDateTimeService
	{
		#region IDateTimeService Members

		public DateTime GetCurrentDateTime()
		{
			return DateTime.UtcNow;
		}

		#endregion
	}
}
