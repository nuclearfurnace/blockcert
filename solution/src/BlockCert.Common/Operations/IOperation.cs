using System;
using BlockCert.Common.IO;
using System.IO;

namespace BlockCert.Common.Operations
{
	public interface IOperation
	{
		/// <summary>
		/// Gets the underlying operation type for this operation.
		/// </summary>
		/// <returns>the operation type</returns>
		OperationType GetOperationType();
		/// <summary>
		/// Sets the underlying operation type for this operation.
		/// </summary>
		/// <param name="operationType">the operation type</param>
		void SetOperationType(OperationType operationType);
		/// <summary>
		/// Dehydrate this object to a stream.
		/// </summary>
		/// <param name="outputStream">the stream to write to</param>
		void ToStream(ProtocolVersion protocolVersion, Stream outputStream);
		/// <summary>
		/// Hydrate from a stream.
		/// </summary>
		/// <param name="protocolVersion">the version of the protocol we're reading from</param>
		/// <param name="inputStream">the stream to read from</param>
		void FromStream(ProtocolVersion protocolVersion, Stream inputStream);
	}
}
