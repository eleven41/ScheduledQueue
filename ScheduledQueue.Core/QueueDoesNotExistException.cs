using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class QueueDoesNotExistException : Exception
	{
		public QueueDoesNotExistException(string queueName)
			: base("Queue does not exist: " + queueName)
		{
			this.QueueName = queueName;
		}

		public string QueueName { get; set; }
	}
}
