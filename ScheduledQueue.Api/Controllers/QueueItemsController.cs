using ScheduledQueue.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScheduledQueue.Core;

namespace ScheduledQueue.Api.Controllers
{
	public class QueueItemsController : BasicController
    {
		IQueueService _service;

		public QueueItemsController(IQueueService service)
		{
			_service = service;
		}

		[Route("SendMessage")]
		public JsonResult SendMessage(SendMessageRequestModel request)
		{
			SendMessageResponseModel result = new SendMessageResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					DateTime availabilityDate = Utils.ParseIso8601Date(request.Date);
					string messageId = _service.SendMessage(request.QueueName, request.MessageBody, availabilityDate);

					result.QueueName = request.QueueName;
					result.MessageId = messageId;
					result.Date = Utils.FormatIso8601Date(availabilityDate);
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
				}
			}

			CollectErrors(result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("ReceiveMessage")]
		public JsonResult ReceiveMessage(ReceiveMessageRequestModel request)
		{
			ReceiveMessageResponseModel result = new ReceiveMessageResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					long receiveTimeout = 0;
					if (request.ReceiveTimeout.HasValue)
						receiveTimeout = request.ReceiveTimeout.Value;

					long visibilityTimeout = 30;
					if (request.VisibilityTimeout.HasValue)
						visibilityTimeout = request.VisibilityTimeout.Value;

					var message = _service.ReceiveMessage(request.QueueName, receiveTimeout, visibilityTimeout);

					result.MessageId = message.MessageId;
					result.MessageBody = message.MessageBody;
					result.Date = Utils.FormatIso8601Date(message.MessageDate);
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
				}
			}

			CollectErrors(result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("DeleteMessage")]
		public JsonResult DeleteMessage(DeleteMessageRequestModel request)
		{
			DeleteMessageResponseModel result = new DeleteMessageResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					_service.DeleteMessage(request.QueueName, request.MessageId);
				}
				catch (Exception e)
				{
					ModelState.AddModelError("", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
				}
			}

			CollectErrors(result);
			return Json(result, JsonRequestBehavior.AllowGet);
		}

		[Route("RescheduleMessage")]
		public JsonResult RescheduleMessage(RescheduleMessageRequestModel request)
		{
			RescheduleMessageResponseModel result = new RescheduleMessageResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					DateTime availabilityDate = Utils.ParseIso8601Date(request.Date);
					string newMessageId = _service.RescheduleMessage(request.QueueName, request.MessageId, availabilityDate);

					result.QueueName = request.QueueName;
					result.MessageId = newMessageId;
					result.Date = Utils.FormatIso8601Date(availabilityDate);
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