using System;
using Xunit;
using BlockCert.Common.Transaction;
using System.IO;
using System.Linq;
using BlockCert.Common.Tests.Operations;

namespace BlockCert.Common.Tests.Transaction
{
	public class TransactionDataTest
	{
		[Fact]
		public void TestTransactionSerializationDeserialization()
		{
			// Create our outgoing transaction.
			var outputOperation = new MockOperation("woohoo!");
			var outputTx = new TransactionData();
			outputTx.SetOperation(outputOperation);

			// Serialize it.
			var outputStream = new MemoryStream();
			outputTx.ToStream(outputStream);

			// Read in our transaction.
			var inputStream = new MemoryStream(outputStream.ToArray());

			var inputTx = new TransactionData();
			inputTx.FromStream(inputStream);

			var inputOperation = inputTx.GetOperation<MockOperation>();

			Assert.Equal(outputOperation, inputOperation, new GenericEqualityComparer<MockOperation>());
			Assert.Equal(outputTx, inputTx);
		}
	}
}