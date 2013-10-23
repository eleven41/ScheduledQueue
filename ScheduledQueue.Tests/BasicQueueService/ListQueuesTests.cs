using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	[TestClass]
	public class ListQueuesTests
	{
		[TestMethod]
		public void ListQueuesEmptyList()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 0);

			// Perform
			var queues = queueService.ListQueues();
			
			// Postconditions
			Assert.AreEqual(queues.Count(), 0);
		}

		[TestMethod]
		public void ListQueuesOneQueue()
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
			var queues = queueService.ListQueues();

			// Postconditions
			Assert.AreEqual(queues.Count(), 1);
			Assert.AreEqual(queues.First(), queueName);
		}

		[TestMethod]
		public void ListQueuesTwoQueues()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			queueDataProvider.InsertQueue(queueName + "1");
			queueDataProvider.InsertQueue(queueName + "2");

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 2);

			// Perform
			var queues = queueService.ListQueues();

			// Postconditions
			Assert.AreEqual(queues.Count(), 2);
			Assert.AreEqual(queues.First(), queueName + "1");
			Assert.AreEqual(queues.Last(), queueName + "2");
		}

		[TestMethod]
		public void ListQueuesTenQueues()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			// Setup
			string queueName = "MyQueue";
			for (int i = 0; i < 10; ++i)
				queueDataProvider.InsertQueue(String.Format("{0}{1}", queueName, i));

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 10);

			// Perform
			var queues = queueService.ListQueues();

			// Postconditions
			Assert.AreEqual(queues.Count(), 10);
			for (int i = 0; i < 10; ++i)
			{
				string qn = String.Format("{0}{1}", queueName, i);
				
				Assert.IsTrue(queues.Contains(qn));
			}
		}
	}
}
