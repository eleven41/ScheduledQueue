using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Newtonsoft.Json;

namespace ScheduledQueue.Api.Models
{
	public class BasicResponseModel
	{
		public List<ErrorItem> Errors { get; set; }

		public bool ShouldSerializeErrors()
		{
			return (Errors != null);
		}
	}

	public class ErrorItem
	{
		public string Key { get; set; }
		public string Message { get; set; }
	}
}