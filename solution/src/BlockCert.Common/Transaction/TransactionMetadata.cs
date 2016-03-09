using System;

namespace BlockCert.Common.Transaction
{
	/// <summary>
	/// Basic metadata for transactions.
	/// </summary>
	public enum TransactionMetadata
	{
		/// <summary>
		/// A certificate is being issued.
		/// </summary>
		Issuance = 0x01,
		/// <summary>
		/// A certificate is being revoked.
		/// </summary>
		Revocation = 0x02
	}
}