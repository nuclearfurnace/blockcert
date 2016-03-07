using System;

namespace BlockCert.Common.Compression
{
	public enum KeyedDomainMetadata : byte {
		None = 0,
		IsProvider = 0x01,
		IsOrganization = 0x02,
		IsCourse = 0x04,
		IsLearner = 0x08,
		IsHTTPS = 0x10,        // scheme defaults to http, otherwise
		IsIPv4 = 0x20,         // host will be encoded as uint32
		Reserved1 = 0x40,      // possibly punycode?
		Reserved2 = 0x80
	}
}