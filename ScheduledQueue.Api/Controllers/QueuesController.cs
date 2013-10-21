using ScheduledQueue.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScheduledQueue.Api.Controllers
{
	public class QueuesController : Controller
	{
		[Route("ListQueues")]
		public JsonResult ListQueues()
		{
			ListQueuesResponseModel result = new ListQueuesResponseModel();
			result.Queues = new List<string>();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("CreateQueue")]
		public JsonResult CreateQueue(CreateQueueRequestModel request)
		{
			CreateQueueResponseModel result = new CreateQueueResponseModel();

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("DeleteQueue")]
		public JsonResult DeleteQueue(DeleteQueueRequestModel request)
		{
			DeleteQueueResponseModel result = new DeleteQueueResponseModel();

			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}