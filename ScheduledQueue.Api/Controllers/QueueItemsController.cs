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
		IQueueService _queueService;
		
		public QueueItemsController(IQueueService queueService)
		{
			_queueService = queueService;
		}

		[Route("SendMessage")]
		public JsonResult SendMessage(SendMessageRequestModel request)
		{
			SendMessageResponseModel result = new SendMessageResponseModel();

			if (ModelState.IsValid)
			{
				try
				{
					SendMessageResult message;
					if (!String.IsNullOrEmpty(request.Date))
					{
						DateTime availabilityDate;
						try
						{
							availabilityDate = Utils.ParseIso8601Date(request.Date);
						}
						catch (Exception e)
						{
							throw new ModelErrorException("Date", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
						}

						message = _queueService.SendMessage(request.QueueName, request.MessageBody, availabilityDate);
					}
					else if (request.Delay.HasValue)
					{
						if (request.Delay.Value < 0)
							throw new ModelErrorException("Delay", "Delay must not be a negative number.");

						var delay = TimeSpan.FromSeconds(request.Delay.Value);
						message = _queueService.SendMessage(request.QueueName, request.MessageBody, delay);
					}
					else
					{
						message = _queueService.SendMessage(request.QueueName, request.MessageBody);
					}

					result.QueueName = request.QueueName;
					result.MessageId = message.MessageId;
					result.Date = Utils.FormatIso8601Date(message.MessageDate);
				}
				catch (ModelErrorException e)
				{
					ModelState.AddModelError(e.Key, e.Message);
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
					TimeSpan receiveTimeout = TimeSpan.FromSeconds(0);
					if (request.ReceiveTimeout.HasValue)
					{
						if (request.ReceiveTimeout.Value < 0)
							throw new ModelErrorException("ReceiveTimeout", "ReceiveTimeout must not be a negative number.");
						receiveTimeout = TimeSpan.FromSeconds(request.ReceiveTimeout.Value);
					}

					TimeSpan visibilityTimeout = TimeSpan.FromSeconds(0);
					if (request.VisibilityTimeout.HasValue)
					{
						if (request.VisibilityTimeout.Value < 0)
							throw new ModelErrorException("VisibilityTimeout", "VisibilityTimeout must not be a negative number.");
						visibilityTimeout = TimeSpan.FromSeconds(request.VisibilityTimeout.Value);
					}

					var message = _queueService.ReceiveMessage(request.QueueName, receiveTimeout, visibilityTimeout);

					result.MessageId = message.MessageId;
					result.MessageBody = message.MessageBody;
					result.Date = Utils.FormatIso8601Date(message.MessageDate);
				}
				catch (ModelErrorException e)
				{
					ModelState.AddModelError(e.Key, e.Message);
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
					_queueService.DeleteMessage(request.QueueName, request.MessageId);
				}
				catch (ModelErrorException e)
				{
					ModelState.AddModelError(e.Key, e.Message);
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
					RescheduleMessageResult message;
					if (!String.IsNullOrEmpty(request.Date))
					{
						DateTime availabilityDate;
						try
						{
							availabilityDate = Utils.ParseIso8601Date(request.Date);
						}
						catch (Exception e)
						{
							throw new ModelErrorException("Date", Eleven41.Helpers.ExceptionHelper.GetInnermostMessage(e));
						}

						message = _queueService.RescheduleMessage(request.QueueName, request.MessageId, availabilityDate);
					}
					else if (request.Delay.HasValue)
					{
						if (request.Delay.Value < 0)
							throw new ModelErrorException("Delay", "Delay must not be a negative number.");

						var delay = TimeSpan.FromSeconds(request.Delay.Value);
						message = _queueService.RescheduleMessage(request.QueueName, request.MessageId, delay);
					}
					else
					{
						// Invalid combination
						throw new Exception("Date or Delay must be specified.");
					}

					result.QueueName = request.QueueName;
					result.MessageId = message.NewMessageId;
					result.Date = Utils.FormatIso8601Date(message.MessageDate);
				}
				catch (ModelErrorException e)
				{
					ModelState.AddModelError(e.Key, e.Message);
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