using System;
using System.Linq;

namespace BlockCert.Common.Tests
{
	public static class TestUtility
	{
		public static void Inspect(string label, byte[] buffer)
		{
			var stringifiedBuffer = string.Join(" ", buffer.Select(b => b.ToString("X2")).ToArray());
			Console.WriteLine("{0}: [{1}]", label, stringifiedBuffer);
		}
	}
}
