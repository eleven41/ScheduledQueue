using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class InProcDataStorage : IDataStorage
	{
		class Queue
		{
			public Queue(string queueName)
			{
				this.Name = queueName;
				this.Messages = new List<Message>();	
			}

			public string Name { get; private set; }

			public List<Message> Messages { get; private set; }
		}

		class Message
		{
			public string MessageId { get; set; }
			public string MessageBody { get; set; }
			public DateTime OriginalAvailabilityDate { get; set; }
			public DateTime AvailabilityDate { get; set; }
		}

		ConcurrentDictionary<string, Queue> _queues = new ConcurrentDictionary<string, Queue>();

		#region IDataStorage Members

		public IEnumerable<string> GetQueues()
		{
			List<string> results = new List<string>();
			foreach (Queue queue in _queues.Values)
			{
				results.Add(queue.Name);
			}
			return results.OrderBy(s => s.ToLower());
		}

		public void InsertQueue(string queueName)
		{
			Queue newQueue = new Queue(queueName);
			_queues.AddOrUpdate(queueName.ToLower(), newQueue, (name, queue) =>
				{
					// Return the original queue
					return queue;
				});
		}

		public void DeleteQueue(string queueName)
		{
			Queue queue;
			if (!_queues.TryRemove(queueName.ToLower(), out queue))
			{
				// Queue could not be deleted
			}
		}

		Queue GetQueue(string queueName)
		{
			Queue queue;
			if (_queues.TryGetValue(queueName.ToLower(), out queue))
				return queue;
			return null;
		}

		public void InsertMessage(string queueName, string messageId, string messageBody, DateTime availabilityDate)
		{
			Queue queue = GetQueue(queueName);
			if (queue == null)
				throw new Exception("Queue does not exist: " + queueName);

			Message message = new Message()
			{
				MessageId = messageId,
				MessageBody = messageBody,
				AvailabilityDate = availabilityDate,
				OriginalAvailabilityDate = availabilityDate
			};

			lock (queue.Messages)
			{
				queue.Messages.Add(message);
			}
		}

		public GetMessageResult GetMessage(string queueName, DateTime beforeDate, DateTime newAvailabiltyDate)
		{
			Queue queue = GetQueue(queueName);
			if (queue == null)
				throw new Exception("Queue does not exist: " + queueName);

			lock (queue.Messages)
			{
				var message = queue.Messages.Where(m => m.AvailabilityDate <= beforeDate).OrderBy(m => m.AvailabilityDate).FirstOrDefault();
				if (message == null)
					return null;

				message.AvailabilityDate = newAvailabiltyDate;

				return new GetMessageResult()
				{
					MessageId = message.MessageId,
					MessageBody = message.MessageBody,
					AvailabilityDate = message.OriginalAvailabilityDate
				};
			}
		}

		public void DeleteMessage(string queueName, string messageId)
		{
			Queue queue = GetQueue(queueName);
			if (queue == null)
				throw new Exception("Queue does not exist: " + queueName);

			lock (queue.Messages)
			{
				var message = queue.Messages.Where(m => m.MessageId == messageId).FirstOrDefault();

				// Delete this message from the queue
				queue.Messages.Remove(message);
			}
		}

		public void UpdateMessage(string queueName, string messageId, string newMessageId, DateTime newAvailabilityDate)
		{
			Queue queue = GetQueue(queueName);
			if (queue == null)
				throw new Exception("Queue does not exist: " + queueName);

			lock (queue.Messages)
			{
				var message = queue.Messages.Where(m => m.MessageId == messageId).FirstOrDefault();
				if (message == null)
					throw new Exception("Message not found with ID: " + messageId);

				message.MessageId = newMessageId;
				message.AvailabilityDate = newAvailabilityDate;
				message.OriginalAvailabilityDate = newAvailabilityDate;
			}
		}

		#endregion
	}
}
