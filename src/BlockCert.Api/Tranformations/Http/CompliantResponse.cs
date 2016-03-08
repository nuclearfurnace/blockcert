using System;
using Microsoft.AspNet.Mvc;
using Newtonsoft.Json;

namespace BlockCert.Api.Transformations.Http
{
	/// <summary>
	/// A JSend-compliant response container.
	/// </summary>
	public class CompliantResponse
	{
		/// <summary>
		/// Status of the response.
		/// 
		/// This should only ever be `success`, `fail`, or `error`.
		/// </summary>
		[JsonProperty("status")]
		public string Status { get; set; }

		/// <summary>
		/// Status code of the response.  Since not all callers will have access to
		/// the raw HTTP status code, it is useful to include here.
		/// </summary>
		[JsonProperty("code", NullValueHandling = NullValueHandling.Ignore)]
		public int? Code { get; set; }

		/// <summary>
		/// Data associated with this response.
		/// 
		/// If `success`, potentially data resulting from the call.
		/// 
		/// If `error`, potentially detailed information about the error.
		/// </summary>
		[JsonProperty("data", NullValueHandling = NullValueHandling.Ignore)]
		public object Data { get; set; }

		/// <summary>
		/// Error message if <paramref name="Status"/> is `error`.
		/// 
		/// Optional.
		/// </summary>
		[JsonProperty("message", NullValueHandling = NullValueHandling.Ignore)]
		public string Message { get; set; }

		public CompliantResponse() {
			Status = "success";
			Code = 200;
		}
	}

	public static class CompliantResponseHelper {
		/// <summary>
		/// Creates a CompliantResponse wrapped as an ObjectResult.
		/// </summary>
		/// <returns>the wrapped CompliantResponse</returns>
		/// <param name="responseType">the ResponseType for this response</param>
		/// <param name="data">any data associated with this response</param>
		public static ObjectResult AsObjectResult(ResponseType responseType, object data) {
			var response = new CompliantResponse () { Data = data };
			switch (responseType) {
			case ResponseType.Fail:
				response.Status = "fail";
				response.Code = 400;
				break;
			case ResponseType.Error:
				response.Status = "error";
				response.Code = 500;
				break;
			}

			return new ObjectResult (response) { StatusCode = response.Code };
		}
	}
}