using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class BasicQueueService : IQueueService
	{
		IDataStorage _dataStorage;
		IDateTimeService _dateTimeService;

		QueueSignal _queueSignal;

		public BasicQueueService(IDataStorage dataStorage, IDateTimeService dateTimeService)
		{
			_dataStorage = dataStorage;
			_dateTimeService = dateTimeService;
			_queueSignal = QueueSignal.Instance;
		}

		#region IQueueService Members

		public IEnumerable<string> ListQueues()
		{
			return _dataStorage.GetQueues();
		}

		public string CreateQueue(string queueName)
		{
			_dataStorage.InsertQueue(queueName);
			return queueName;
		}

		public void DeleteQueue(string queueName)
		{
			_dataStorage.DeleteQueue(queueName);
		}

		public SendMessageResult SendMessage(string queueName, string messageBody)
		{
			var availabilityDate = _dateTimeService.GetCurrentDateTime();
			return SendMessage(queueName, messageBody, availabilityDate);
		}

		public SendMessageResult SendMessage(string queueName, string messageBody, DateTime availabilityDate)
		{
			string messageId = GenerateMessageId();
			_dataStorage.InsertMessage(queueName, messageId, messageBody, availabilityDate);

			// Notify any waiting receivers
			_queueSignal.Signal(queueName);

			return new SendMessageResult()
			{
				MessageId = messageId,
				MessageDate = availabilityDate
			};
		}

		private static string GenerateMessageId()
		{
			return Guid.NewGuid()
				.ToString()
				.Replace("-", "");
		}

		public SendMessageResult SendMessage(string queueName, string messageBody, TimeSpan availabilityDelay)
		{
			var currentTime = _dateTimeService.GetCurrentDateTime();
			var availabilityDate = currentTime + availabilityDelay;
			return SendMessage(queueName, messageBody, availabilityDate);
		}

		public ReceiveMessageResult ReceiveMessage(string queueName, TimeSpan receiveTimeout, TimeSpan visibilityTimeout)
		{
			var currentTime = _dateTimeService.GetCurrentDateTime();
			var newAvailabilityDate = currentTime + visibilityTimeout;
			var message = _dataStorage.GetMessage(queueName, currentTime, newAvailabilityDate);
			if (message != null)
			{
				return new ReceiveMessageResult()
				{
					MessageBody = message.MessageBody,
					MessageId = message.MessageId,
					MessageDate = message.AvailabilityDate
				};
			}

			var endTime = currentTime + visibilityTimeout;
			while (currentTime < endTime)
			{
				// No message was found, wait for the receive timeout period to see if anything new comes
				TimeSpan delay = endTime - currentTime;
				if (!_queueSignal.Wait(queueName, delay))
					break;

				// Try again
				newAvailabilityDate = currentTime + visibilityTimeout;
				message = _dataStorage.GetMessage(queueName, currentTime, newAvailabilityDate);
				if (message != null)
				{
					return new ReceiveMessageResult()
					{
						MessageBody = message.MessageBody,
						MessageId = message.MessageId,
						MessageDate = message.AvailabilityDate
					};
				}

				currentTime = _dateTimeService.GetCurrentDateTime();
			}

			return null;
		}

		public void DeleteMessage(string queueName, string messageId)
		{
			_dataStorage.DeleteMessage(queueName, messageId);
		}

		public RescheduleMessageResult RescheduleMessage(string queueName, string messageId, DateTime availabilityDate)
		{
			string newMessageId = GenerateMessageId();
			_dataStorage.UpdateMessage(queueName, messageId, newMessageId, availabilityDate);
			return new RescheduleMessageResult()
			{
				NewMessageId = newMessageId,
				MessageDate = availabilityDate
			};
		}

		public RescheduleMessageResult RescheduleMessage(string queueName, string messageId, TimeSpan availabilityDelay)
		{
			var currentTime = _dateTimeService.GetCurrentDateTime();
			var availabilityDate = currentTime + availabilityDelay;
			return RescheduleMessage(queueName, messageId, availabilityDate);
		}

		#endregion
	}
}
