using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class InProcSignalService : ISignalService
	{
		Dictionary<string, AutoResetEvent> _eventMap = new Dictionary<string, AutoResetEvent>();

		AutoResetEvent GetEvent(string queueName)
		{
			lock (_eventMap)
			{
				if (!_eventMap.Keys.Contains(queueName))
				{
					var ev = new AutoResetEvent(false);
					_eventMap.Add(queueName, ev);
					return ev;
				}
				else
				{
					return _eventMap[queueName];
				}
			}
		}

		#region ISignalService Members

		public bool Wait(string queueName, TimeSpan delay)
		{
			var ev = GetEvent(queueName);
			return ev.WaitOne(delay);
		}

		public virtual void Signal(string queueName, SignalSources source)
		{
			var ev = GetEvent(queueName);
			ev.Set();
		}

		#endregion
	}
}
