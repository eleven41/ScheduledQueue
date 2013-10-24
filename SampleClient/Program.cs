using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScheduledQueue.Client;

namespace SampleClient
{
	class Program
	{
		static void Main(string[] args)
		{
			ScheduledQueueClient client = new ScheduledQueueClient(new Uri("http://localhost:52823"));
			
			{
				Console.WriteLine("Create queue");

				var request = new CreateQueueRequest();
				request.QueueName = "MyQueue";
				var response = client.CreateQueue(request);
				System.Diagnostics.Debug.Assert(response.QueueName == request.QueueName);
			}

			{
				Console.WriteLine("List queues");

				var request = new ListQueuesRequest();
				var response = client.ListQueues(request);
				
				foreach (var queue in response.Queues)
				{
					Console.WriteLine("{0}", queue);
				}
			}

			string messageId;
			{
				Console.WriteLine("Send message");

				var request = new SendMessageRequest();
				request.QueueName = "MyQueue";
				request.MessageBody = "Hello";
				var response = client.SendMessage(request);
				Console.WriteLine("Message ID: {0}", response.MessageId);

				messageId = response.MessageId;
			}

			{
				Console.WriteLine("Receive message");

				var request = new ReceiveMessageRequest();
				request.QueueName = "MyQueue";
				var response = client.ReceiveMessage(request);
				Console.WriteLine("Message Body: {0}", response.MessageBody);

				System.Diagnostics.Debug.Assert(response.MessageId == messageId);
			}

			{
				Console.WriteLine("Reschedule message");

				var request = new RescheduleMessageRequest();
				request.QueueName = "MyQueue";
				request.MessageId = messageId;
				request.Delay = 10;
				var response = client.RescheduleMessage(request);
				messageId = response.MessageId;
			}

			{
				Console.WriteLine("Receive message");

				var request = new ReceiveMessageRequest();
				request.QueueName = "MyQueue";
				request.ReceiveTimeout = 20;
				var response = client.ReceiveMessage(request);
				Console.WriteLine("Message Body: {0}", response.MessageBody);

				System.Diagnostics.Debug.Assert(response.MessageId == messageId);
			}

			{
				Console.WriteLine("Delete message");

				var request = new DeleteMessageRequest();
				request.QueueName = "MyQueue";
				request.MessageId = messageId;
				var response = client.DeleteMessage(request);
			}
		}
	}
}
