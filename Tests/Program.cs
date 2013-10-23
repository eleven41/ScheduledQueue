﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduledQueue.Core;

namespace Tests
{
	class Program
	{
		static void Main(string[] args)
		{
			BasicQueueServiceTests();

			//Test1();
			//Test2();
		}

		private static void BasicQueueServiceTests()
		{
			CreateQueue1();
			CreateQueue2();
			CreateQueue3();
			CreateQueue4();
			CreateQueue5();
		}

		private static void CreateQueue1()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Preconditions
			System.Diagnostics.Debug.Assert(dataStorage.GetQueues().Count() == 0);

			{
				Console.WriteLine("Creating queue '{0}'", queueName);
				string newQueueName = queueService.CreateQueue(queueName);
				System.Diagnostics.Debug.Assert(queueName == newQueueName);
			}

			// Postconditions
			var queues = dataStorage.GetQueues();
			System.Diagnostics.Debug.Assert(queues.Count() == 1);
			System.Diagnostics.Debug.Assert(queues.Single() == queueName);
		}

		private static void CreateQueue2()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Setup
			dataStorage.InsertQueue(queueName);

			// Preconditions
			System.Diagnostics.Debug.Assert(dataStorage.GetQueues().Count() == 1);

			{
				Console.WriteLine("Creating queue '{0}'", queueName);
				string newQueueName = queueService.CreateQueue(queueName);
				System.Diagnostics.Debug.Assert(queueName == newQueueName);
			}

