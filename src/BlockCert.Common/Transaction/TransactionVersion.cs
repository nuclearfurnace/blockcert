using System;

namespace BlockCert.Common.Transaction
{
	/// <summary>
	/// The version of transactions.
	/// 
	/// This is required for the eventuality of switching to another blockchain
	/// technology or changes to the Bitcoin protocol that grow or shrink the 
	/// size of metadata allowed in an OP_RETURN script.
	/// </summary>
	public enum TransactionVersion : byte
	{
		VersionSyncopate = 1
	}
}