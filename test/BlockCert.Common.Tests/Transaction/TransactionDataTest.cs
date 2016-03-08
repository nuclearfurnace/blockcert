using System;
using Xunit;
using BlockCert.Common.Transaction;
using System.IO;

namespace BlockCert.Common.Tests.Transaction
{
	public class TransactionDataTest
	{
		[Fact]
		public void TestTransactionSerializationDeserialization()
		{
			var tx = new TransactionData() {
				Metadata = TransactionMetadata.Issuance,
				Provider = "edx.org",
				Organization = "mit.edu",
				Course = 123456
			};

			var serialized = tx.ToBytes();
			var deserialized = TransactionData.FromStream(new MemoryStream(serialized));

			Assert.Equal(tx, deserialized);
		}
	}
}