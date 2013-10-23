using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class QueueNotFoundException : Exception
	{
		public QueueNotFoundException(string queueName)
			: base("Queue not found: " + queueName)
		{
			this.QueueName = queueName;
		}

		public string QueueName { get; set; }
	}
}
