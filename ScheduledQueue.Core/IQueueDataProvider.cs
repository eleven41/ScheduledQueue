using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public interface IQueueDataProvider
	{
		/// <summary>
		/// Returns the names of the available queues.
		/// </summary>
		/// <returns>IEnumerable of the queue names.</returns>
		IEnumerable<string> GetQueues();

		/// <summary>
		/// Inserts a new queue into the list of queues. If a queue of the same name already
		/// exists, no exception is thrown.
		/// </summary>
		/// <param name="queueName">Name of the queue to insert.</param>
		void InsertQueue(string queueName);

		/// <summary>
		/// Deletes the queue with the specified name. If no queue exists with the specified name,
		/// no exception is thrown.
		/// </summary>
		/// <param name="queueName"></param>
		void DeleteQueue(string queueName);

		/// <summary>
		/// Inserts a message into the specified queue.
		/// </summary>
		/// <param name="queueName">Name of the queue.</param>
		/// <param name="messageId">Message identifier of the new message. Must be unique amongst all messages in the queue.</param>
		/// <param name="messageBody">Body of the message.</param>
		/// <param name="availabilityDate">Availability date of the message.</param>
		void InsertMessage(string queueName, string messageId, string messageBody, DateTime availabilityDate);

		/// <summary>
		/// Returns the earliest message earlier than the indicated date and updates the availability date of the message.
		/// </summary>
		/// <param name="queueName">Name of the queue.</param>
		/// <param name="beforeDate">Latest date of the returned message.</param>
		/// <param name="newAvailabiltyDate">New availability date.</param>
		/// <returns>Message data. Returns null if no message is found.</returns>
		GetMessageResult GetMessage(string queueName, DateTime beforeDate, DateTime newAvailabiltyDate);

		/// <summary>
		/// Deletes a message from the queue.
		/// </summary>
		/// <param name="queueName">Name of the queue.</param>
		/// <param name="messageId">Unique identifier of the message.</param>
		void DeleteMessage(string queueName, string messageId);

		/// <summary>
		/// Updates a message in the queue.
		/// </summary>
		/// <param name="queueName">Name of the queue.</param>
		/// <param name="messageId">Unique identifier of the message to update.</param>
		/// <param name="newMessageId">New unique identifier for the message.</param>
		/// <param name="newAvailabilityDate">New availability date for the message.</param>
		void UpdateMessage(string queueName, string messageId, string newMessageId, DateTime newAvailabilityDate);
	}

	public class GetMessageResult
	{
		public string MessageId { get; set; }
		public string MessageBody { get; set; }
		public DateTime AvailabilityDate { get; set; }
	}
}
