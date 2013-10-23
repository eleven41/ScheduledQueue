using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	[TestClass]
	public class ReceiveMessageTests
	{
		[TestMethod]
		public void ReceiveMessage()
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

			// Perform
			var message = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(10));

			// Postconditions

			DateTime afterDate = dateTimeService.GetCurrentDateTime();
			
			// Validate the result
			Assert.IsNotNull(message);
			Assert.AreEqual(message.MessageBody, "Hello", "Post: Message body is incorrect.");
			Assert.AreEqual(message.MessageId, "abc", "Post: Message ID is incorrect.");

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, afterDate) == 0);
		}

		[TestMethod]
		public void ReceiveMessageWithDelayedMessageWithinWindow()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);

			DateTime firstDate = dateTimeService.GetCurrentDateTime();

			queueDataProvider.InsertMessage(queueName, "abc", "Hello", firstDate.AddSeconds(5));

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);

			// Perform
			var message = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(10), TimeSpan.FromSeconds(10));

			// Postconditions

			DateTime afterDate = dateTimeService.GetCurrentDateTime();

			// Validate the result
			Assert.IsNotNull(message);
			Assert.AreEqual(message.MessageBody, "Hello", "Post: Message body is incorrect.");
			Assert.AreEqual(message.MessageId, "abc", "Post: Message ID is incorrect.");

			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, afterDate) == 0);
		}

		[TestMethod]
		public void ReceiveMessageWithDelayedMessageOutOfWindow()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new TestQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName);

			DateTime firstDate = dateTimeService.GetCurrentDateTime();

			queueDataProvider.InsertMessage(queueName, "abc", "Hello", firstDate.AddSeconds(10));

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, firstDate) == 0);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, firstDate.AddSeconds(10)) == 1);

			// Perform
			var message = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(10));

			// Postconditions

			// Validate the result
			Assert.IsNull(message);
			
			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, firstDate) == 0);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, firstDate.AddSeconds(10)) == 1);
		}

		[TestMethod]
		public void ReceiveMessageWithLaterSend()
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

			var beforeDate = dateTimeService.GetCurrentDateTime();

			// Perform

			// Start a sender that will send in 5 seconds
			Action d = new Action(() =>
				{
					// Delay for 5 seconds
					System.Threading.Thread.Sleep(5*1000);

					queueService.SendMessage(queueName, "Hello");
				});
			d.BeginInvoke(null, null);

			// Start waiting for a limit of 15 seconds
			var message = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(15), TimeSpan.FromSeconds(10));

			// Postconditions

			var afterDate = dateTimeService.GetCurrentDateTime();

			// Validate the result
			Assert.IsNotNull(message);
			Assert.AreEqual(message.MessageBody, "Hello", "Post: Message body is incorrect.");

			// Validate time taken
			Assert.IsTrue(afterDate < beforeDate.AddSeconds(15));
			
			// Validate the queue state
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);
			Assert.IsTrue(queueDataProvider.NumTotalMessages(queueName) == 1);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, afterDate) == 0);
			Assert.IsTrue(queueDataProvider.NumAvailableMessages(queueName, afterDate.AddSeconds(10)) == 1);
		}

		private delegate void SendMessageDelegate(IQueueService queueService);
	}
}
