using System;

namespace BlockCert.Compression
{
	public enum FieldMetadata
	{
		IsProvider = 0x01,
		IsOrganization = 0x02,
		IsCourse = 0x04,
		IsLearner = 0x08
	}
}