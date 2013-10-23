using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;


namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	[TestClass]
	public class DeleteMessageTests
	{
		[TestMethod]
		public void DeleteMessage()
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
			queueService.DeleteMessage(queueName, messageId);

			// Postconditions

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 0);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteMessageWithEmptyQueueName()
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
			queueService.DeleteMessage("", "abc");
		}

		[TestMethod]
		[ExpectedException(typeof(QueueDoesNotExistException))]
		public void DeleteMessageWithQueueThatDoesNotExist()
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
			queueService.DeleteMessage("badqueue", messageId);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteMessageWithEmptyMessageId()
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
			queueService.DeleteMessage(queueName, "");
		}

		[TestMethod]
		public void DeleteMessageWithMessageThatDoesNotExist()
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
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1, "Pre: Incorrect number of queues.");
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1, "Pre: Incorrect number of messages.");

			// Perform
			queueService.DeleteMessage(queueName, "abcd");

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1, "Post: Incorrect number of queues.");
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1, "Post: Incorrect number of messages.");
		}
	}
}
