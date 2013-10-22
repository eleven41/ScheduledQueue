using System;
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
			//Test1();
			//Test2();
			Test3();
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
				var result = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(0), TimeSpan.FromSeconds(0));
				System.Diagnostics.Debug.Assert(result == null);
				System.Diagnostics.Debug.Assert(DateTime.UtcNow < receive1Date.AddSeconds(30));
			}

			{
				Console.WriteLine("Receiving message (60 second delay)");
				var result = queueService.ReceiveMessage(queueName, TimeSpan.FromSeconds(60), TimeSpan.FromSeconds(30));
				System.Diagnostics.Debug.Assert(result != null);
				System.Diagnostics.Debug.Assert(result.MessageId == messageId);
				System.Diagnostics.Debug.Assert(DateTime.UtcNow >= receive1Date.AddSeconds(30));
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

		static void Test3()
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
		}
	}

	public class TimeoutSignalService : ISignalService
	{
		public int NumTimeouts { get; set; }

		#region ISignalService Members

		public void Signal(string queueName, SignalSources source)
		{
			if (source == SignalSources.ReceiveTimeout)
			{
				lock (this)
				{
					this.NumTimeouts = this.NumTimeouts + 1;
				}
			}

			QueueSignal.Instance.Signal(queueName);
		}

		#endregion
	}
}
