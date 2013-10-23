using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ScheduledQueue.Core
{
	public class MessageNotFoundException : Exception
	{
		public MessageNotFoundException(string messageId)
			: base("Message not found: " + messageId)
		{
			this.MessageId = messageId;
		}

		public string MessageId { get; set; }
	}
}
