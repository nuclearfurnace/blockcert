using System;
using Microsoft.AspNet.Mvc;

namespace BlockCert.Api.Transformations.Http
{
	/// <summary>
	/// Type of response, corresponding to JSend.
	/// </summary>
	public enum ResponseType
	{
		Success,
		Error,
		Fail
	}

	public class ResponseTypeHelper
	{
		public static ResponseType ClassifyObjectResult(ObjectResult objectResult)
		{
			if (objectResult.StatusCode.HasValue) {
				var statusCode = objectResult.StatusCode.Value;
				if (statusCode >= 500) {
					return ResponseType.Error;
				}

				if (statusCode >= 400) {
					return ResponseType.Fail;
				}
			}
				
			return ResponseType.Success;
		}
	}
}