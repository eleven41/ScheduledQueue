using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ScheduledQueue.Api.Models;

namespace ScheduledQueue.Api.Controllers
{
	public class BasicController : Controller
	{
		protected void CollectErrors(BasicResponseModel response)
		{
			foreach (var error in ModelState)
			{
				string errorMessage = null;
				var modelError = error.Value.Errors.FirstOrDefault();
				if (modelError == null)
					continue;
				errorMessage = modelError.ErrorMessage;
				if (String.IsNullOrEmpty(errorMessage))
					continue;

				if (response.Errors == null)
					response.Errors = new List<ErrorItem>();

				response.Errors.Add(new ErrorItem()
				{
					Key = error.Key,
					Message = errorMessage
				});
			}
		}
	}
}