using System;
using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc;
using BlockCert.Api.Transformations.Http;

namespace BlockCert.Api.Transformations
{
	public class CompliantResponseAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuted(ActionExecutedContext context)
		{
			// If an exception occured, mark it handled and wrap it as a compliant response.
			if (context.Exception != null && !context.ExceptionHandled) {
				context.Result = CompliantResponseHelper.AsObjectResult (ResponseType.Error, context.Exception.Message);
				context.ExceptionHandled = true;
				return;
			}

			// No exceptions captured, so proceed normally.
			ObjectResult objectResult = context.Result as ObjectResult;
			if (objectResult != null) {
				var responseType = ResponseTypeHelper.ClassifyObjectResult(objectResult);
				context.Result = CompliantResponseHelper.AsObjectResult(responseType, objectResult.Value);
			}
		}
	}
}