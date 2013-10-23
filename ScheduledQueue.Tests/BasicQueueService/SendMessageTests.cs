using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	[TestClass]
	public class SendMessageTests
	{
		[TestMethod]
		public void SendMessage()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);
			DateTime beforeDate = dateTimeService.GetCurrentDateTime();

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 0);

			// Perform
			var result = queueService.SendMessage(queueName, "Hello");
			
			// Postconditions
			
			// Validate the returned information
			Assert.IsNotNull(result);
			Assert.IsTrue(beforeDate <= result.MessageDate);
			Assert.IsTrue(result.MessageDate <= dateTimeService.GetCurrentDateTime());

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			// Verify the message data in the queue for the new message is the same
			// as what was sent and what was returned.
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, 0), "Hello");
			Assert.AreEqual(queueDataProvider.GetMessageId(queueName, 0), result.MessageId);
			Assert.AreEqual(queueDataProvider.GetMessageDate(queueName, 0), result.MessageDate);
		}

		[TestMethod]
		public void SendMessageWithAvailabilityDate()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);
			DateTime beforeDate = dateTimeService.GetCurrentDateTime();

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 0);

			DateTime availabilityDate = dateTimeService.GetCurrentDateTime().AddSeconds(30);
			
			// Perform
			var result = queueService.SendMessage(queueName, "Hello", availabilityDate);

			// Postconditions

			// Validate the returned information
			Assert.IsNotNull(result);
			Assert.AreEqual(availabilityDate, result.MessageDate);
			
			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			// Verify the message data in the queue for the new message is the same
			// as what was sent and what was returned.
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, 0), "Hello");
			Assert.AreEqual(queueDataProvider.GetMessageId(queueName, 0), result.MessageId);
			Assert.AreEqual(queueDataProvider.GetMessageDate(queueName, 0), result.MessageDate);
		}

		[TestMethod]
		public void SendMessageWithDelay()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);
			DateTime beforeDate = dateTimeService.GetCurrentDateTime();

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 0);

			TimeSpan delay = TimeSpan.FromSeconds(30);
			
			// Perform
			var result = queueService.SendMessage(queueName, "Hello", delay);

			// Postconditions

			DateTime afterDate = dateTimeService.GetCurrentDateTime();

			// Validate the returned information
			Assert.IsNotNull(result);
			Assert.IsTrue(beforeDate + delay <= result.MessageDate);
			Assert.IsTrue(result.MessageDate <= afterDate + delay);

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			// Verify the message data in the queue for the new message is the same
			// as what was sent and what was returned.
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, 0), "Hello");
			Assert.AreEqual(queueDataProvider.GetMessageId(queueName, 0), result.MessageId);
			Assert.AreEqual(queueDataProvider.GetMessageDate(queueName, 0), result.MessageDate);
		}

		[TestMethod]
		public void SendMessageAnotherMessage()
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

			DateTime beforeDate = dateTimeService.GetCurrentDateTime();

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, 0), "Hello");
			Assert.AreEqual(queueDataProvider.GetMessageId(queueName, 0), "abc");

			// Perform
			var result = queueService.SendMessage(queueName, "Hello2");

			// Postconditions

			// Validate the returned information
			Assert.IsNotNull(result);
			Assert.IsTrue(beforeDate <= result.MessageDate);
			Assert.IsTrue(result.MessageDate <= dateTimeService.GetCurrentDateTime());
			Assert.AreNotEqual(result.MessageId, "abc");

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 2);
			
			int originalMessageIndex = queueDataProvider.FindMessageFromId(queueName, "abc");
			int newMessageIndex = queueDataProvider.FindMessageFromId(queueName, result.MessageId);
			Assert.AreNotEqual(originalMessageIndex, newMessageIndex);

			// Make sure the original message was not modified
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, originalMessageIndex), "Hello");
			Assert.IsTrue(queueDataProvider.GetMessageDate(queueName, originalMessageIndex) <= queueDataProvider.GetMessageDate(queueName, newMessageIndex));
			Assert.AreEqual(queueDataProvider.GetMessageDate(queueName, originalMessageIndex), firstDate);

			// Verify the message data in the queue for the new message is the same
			// as what was sent and what was returned.
			Assert.AreEqual(queueDataProvider.GetMessageBody(queueName, newMessageIndex), "Hello2");
			Assert.AreEqual(queueDataProvider.GetMessageId(queueName, newMessageIndex), result.MessageId);
			Assert.AreEqual(queueDataProvider.GetMessageDate(queueName, newMessageIndex), result.MessageDate);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void SendMessageWithEmptyQueueName()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);
			
			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			
			// Perform
			var result = queueService.SendMessage("", "Hello");
		}

		[TestMethod]
		[ExpectedException(typeof(QueueDoesNotExistException))]
		public void SendMessageWithQueueThatDoesNotExist()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);

			// Perform
			var result = queueService.SendMessage("MyQueue2", "Hello");
		}
	}
}
