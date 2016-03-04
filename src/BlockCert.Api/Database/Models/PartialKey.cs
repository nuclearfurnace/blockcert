using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;

namespace BlockCert.Api.Database.Models
{
	/// <summary>
	/// A partial key describing the name and type of a key to be generated.
	/// </summary>
	public class PartialKey
	{
		[Required]
		[StringLength(64, MinimumLength = 6)]
		public string KeyName { get; set; }

		[Required]
		[StringLength(16, MinimumLength = 6)]
		public string KeyType { get; set; }

		public override string ToString()
		{
			return string.Format("PartialKey[KeyName: {0}, KeyType: {1}]", KeyName, KeyType);
		}
	}
}