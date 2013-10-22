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
		IDataStorage _dataStorage;
		IDateTimeService _dateTimeService;
		ISignalService _signalService;

		QueueSignal _queueSignal;

		public BasicQueueService(IDataStorage dataStorage, IDateTimeService dateTimeService, ISignalService signalService)
		{
			_dataStorage = dataStorage;
			_dateTimeService = dateTimeService;
			_signalService = signalService;
			_queueSignal = QueueSignal.Instance;
		}

		#region IQueueService Members

		public IEnumerable<string> ListQueues()
		{
			return _dataStorage.GetQueues();
		}

		public string CreateQueue(string queueName)
		{
			if (String.IsNullOrEmpty(queueName))
				throw new ArgumentNullException("queueName");

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
			var currentTime = _dateTimeService.GetCurrentDateTime();
			var availabilityDate = currentTime + availabilityDelay;
			return SendMessage(queueName, messageBody, availabilityDate);
		}

		static ConcurrentDictionary<string, System.Threading.Timer> _visibilityTimers = new ConcurrentDictionary<string, System.Threading.Timer>();

		private void AddVisibilitySignal(string queueName, string messageId, TimeSpan visibilityTimeout)
		{
			// Create a timer, but don't start it yet
			System.Threading.Timer t = new System.Threading.Timer((e) =>
			{
				_signalService.Signal(queueName, SignalSources.ReceiveTimeout);
			}, null, TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));

			// Add it to our list of timers
			_visibilityTimers.TryAdd(messageId, t);

			// Get the timer going
			t.Change(visibilityTimeout, TimeSpan.FromMilliseconds(-1));
		}

		private void CancelVisibilitySignal(string queueName, string messageId)
		{
			// Find the right timer
			System.Threading.Timer t;
			if (_visibilityTimers.TryRemove(messageId, out t))
			{
				// Stop the timer
				t.Change(TimeSpan.FromMilliseconds(-1), TimeSpan.FromMilliseconds(-1));
			}
		}

		public ReceiveMessageResult ReceiveMessage(string queueName, TimeSpan receiveTimeout, TimeSpan visibilityTimeout)
		{
			var currentTime = _dateTimeService.GetCurrentDateTime();
			var newAvailabilityDate = currentTime + visibilityTimeout;
			var message = _dataStorage.GetMessage(queueName, currentTime, newAvailabilityDate);
			if (message != null)
			{
				AddVisibilitySignal(queueName, message.MessageId, visibilityTimeout);

				return new ReceiveMessageResult()
				{
					MessageBody = message.MessageBody,
					MessageId = message.MessageId,
					MessageDate = message.AvailabilityDate
				};
			}

			var endTime = currentTime + receiveTimeout;
			while (currentTime < endTime)
			{
				// No message was found, wait for the receive timeout period to see if anything new comes
				TimeSpan delay = endTime - currentTime;
				if (!_queueSignal.Wait(queueName, delay))
					break;

				currentTime = _dateTimeService.GetCurrentDateTime();

				// Try again
				newAvailabilityDate = currentTime + visibilityTimeout;
				message = _dataStorage.GetMessage(queueName, currentTime, newAvailabilityDate);
				if (message != null)
				{
					AddVisibilitySignal(queueName, message.MessageId, visibilityTimeout);

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
			CancelVisibilitySignal(queueName, messageId);

			_dataStorage.DeleteMessage(queueName, messageId);
		}

		public RescheduleMessageResult RescheduleMessage(string queueName, string messageId, DateTime availabilityDate)
		{
			CancelVisibilitySignal(queueName, messageId);

			string newMessageId = GenerateMessageId();
			_dataStorage.UpdateMessage(queueName, messageId, newMessageId, availabilityDate);

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
			var currentTime = _dateTimeService.GetCurrentDateTime();
			var availabilityDate = currentTime + availabilityDelay;
			return RescheduleMessage(queueName, messageId, availabilityDate);
		}

		#endregion
	}
}
