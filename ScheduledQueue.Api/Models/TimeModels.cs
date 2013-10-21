using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ScheduledQueue.Api.Models
{
	public class GetCurrentDateTimeResponseModel : BasicResponseModel
	{
		public string Date { get; set; }
	}
}