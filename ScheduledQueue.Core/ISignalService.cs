﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScheduledQueue.Core
{
	public enum SignalSources
	{
		SendMessage,
		ReceiveTimeout,
		RescheduleMessage
	}

	public interface ISignalService
	{
		void Signal(string queueName, SignalSources source);
	}
}
