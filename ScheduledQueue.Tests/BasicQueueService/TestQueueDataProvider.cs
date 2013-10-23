using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	class TestQueueDataProvider : InProcQueueDataProvider
	{
		public int NumTotalMessages(string queueName)
		{
			Queue queue = GetQueue(queueName);
			return queue.Messages.Count();
		}

		public int NumAvailableMessages(string queueName, DateTime before)
		{
			Queue queue = GetQueue(queueName);
			return queue.Messages.Where(m => m.AvailabilityDate <= before).Count();
		}

		public string GetMessageId(string queueName, int index)
		{
			Queue queue = GetQueue(queueName);
			return queue.Messages[index].MessageId;
		}

		public string GetMessageBody(string queueName, int index)
		{
			Queue queue = GetQueue(queueName);
			return queue.Messages[index].MessageBody;
		}

		public DateTime GetMessageDate(string queueName, int index)
		{
			Queue queue = GetQueue(queueName);
			return queue.Messages[index].AvailabilityDate;
		}

		public int FindMessageFromId(string queueName, string messageId)
		{
			Queue queue = GetQueue(queueName);
			for (int i = 0; i < queue.Messages.Count(); ++i)
			{
				if (queue.Messages[i].MessageId == messageId)
					return i;
			}
			return -1;
		}
	}
}
