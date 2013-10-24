using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ScheduledQueue.Api.Models
{
	public class ListQueuesResponseModel : BasicResponseModel
	{
		public List<string> Queues { get; set; }

		public bool ShouldSerializeQueues()
		{
			return (Queues != null);
		}
	}

	public class CreateQueueRequestModel
	{
		[Required]
		public string QueueName { get; set; }
	}

	public class CreateQueueResponseModel : BasicResponseModel
	{
		public string QueueName { get; set; }

		public bool ShouldSerializeQueueName()
		{
			return !String.IsNullOrEmpty(QueueName);
		}
	}

	public class DeleteQueueRequestModel
	{
		[Required]
		public string QueueName { get; set; }
	}

	public class DeleteQueueResponseModel : BasicResponseModel
	{
	}
}