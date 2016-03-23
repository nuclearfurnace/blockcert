using System;
using BlockCert.Common.Operations;
using System.IO;
using System.Text;

namespace BlockCert.Common.Tests.Operations
{
	public class MockOperation : IOperation, IEquatable<MockOperation>
	{
		public string Data { get; private set; }

		public MockOperation()
		{
		}

		public MockOperation(string data)
		{
			Data = data;
		}

		#region IOperation implementation

		public OperationType GetOperationType()
		{
			return OperationType.None;
		}

		public void SetOperationType(OperationType operationType)
		{
		}

		public void ToStream(ProtocolVersion protocolVersion, Stream outputStream)
		{
			var writer = new BinaryWriter(outputStream);
			writer.Write(Encoding.ASCII.GetBytes(Data));
			writer.Flush();
		}

		public void FromStream(ProtocolVersion protocolVersion, Stream inputStream)
		{
			var reader = new BinaryReader(inputStream);
			var dataLength = reader.BaseStream.Length - reader.BaseStream.Position;
			var buf = reader.ReadBytes((int)dataLength);
			Data = Encoding.ASCII.GetString(buf);
		}

		#endregion

		#region IEquatable implementation

		public override int GetHashCode()
		{
			return Data.GetHashCode();
		}

		public bool Equals(MockOperation other)
		{
			return Data.Equals(other.Data);
		}

		#endregion
	}
}
