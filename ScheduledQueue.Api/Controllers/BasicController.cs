using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Newtonsoft.Json;
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

		#region Override Json serializer with Json.NET

		// Overriding the built-in Json serializer with Json.NET
		// Credits to Ricky Wan
		// http://wingkaiwan.com/2012/12/28/replacing-mvc-javascriptserializer-with-json-net-jsonserializer/

		class JsonNetResult : JsonResult
		{
			public JsonNetResult()
			{
				Settings = new JsonSerializerSettings
				{
					ReferenceLoopHandling = ReferenceLoopHandling.Error
				};
			}

			public JsonSerializerSettings Settings { get; private set; }

			public override void ExecuteResult(ControllerContext context)
			{
				if (context == null)
					throw new ArgumentNullException("context");
				if (this.JsonRequestBehavior == JsonRequestBehavior.DenyGet && string.Equals(context.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
					throw new InvalidOperationException("JSON GET is not allowed");

				HttpResponseBase response = context.HttpContext.Response;
				response.ContentType = string.IsNullOrEmpty(this.ContentType) ? "application/json" : this.ContentType;

				if (this.ContentEncoding != null)
					response.ContentEncoding = this.ContentEncoding;
				if (this.Data == null)
					return;

				var scriptSerializer = JsonSerializer.Create(this.Settings);

				using (var sw = new StringWriter())
				{
					scriptSerializer.Serialize(sw, this.Data);
					response.Write(sw.ToString());
				}
			}
		}

		protected override JsonResult Json(object data, string contentType,
			Encoding contentEncoding, JsonRequestBehavior behavior)
		{
			return new JsonNetResult
			{
				Data = data,
				ContentType = contentType,
				ContentEncoding = contentEncoding,
				JsonRequestBehavior = behavior
			};
		}

		#endregion
	}
}