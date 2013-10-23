using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	[TestClass]
	public class CreateQueueTests
	{
		[TestMethod]
		public void CreateQueue()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 0);

			{
				string newQueueName = queueService.CreateQueue(queueName);
				Assert.AreEqual(queueName, newQueueName);
			}

			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 1);
			Assert.AreEqual(queues.Single(), queueName);
		}

		[TestMethod]
		public void CreateQueueWithNameTrimmed()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "  MyQueue  ";

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 0);

			{
				string newQueueName = queueService.CreateQueue(queueName);
				Assert.AreEqual(queueName.Trim(), newQueueName);
			}

			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 1);
			Assert.AreEqual(queues.Single(), queueName.Trim());
		}

		[TestMethod]
		public void CreateQueueWithDuplicateName()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Setup
			queueDataProvider.InsertQueue(queueName);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);

			{
				string newQueueName = queueService.CreateQueue(queueName);
				Assert.AreEqual(queueName, newQueueName);
			}

			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 1);
			Assert.AreEqual(queues.Single(), queueName);
		}

		[TestMethod]
		public void CreateQueueWithDuplicateNameDifferentCase()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Setup
			queueDataProvider.InsertQueue(queueName);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);

			{
				queueName = "myqueue";
				string newQueueName = queueService.CreateQueue(queueName);
				Assert.AreEqual(queueName, newQueueName);
			}

			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 1);
			Assert.AreEqual(queues.Single(), "MyQueue");
		}

		[TestMethod]
		public void CreateQueueWithDifferentName()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Setup
			queueDataProvider.InsertQueue(queueName);

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 1);

			{
				queueName = "myqueue2";
				string newQueueName = queueService.CreateQueue(queueName);
				Assert.AreEqual(queueName, newQueueName);
			}

			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 2);
			Assert.AreEqual(queues.First(), "MyQueue");
			Assert.AreEqual(queues.Last(), "myqueue2");
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void CreateQueueWithEmptyName()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "";

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 0);

			var newQueueName = queueService.CreateQueue(queueName);
		}
	}
}
