using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScheduledQueue.Api.Models;
using ScheduledQueue.Core;

namespace ScheduledQueue.Api.Controllers
{
    public class DateTimeController : BasicController
    {
		IDateTimeService _dateTimeService;

		public DateTimeController(IDateTimeService dateTimeService)
		{
			_dateTimeService = dateTimeService;
		}

        [Route("GetCurrentDateTime")]
        public JsonResult GetCurrentDateTime()
        {
			GetCurrentDateTimeResponseModel result = new GetCurrentDateTimeResponseModel();
			if (ModelState.IsValid)
			{
				try
				{
					DateTime date = _dateTimeService.GetCurrentDateTime();
					result.Date = Utils.FormatIso8601Date(date);
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