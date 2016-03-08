using System;
using System.Collections.Generic;

namespace BlockCert.Common.Transaction.Compression
{
	/// <summary>
	/// Collection of dictionaries tailored to specific compression schemes.
	/// </summary>
	public static class Dictionaries
	{
		/// <summary>
		/// Default dictionary for domain compression.  Contains the most common TLDs.
		/// TODO(tobz): figure out a way to generate this list programmatically so that
		/// we can be sure the list is statistically accurate and representative.
		/// </summary>
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