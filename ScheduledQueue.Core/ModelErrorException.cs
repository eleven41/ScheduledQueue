using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class ModelErrorException : Exception
	{
		public ModelErrorException(string key, string message)
			: base(message)
		{
			this.Key = key;
		}

		public ModelErrorException(string key, string message, Exception innerException)
			: base(message, innerException)
		{
			this.Key = key;
		}

		public string Key { get; protected set; }
	}
}
