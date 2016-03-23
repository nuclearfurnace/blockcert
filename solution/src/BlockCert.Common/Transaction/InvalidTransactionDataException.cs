using System;

namespace BlockCert.Common.Transaction
{
	/// <summary>
	/// An exception for when transaction data -- magic header, version, etc -- is determined to be invalid.
	/// </summary>
	public class InvalidTransactionDataException : Exception
	{
		public InvalidTransactionDataException(string message) : base(message)
		{
		}
	}
}

