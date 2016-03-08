using System;
using System.Collections.Generic;

namespace BlockCert.Common.Transaction.Compression
{
	public static class Dictionaries
	{
		public static Dictionary<string, byte> DefaultDomainDictionary = new Dictionary<string, byte>() {
			{"www.", 1},
			{".com", 2},
			{".net", 3},
			{".org", 4},
			{".edu", 5},
			{".gov", 6},
			{".uk", 7},
			{".co.uk", 8},
			{".ac.uk", 9},
		};
	}
}

