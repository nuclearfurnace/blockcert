using System;
using System.IO;
using BlockCert.Common.Compression;
using BlockCert.Common.IO;
using System.Linq;
using BlockCert.Common.Operations;

namespace BlockCert.Common.Transaction
{
	public class TransactionData : IEquatable<TransactionData>
	{
		private static byte[] MAGIC_HEADER = new byte[2] { 0xAB, 0xCD };

		private Stream _transactionDataStream;
		private IOperation _operation;

		public ProtocolVersion ProtocolVersion { get; set; }
		public OperationType OperationType { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockCert.Common.Transaction.TransactionData"/> class.
		/// </summary>
		public TransactionData()
		{
			ProtocolVersion = ProtocolVersion.VersionSyncopate;
		}

		/// <summary>
		/// Sets the operation this transaction represents.
		/// </summary>
		/// <param name="operation">the operation</param>
		public void SetOperation(IOperation operation)
		{
			OperationType = operation.GetOperationType();
			_operation = operation;
		}

		/// <summary>
		/// Creates an operation and attempts to deserialize it from the transaction data stream.
		/// </summary>
		/// <returns>the deserialized operation</returns>
		/// <typeparam name="TOperation">the operation type which must implement <see cref="BlockCert.Common.Operations.IOperation"/></typeparam>
		public TOperation GetOperation<TOperation>()
			where TOperation : IOperation, new()
		{
			var newOperation = new TOperation();
			newOperation.SetOperationType(OperationType);
			newOperation.FromStream(ProtocolVersion, _transactionDataStream);

			_operation = newOperation;

			return newOperation;
		}

		public void ToStream(Stream outputStream)
		{
			var builder = new DenseBinaryWriter(outputStream);

			// Write the magic header.
			builder.Write(MAGIC_HEADER);

			// Put in the protocol version/operation.
			builder.Write((byte)ProtocolVersion);
			builder.Write((byte)OperationType);

			// Write out our operation if we have one.
			if(_operation != null)
			{
				_operation.ToStream(ProtocolVersion, outputStream);
			}
		}

		public void FromStream(Stream inputStream)
		{
			var copiedInputStream = new MemoryStream();
			inputStream.CopyTo(copiedInputStream);
			copiedInputStream.Seek(0, SeekOrigin.Begin);

			var reader = new DenseBinaryReader(copiedInputStream);
			
			var magicHeader = reader.ReadBytes(2);
			if(magicHeader.Length != 2 || magicHeader[0] != MAGIC_HEADER[0] || magicHeader[1] != MAGIC_HEADER[1])
			{
				throw new InvalidTransactionDataException(string.Format(
					"magic header does not match! (expected {0}, got {1})",
					MAGIC_HEADER, magicHeader
				));
			}

			// Pull out the transaction version and operation.  Right now, we
			// only support one verion (Syncopate) but future code might require
			// knowing the transaction version for reading newer fields, etc.
			var protocolVersion = reader.ReadEnumeration<ProtocolVersion>();
			var operationType = reader.ReadEnumeration<OperationType>();

			_transactionDataStream = copiedInputStream;
			ProtocolVersion = protocolVersion;
			OperationType = operationType;
		}

		#region IEquatable implementation

		public bool Equals(TransactionData other)
		{
			return ProtocolVersion.Equals(other.ProtocolVersion)
				&& OperationType.Equals(other.OperationType)
				&& (_operation != null ? _operation.GetHashCode() == other._operation.GetHashCode() : true);
		}

		#endregion
	}
}