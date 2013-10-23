using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class BasicQueueService : IQueueService
	{
		IQueueDataProvider _queueDataProvider;
		IDateTimeService _dateTimeService;
		ISignalService _signalService;

		public BasicQueueService(IQueueDataProvider queueDataProvider, IDateTimeService dateTimeService, ISignalService signalService)
		{
			_queueDataProvider = queueDataProvider;
			_dateTimeService = dateTimeService;
			_signalService = signalService;
		}

		#region IQueueService Members

		public IEnumerable<string> ListQueues()
		{
			return _queueDataProvider.GetQueues();
		}

		public string CreateQueue(string queueName)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();

			_queueDataProvider.InsertQueue(queueName);
			return queueName;
		}

		public void DeleteQueue(string queueName)
		{
			if (String.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");

			_queueDataProvider.DeleteQueue(queueName);
		}

		public SendMessageResult SendMessage(string queueName, string messageBody)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();

			var availabilityDate = _dateTimeService.GetCurrentDateTime();
			return SendMessage(queueName, messageBody, availabilityDate);
		}

		public SendMessageResult SendMessage(string queueName, string messageBody, DateTime availabilityDate)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();

			string messageId = GenerateMessageId();
			_queueDataProvider.InsertMessage(queueName, messageId, messageBody, availabilityDate);

			// Notify any waiting receivers
			_signalService.Signal(queueName, SignalSources.SendMessage);

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
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();

			var currentTime = _dateTimeService.GetCurrentDateTime();
			var availabilityDate = currentTime + availabilityDelay;
			return SendMessage(queueName, messageBody, availabilityDate);
		}

		public ReceiveMessageResult ReceiveMessage(string queueName, TimeSpan receiveTimeout, TimeSpan visibilityTimeout)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();

			var currentTime = _dateTimeService.GetCurrentDateTime();
			var newAvailabilityDate = currentTime + visibilityTimeout;

			// Check to see if any backlogged messages exist
			var message = _queueDataProvider.GetMessage(queueName, currentTime, newAvailabilityDate);
			if (message != null)
			{
				return new ReceiveMessageResult()
				{
					MessageBody = message.MessageBody,
					MessageId = message.MessageId,
					MessageDate = message.AvailabilityDate
				};
			}

			var endTime = currentTime + receiveTimeout;
			currentTime = _dateTimeService.GetCurrentDateTime();
			while (currentTime <= endTime)
			{
				TimeSpan delay = endTime - currentTime;

				// Will any messages be available within our receive window?
				DateTime? nextMessage = _queueDataProvider.PeekMessage(queueName, endTime);
				if (nextMessage.HasValue)
				{
					delay = nextMessage.Value - currentTime;
				}
			
				// Wait for the receive timeout period to see if anything new comes
				_signalService.Wait(queueName, delay);

				currentTime = _dateTimeService.GetCurrentDateTime();

				// Try again
				newAvailabilityDate = currentTime + visibilityTimeout;
				message = _queueDataProvider.GetMessage(queueName, currentTime, newAvailabilityDate);
				if (message != null)
				{
					return new ReceiveMessageResult()
					{
						MessageBody = message.MessageBody,
						MessageId = message.MessageId,
						MessageDate = message.AvailabilityDate
					};
				}
			}

			return null;
		}

		public void DeleteMessage(string queueName, string messageId)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();
			if (String.IsNullOrWhiteSpace(messageId))
				throw new ArgumentNullException("messageId");
			messageId = messageId.Trim();

			_queueDataProvider.DeleteMessage(queueName, messageId);
		}

		public RescheduleMessageResult RescheduleMessage(string queueName, string messageId, DateTime availabilityDate)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();
			if (String.IsNullOrWhiteSpace(messageId))
				throw new ArgumentNullException("messageId");
			messageId = messageId.Trim();

			string newMessageId = GenerateMessageId();
			_queueDataProvider.UpdateMessage(queueName, messageId, newMessageId, availabilityDate);

			// Notify any listeners
			_signalService.Signal(queueName, SignalSources.RescheduleMessage);

			return new RescheduleMessageResult()
			{
				NewMessageId = newMessageId,
				MessageDate = availabilityDate
			};
		}

		public RescheduleMessageResult RescheduleMessage(string queueName, string messageId, TimeSpan availabilityDelay)
		{
			if (String.IsNullOrWhiteSpace(queueName))
				throw new ArgumentNullException("queueName");
			queueName = queueName.Trim();
			if (String.IsNullOrWhiteSpace(messageId))
				throw new ArgumentNullException("messageId");
			messageId = messageId.Trim();

			var currentTime = _dateTimeService.GetCurrentDateTime();
			var availabilityDate = currentTime + availabilityDelay;
			return RescheduleMessage(queueName, messageId, availabilityDate);
		}

		#endregion
	}
}
