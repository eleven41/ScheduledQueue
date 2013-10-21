using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduledQueue.Api.Models
{
	public class ListQueuesResponseModel : BasicResponseModel
	{
		public List<string> Queues { get; set; }
	}

	public class CreateQueueRequestModel
	{
		public string QueueName { get; set; }
	}

	public class CreateQueueResponseModel : BasicResponseModel
	{
	}

	public class DeleteQueueRequestModel
	{
		public string QueueName { get; set; }
	}

	public class DeleteQueueResponseModel : BasicResponseModel
	{
	}
}