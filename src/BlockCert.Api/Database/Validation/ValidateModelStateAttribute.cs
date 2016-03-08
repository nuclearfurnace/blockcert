using Microsoft.AspNet.Mvc.Filters;
using Microsoft.AspNet.Mvc;

namespace BlockCert.Api.Database.Validation
{
	/// <summary>
	/// Action filter to send back model state errors to the caller if the model state isn't valid.
	/// </summary>
	public class ValidateModelStateAttribute : ActionFilterAttribute
	{
		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			if(!actionContext.ModelState.IsValid)
			{
				actionContext.Result = new BadRequestObjectResult(actionContext.ModelState);
			}
		}
	}
}