using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduledQueue.Api.Models
{
	public class SendMessageRequestModel
	{
		public string QueueName { get; set; }
		public string Data { get; set; }
		public string Date { get; set; }
	}

	public class SendMessageResponseModel : BasicResponseModel
	{
		public string QueueName { get; set; }
		public string ItemId { get; set; }
		public string Date { get; set; }
	}

	public class ReceiveMessageRequestModel
	{
		public string QueueName { get; set; }
		public long Timeout { get; set; }
	}

	public class ReceiveMessageResponseModel : BasicResponseModel
	{
		public string Data { get; set; }
		public string Date { get; set; }
		public string ItemId { get; set; }
	}

	public class DeleteMessageRequestModel
	{
		public string ItemId { get; set; }
	}

	public class DeleteMessageResponseModel : BasicResponseModel
	{

	}
}