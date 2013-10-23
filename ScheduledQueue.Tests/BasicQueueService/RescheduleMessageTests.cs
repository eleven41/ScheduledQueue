using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	[TestClass]
	public class RescheduleMessageTests
	{
		[TestMethod]
		public void RescheduleMessage()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);

			DateTime firstDate = dateTimeService.GetCurrentDateTime();

			queueDataProvider.InsertMessage(queueName, "abc", "Hello", firstDate);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			DateTime newAvailabilityDate = dateTimeService.GetCurrentDateTime().AddSeconds(30);

			// Perform
			var result = queueService.RescheduleMessage(queueName, "abc", newAvailabilityDate);

			// Postconditions

			// Validate the returned information
			Assert.IsNotNull(result);
			Assert.AreEqual(newAvailabilityDate, result.MessageDate);
			Assert.AreNotEqual("abc", result.NewMessageId);

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			// Verify the message data in the queue for the new message is the same
			// as what was sent and what was returned.
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, 0), "Hello");
			Assert.AreEqual(queueDataProvider.GetMessageId(queueName, 0), result.NewMessageId);
			Assert.AreEqual(queueDataProvider.GetMessageDate(queueName, 0), result.MessageDate);
		}

		[TestMethod]
		[ExpectedException(typeof(QueueNotFoundException))]
		public void RescheduleMessageWithQueueThatDoesNotExist()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);

			DateTime firstDate = dateTimeService.GetCurrentDateTime();

			queueDataProvider.InsertMessage(queueName, "abc", "Hello", firstDate);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			string messageId = queueDataProvider.GetMessageId(queueName, 0);

			// Perform
			queueService.RescheduleMessage("badqueue", "abc", dateTimeService.GetCurrentDateTime().AddSeconds(30));
		}

		[TestMethod]
		[ExpectedException(typeof(MessageNotFoundException))]
		public void RescheduleMessageWithMessageThatDoesNotExist()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 0);

			// Perform
			queueService.RescheduleMessage(queueName, "abc", dateTimeService.GetCurrentDateTime().AddSeconds(30));
		}
	}
}
