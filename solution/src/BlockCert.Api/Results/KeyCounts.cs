using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace BlockCert.Api.Results
{
	public class KeyCounts
	{
		[JsonProperty("total")]
		public int Total;

		[JsonProperty("counts")]
		public Dictionary<string, int> Counts;

		public KeyCounts()
		{
			Counts = new Dictionary<string, int>();
		}

		public void AddCount(string name, int count)
		{
			Counts.Add(name, count);
		}
	}
}