using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ScheduledQueue.Core;

namespace ScheduledQueue.Tests.BasicQueueServiceTests
{
	/// <summary>
	/// Summary description for DeleteQueue
	/// </summary>
	[TestClass]
	public class DeleteQueueTests
	{
		[TestMethod]
		public void DeleteQueue()
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

			// Perform
			queueService.DeleteQueue(queueName);
			
			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 0);
		}

		[TestMethod]
		public void DeleteQueueThatDoesNotExists()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 0);

			// Perform
			queueService.DeleteQueue(queueName);
			
			// Postconditions
			var queues = queueDataProvider.GetQueues();
			Assert.AreEqual(queues.Count(), 0);
		}

		[TestMethod]
		[ExpectedException(typeof(ArgumentNullException))]
		public void DeleteQueueWithEmptyName()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var queueDataProvider = new InProcQueueDataProvider();
			var queueService = new BasicQueueService(queueDataProvider, dateTimeService, signalService);

			string queueName = "";

			// Preconditions
			Assert.IsTrue(queueDataProvider.GetQueues().Count() == 0);

			// Perform
			queueService.DeleteQueue(queueName);
		}
	}
}
