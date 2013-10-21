using ScheduledQueue.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScheduledQueue.Core;

namespace ScheduledQueue.Api.Controllers
{
	public class QueuesController : BasicController
	{
		IQueueService _service;

		public QueuesController(IQueueService service)
		{
			_service = service;
		}

		[Route("ListQueues")]
		public JsonResult ListQueues()
		{
			ListQueuesResponseModel result = new ListQueuesResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					result.Queues = _service.ListQueues().ToList();
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
				}
				
			}

			CollectErrors(result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("CreateQueue")]
		public JsonResult CreateQueue(CreateQueueRequestModel request)
		{
			CreateQueueResponseModel result = new CreateQueueResponseModel();

			if (ModelState.IsValid)
			{

				try
				{
					string queueName = _service.CreateQueue(request.QueueName);
					result.QueueName = queueName;
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
				}
			}

			CollectErrors(result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("DeleteQueue")]
		public JsonResult DeleteQueue(DeleteQueueRequestModel request)
		{
			DeleteQueueResponseModel result = new DeleteQueueResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					_service.DeleteQueue(request.QueueName);
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
				}
			} 

			CollectErrors(result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}
	}
}