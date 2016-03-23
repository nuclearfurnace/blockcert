using System;

namespace BlockCert.Common.Operations
{
	/// <summary>
	/// Type of operation being performed by a transaction.
	/// </summary>
	public enum OperationType
	{
		/// <summary>
		/// Default value.
		/// </summary>
		None = 0x00,
		/// <summary>
		/// A certificate is being issued.
		/// </summary>
		Issue = 0x01,
		/// <summary>
		/// A certificate is being revoked.
		/// </summary>
		Revoke = 0x02,
		/// <summary>
		/// An identity digest is being established.
		/// </summary>
		IdentityDigest = 0x03
	}
}
