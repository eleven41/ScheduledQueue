using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Client
{
	public class ScheduledQueueException : Exception
	{
		public ScheduledQueueException(string key, string message)
			: base(message)
		{
			this.Key = key;
		}

		public string Key { get; set; }
	}
}