			// Postconditions
			var queues = dataStorage.GetQueues();
			System.Diagnostics.Debug.Assert(queues.Count() == 1);
			System.Diagnostics.Debug.Assert(queues.Single() == queueName);
		}

		private static void CreateQueue3()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Setup
			dataStorage.InsertQueue(queueName);

			// Preconditions
			System.Diagnostics.Debug.Assert(dataStorage.GetQueues().Count() == 1);

			{
				queueName = "myqueue";
				Console.WriteLine("Creating queue '{0}'", queueName);
				string newQueueName = queueService.CreateQueue(queueName);
				System.Diagnostics.Debug.Assert(queueName == newQueueName);
			}

			// Postconditions
			var queues = dataStorage.GetQueues();
			System.Diagnostics.Debug.Assert(queues.Count() == 1);
			System.Diagnostics.Debug.Assert(queues.Single() == "MyQueue");
		}

		private static void CreateQueue4()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "MyQueue";

			// Setup
			dataStorage.InsertQueue(queueName);

			// Preconditions
			System.Diagnostics.Debug.Assert(dataStorage.GetQueues().Count() == 1);

			{
				queueName = "myqueue2";
				Console.WriteLine("Creating queue '{0}'", queueName);
				string newQueueName = queueService.CreateQueue(queueName);
				System.Diagnostics.Debug.Assert(queueName == newQueueName);
			}

			// Postconditions
			var queues = dataStorage.GetQueues();
			System.Diagnostics.Debug.Assert(queues.Count() == 2);
			System.Diagnostics.Debug.Assert(queues.First() == "MyQueue");
			System.Diagnostics.Debug.Assert(queues.Last() == "myqueue2");
		}

		private static void CreateQueue5()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "";

			// Preconditions
			System.Diagnostics.Debug.Assert(dataStorage.GetQueues().Count() == 0);

			{
				Console.WriteLine("Creating queue '{0}'", queueName);
				try
				{
					queueService.CreateQueue(queueName);

					// Should not get here
					System.Diagnostics.Debug.Assert(false);
				}
				catch
				{
					// Correct path
				}
			}

			// Postconditions
			var queues = dataStorage.GetQueues();
			System.Diagnostics.Debug.Assert(queues.Count() == 0);
		}


		static void Test1()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new InProcSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "MyQueue";

			{
				Console.WriteLine("Creating queue '{0}'", queueName);
				queueService.CreateQueue(queueName);
			}

			DateTime messageDate;
			string messageId;
			{
				Console.WriteLine("Sending message 'Hello'");

				var result = queueService.SendMessage(queueName, "Hello");

				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(result.MessageId));
				System.Diagnostics.Debug.Assert(result.MessageDate <= DateTime.UtcNow);
				messageDate = result.MessageDate;
				messageId = result.MessageId;
			}

			DateTime receive1Date = DateTime.UtcNow;
			{
				Console.WriteLine("Receiving message (no delay)");

				var result = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));
				System.Diagnostics.Debug.Assert(!String.IsNullOrEmpty(result.MessageId));
				System.Diagnostics.Debug.Assert(result.MessageBody == "Hello");
				System.Diagnostics.Debug.Assert(result.MessageDate == messageDate);
			}

			{
				Console.WriteLine("Receiving message (no delay)");
				var result = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(30));
				System.Diagnostics.Debug.Assert(result == null);
				System.Diagnostics.Debug.Assert(DateTime.UtcNow < receive1Date.AddSeconds(30));
			}

			// Wait 15 seconds
			Console.WriteLine("Wait 15 seconds");
			System.Threading.Thread.Sleep(15 * 1000);

			{
				Console.WriteLine("Receiving message (60 second delay)");
				var result = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));
				System.Diagnostics.Debug.Assert(result != null);
				System.Diagnostics.Debug.Assert(result.MessageId == messageId);
				System.Diagnostics.Debug.Assert(DateTime.UtcNow >= receive1Date.AddSeconds(30));
			}

			{
				Console.WriteLine("Deleting message");
				queueService.DeleteMessage(queueName, messageId);
			}

			DateTime receive2Date = DateTime.UtcNow;
			{
				Console.WriteLine("Receiving message (60 second delay)");
				var result = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));
				System.Diagnostics.Debug.Assert(result == null);
				System.Diagnostics.Debug.Assert(DateTime.UtcNow >= receive2Date.AddSeconds(60));
			}
		}

		static void Test2()
		{
			var dateTimeService = new InProcDateTimeService();
			var signalService = new TimeoutSignalService();
			var dataStorage = new InProcDataStorage();
			var queueService = new BasicQueueService(dataStorage, dateTimeService, signalService);

			string queueName = "MyQueue";

			{
				Console.WriteLine("Creating queue '{0}'", queueName);
				queueService.CreateQueue(queueName);
			}

			// Start a consumer thread
			System.Threading.Thread t = new System.Threading.Thread((e) =>
				{
					// Start producing for 10 seconds
					DateTime start = DateTime.Now;
					DateTime end = start.AddSeconds(10);
					int count = 0;
					while (DateTime.Now < end)
					{
						queueService.SendMessage(queueName, String.Format("Hello {0}", count++));
					}

					Console.WriteLine("Done producing {0} items", count);
				});
			t.Start();

			DateTime consumeStart = DateTime.Now;
			while (true)
			{
				TimeSpan ts = DateTime.Now - consumeStart;
				Console.WriteLine("[{0}] Receiving message...", ts.TotalSeconds);
				var message = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(5), TimeSpan.FromSeconds(30));
				if (message != null)
				{
					Console.WriteLine("Message received: " + message.MessageBody);
					queueService.DeleteMessage(queueName, message.MessageId);
				}
				else
				{
					break;
				}
			}

			System.Diagnostics.Debug.Assert(signalService.NumTimeouts == 0);
		}
	}

	public class TimeoutSignalService : InProcSignalService
	{
		public int NumTimeouts { get; set; }

		#region ISignalService Members

		public override void Signal(string queueName, SignalSources source)
		{
			if (source == SignalSources.ReceiveTimeout)
			{
				lock (this)
				{
					this.NumTimeouts = this.NumTimeouts + 1;
				}
			}

			base.Signal(queueName, source);
		}

		#endregion
	}
}
