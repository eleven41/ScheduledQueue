using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	/// <summary>
	/// Defines the interface for working with queues and messages.
	/// </summary>
	public interface IQueueService
	{
		/// <summary>
		/// Returns the list of available queues.
		/// </summary>
		/// <returns>IEnumerable of queue names.</returns>
		IEnumerable<string> ListQueues();

		/// <summary>
		/// Creates a new queue with the specified name if it does not already exist.
		/// </summary>
		/// <param name="queueName">Name of the queue to create.</param>
		/// <returns>Name of the queue.</returns>
		string CreateQueue(string queueName);

		/// <summary>
		/// Deletes the queue with the specified name. 
		/// If a queue with the specified name does not exist, no exception is thrown.
		/// </summary>
		/// <param name="queueName">Name of the queue to delete.</param>
		void DeleteQueue(string queueName);

		/// <summary>
		/// Adds a message to the specified queue. The message will be available to be
		/// received after the availability date.
		/// </summary>
		/// <param name="queueName">Name of the queue to which the message will be added.</param>
		/// <param name="messageBody">Body of the message.</param>
		/// <param name="availabilityDate">Availability date of the message.</param>
		/// <returns>Unique identifier of the message.</returns>
		string SendMessage(string queueName, string messageBody, DateTime availabilityDate);

		/// <summary>
		/// Receives the next available message from a queue. The message is not removed from the queue, 
		/// but is not made available again for a time specified.
		/// </summary>
		/// <param name="queueName">Name of the queue from which to receive the next message.</param>
		/// <param name="receiveTimeout">Amount of time (in ms) to wait for a message to arrive.</param>
		/// <param name="visibilityTimeout">Amount of time (in ms) where the message is unavailable to receivers.</param>
		/// <returns>Message data if a message is found. Null otherwise.</returns>
		ReceiveMessageResult ReceiveMessage(string queueName, long receiveTimeout, long visibilityTimeout);

		/// <summary>
		/// Deletes the specified message
		/// </summary>
		/// <param name="queueName">Name of the queue from which to delete the message.</param>
		/// <param name="messageId">Identifier of the message.</param>
		void DeleteMessage(string queueName, string messageId);

		/// <summary>
		/// Reschedules a message for the specified availability date.
		/// </summary>
		/// <param name="queueName">Name of the queue in which the message resides.</param>
		/// <param name="messageId">Identifier of the message.</param>
		/// <param name="availabilityDate">New availability date of the message.</param>
		/// <returns>New identifier of the message.</returns>
		string RescheduleMessage(string queueName, string messageId, DateTime availabilityDate);
	}

	public class ReceiveMessageResult
	{
		public string MessageId { get; set; }
		public string MessageBody { get; set; }
		public DateTime MessageDate { get; set; }
	}
}
