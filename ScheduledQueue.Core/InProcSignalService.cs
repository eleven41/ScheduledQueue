using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public class InProcSignalService : ISignalService
	{
		#region ISignalService Members

		public void Signal(string queueName, SignalSources source)
		{
			QueueSignal.Instance.Signal(queueName);
		}

		#endregion
	}
}
