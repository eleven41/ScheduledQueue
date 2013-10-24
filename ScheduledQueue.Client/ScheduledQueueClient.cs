using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ScheduledQueue.Client
{
    public class ScheduledQueueClient
    {
		public ScheduledQueueClient()
		{
			this.Endpoint = new Uri("http://localhost/ScheduledQueue");
		}

		public ScheduledQueueClient(Uri endpoint)
		{
			this.Endpoint = endpoint;
		}

		public Uri Endpoint { get; set; }

		#region Queues

		public ListQueuesResponse ListQueues(ListQueuesRequest request)
		{
			var task = ListQueuesAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<ListQueuesResponse> ListQueuesAsync(ListQueuesRequest request)
		{
			return InvokeAsync<ListQueuesRequest, ListQueuesResponse>("ListQueues", request);
		}

		public CreateQueueResponse CreateQueue(CreateQueueRequest request)
		{
			var task = CreateQueueAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<CreateQueueResponse> CreateQueueAsync(CreateQueueRequest request)
		{
			return InvokeAsync<CreateQueueRequest, CreateQueueResponse>("CreateQueue", request);
		}

		public DeleteQueueResponse DeleteQueue(DeleteQueueRequest request)
		{
			var task = DeleteQueueAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<DeleteQueueResponse> DeleteQueueAsync(DeleteQueueRequest request)
		{
			return InvokeAsync<DeleteQueueRequest, DeleteQueueResponse>("DeleteQueue", request);
		}

		#endregion

		#region Messages

		public SendMessageResponse SendMessage(SendMessageRequest request)
		{
			var task = SendMessageAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<SendMessageResponse> SendMessageAsync(SendMessageRequest request)
		{
			return InvokeAsync<SendMessageRequest, SendMessageResponse>("SendMessage", request);
		}

		public ReceiveMessageResponse ReceiveMessage(ReceiveMessageRequest request)
		{
			var task = ReceiveMessageAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<ReceiveMessageResponse> ReceiveMessageAsync(ReceiveMessageRequest request)
		{
			return InvokeAsync<ReceiveMessageRequest, ReceiveMessageResponse>("ReceiveMessage", request);
		}

		public DeleteMessageResponse DeleteMessage(DeleteMessageRequest request)
		{
			var task = DeleteMessageAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<DeleteMessageResponse> DeleteMessageAsync(DeleteMessageRequest request)
		{
			return InvokeAsync<DeleteMessageRequest, DeleteMessageResponse>("DeleteMessage", request);
		}

		public RescheduleMessageResponse RescheduleMessage(RescheduleMessageRequest request)
		{
			var task = RescheduleMessageAsync(request);
			task.Wait();
			return task.Result;
		}

		public Task<RescheduleMessageResponse> RescheduleMessageAsync(RescheduleMessageRequest request)
		{
			return InvokeAsync<RescheduleMessageRequest, RescheduleMessageResponse>("RescheduleMessage", request);
		}

		#endregion

		#region Internal Helpers

		private async Task<ResponseT> InvokeAsync<RequestT, ResponseT>(string action, RequestT request) 
			where ResponseT : BasicResponse
		{
			// Build our action URI
			Uri actionUri = GetActionUri(action);

			// Serialize the request into json
			string postData = SerializeJson<RequestT>(request);
			StringContent theContent = new StringContent(postData, System.Text.Encoding.UTF8, "application/json"); 

			HttpClient client = new HttpClient();
			HttpResponseMessage httpResponse = await client.PostAsync(actionUri, theContent)
				.ConfigureAwait(false);
			
			if (!httpResponse.IsSuccessStatusCode)
				throw new Exception(httpResponse.ReasonPhrase);
			
			// Deserialize the response json
			string responseString = await httpResponse.Content.ReadAsStringAsync()
				.ConfigureAwait(false);
			var response = DeserializeJson<ResponseT>(responseString);

			if (response.Errors != null)
			{
				if (response.Errors.Count() > 0)
				{
					var first = response.Errors.First();
					throw new ScheduledQueueException(first.Key, first.Message);
				}
			}

			return response;
		}

		private Uri GetActionUri(string action)
		{
			UriBuilder ub = new UriBuilder(this.Endpoint);

			// Append action to our endpoint path
			string path = ub.Path;
			if (String.IsNullOrEmpty(path))
				path = "/" + action;
			else if (path.Last() == '/')
				path = path + action;
			else
				path = path + "/" + action;
			ub.Path = path;
			return ub.Uri;
		}

		private static ResponseT DeserializeJson<ResponseT>(string responseString)
		{
			Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
			var jsonSerializer = Newtonsoft.Json.JsonSerializer.Create(settings);
			var textReader = new System.IO.StringReader(responseString);
			var jsonReader = new Newtonsoft.Json.JsonTextReader(textReader);
			var response = jsonSerializer.Deserialize<ResponseT>(jsonReader);
			return response;
		}

		private static string SerializeJson<RequestT>(RequestT request)
		{
			Newtonsoft.Json.JsonSerializerSettings settings = new Newtonsoft.Json.JsonSerializerSettings();
			var jsonSerializer = Newtonsoft.Json.JsonSerializer.Create(settings);
			var textWriter = new System.IO.StringWriter();
			var jsonWriter = new Newtonsoft.Json.JsonTextWriter(textWriter);
			jsonSerializer.Serialize(jsonWriter, request);
			return textWriter.ToString();
		}

		#endregion
	}

	public class BasicResponse
	{
		internal List<ErrorItem> Errors { get; set; }
	}

	public class ErrorItem
	{
		public string Key { get; set; }
		public string Message { get; set; }
	}

	public class ListQueuesRequest
	{
	}

	public class ListQueuesResponse : BasicResponse
	{
		public List<string> Queues { get; set; }
	}

	public class CreateQueueRequest
	{
		public string QueueName { get; set; }
	}

	public class CreateQueueResponse : BasicResponse
	{
		public string QueueName { get; set; }
	}

	public class DeleteQueueRequest
	{
		public string QueueName { get; set; }
	}

	public class DeleteQueueResponse : BasicResponse
	{
	}

	public class SendMessageRequest
	{
		public string QueueName { get; set; }
		public string MessageBody { get; set; }

		public string Date { get; set; }
		public double? Delay { get; set; }
	}

	public class SendMessageResponse : BasicResponse
	{
		public string QueueName { get; set; }
		public string MessageId { get; set; }
		public string Date { get; set; }
	}

	public class ReceiveMessageRequest
	{
		public string QueueName { get; set; }
		public double? ReceiveTimeout { get; set; }
		public double? VisibilityTimeout { get; set; }
	}

	public class ReceiveMessageResponse : BasicResponse
	{
		public string MessageBody { get; set; }
		public string Date { get; set; }
		public string MessageId { get; set; }
	}

	public class DeleteMessageRequest
	{
		public string QueueName { get; set; }
		public string MessageId { get; set; }
	}

	public class DeleteMessageResponse : BasicResponse
	{
	}

	public class RescheduleMessageRequest
	{
		public string QueueName { get; set; }
		public string MessageId { get; set; }
		public string Date { get; set; }
		public double? Delay { get; set; }
	}

	public class RescheduleMessageResponse : BasicResponse
	{
		public string QueueName { get; set; }
		public string MessageId { get; set; }
		public string Date { get; set; }
	}
}
