using ScheduledQueue.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ScheduledQueue.Api.Controllers
{
	public class QueueItemsController : Controller
    {
		[Route("SendMessage")]
		public JsonResult SendMessage(SendMessageRequestModel request)
		{
			SendMessageResponseModel result = new SendMessageResponseModel();

			result.QueueName = request.QueueName;
			result.Date = request.Date;
			result.ItemId = Guid.NewGuid()
				.ToString()
				.Replace("-", "");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("ReceiveMessage")]
		public JsonResult ReceiveMessage(ReceiveMessageRequestModel request)
		{
			ReceiveMessageResponseModel result = new ReceiveMessageResponseModel();

			//result.Date = DateTime.UtcNow;
			result.ItemId = Guid.NewGuid()
				.ToString()
				.Replace("-", "");

			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("DeleteMessage")]
		public JsonResult DeleteQueueItem(DeleteMessageRequestModel request)
		{
			DeleteMessageResponseModel result = new DeleteMessageResponseModel();
			
			return Json(result, JsonRequestBehavior.AllowGet);
		}

	}
}