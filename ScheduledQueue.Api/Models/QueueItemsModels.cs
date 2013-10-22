using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScheduledQueue.Api.Models
{
	public class SendMessageRequestModel
	{
		[Required]
		public string QueueName { get; set; }

		[Required]
		public string MessageBody { get; set; }

		public string Date { get; set; }

		public double? Delay { get; set; }
	}

	public class SendMessageResponseModel : BasicResponseModel
	{
		public string QueueName { get; set; }
		public string MessageId { get; set; }
		public string Date { get; set; }
	}

	public class ReceiveMessageRequestModel
	{
		[Required]
		public string QueueName { get; set; }

		public double? ReceiveTimeout { get; set; }
		public double? VisibilityTimeout { get; set; }
	}

	public class ReceiveMessageResponseModel : BasicResponseModel
	{
		public string MessageBody { get; set; }
		public string Date { get; set; }
		public string MessageId { get; set; }
	}

	public class DeleteMessageRequestModel
	{
		[Required]
		public string QueueName { get; set; }

		[Required]
		public string MessageId { get; set; }
	}

	public class DeleteMessageResponseModel : BasicResponseModel
	{

	}

	public class RescheduleMessageRequestModel
	{
		[Required]
		public string QueueName { get; set; }

		[Required]
		public string MessageId { get; set; }

		public string Date { get; set; }
		public double? Delay { get; set; }
	}

	public class RescheduleMessageResponseModel : BasicResponseModel
	{
		public string QueueName { get; set; }
		public string MessageId { get; set; }
		public string Date { get; set; }
	}
}