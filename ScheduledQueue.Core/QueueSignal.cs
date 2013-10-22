using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	internal class QueueSignal
	{
		#region Singleton

		private static QueueSignal _instance;

		static QueueSignal()
		{
			_instance = new QueueSignal();
		}

		internal static QueueSignal Instance
		{
			get
			{
				return _instance;
			}
		}

		#endregion

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

		public bool Wait(string queueName, TimeSpan delay)
		{
			var ev = GetEvent(queueName);
			return ev.WaitOne(delay);
		}

		public void Signal(string queueName)
		{
			var ev = GetEvent(queueName);
			ev.Set();
		}
	}
}
