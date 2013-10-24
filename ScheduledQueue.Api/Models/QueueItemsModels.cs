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

		public bool ShouldSerializeQueueName()
		{
			return !String.IsNullOrEmpty(QueueName);
		}

		public string MessageId { get; set; }

		public bool ShouldSerializeMessageId()
		{
			return !String.IsNullOrEmpty(MessageId);
		}

		public string Date { get; set; }

		public bool ShouldSerializeDate()
		{
			return !String.IsNullOrEmpty(Date);
		}
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

		public bool ShouldSerializeMessageBody()
		{
			return !String.IsNullOrEmpty(MessageBody);
		}
		
		public string Date { get; set; }

		public bool ShouldSerializeDate()
		{
			return !String.IsNullOrEmpty(Date);
		}

		public string MessageId { get; set; }

		public bool ShouldSerializeMessageId()
		{
			return !String.IsNullOrEmpty(MessageId);
		}
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

		public bool ShouldSerializeQueueName()
		{
			return !String.IsNullOrEmpty(QueueName);
		}

		public string MessageId { get; set; }

		public bool ShouldSerializeMessageId()
		{
			return !String.IsNullOrEmpty(MessageId);
		}

		public string Date { get; set; }

		public bool ShouldSerializeDate()
		{
			return !String.IsNullOrEmpty(Date);
		}
	}
}