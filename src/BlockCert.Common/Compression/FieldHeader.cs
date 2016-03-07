using System;
using System.IO;

namespace BlockCert.Compression
{
	public class FieldHeader
	{
		private const int HEADER_SIZE = 2;

		public FieldMetadata Metadata { get; set; }
		public int Length { get; set; }

		public static FieldHeader FromStream(Stream stream)
		{
			if(stream.Position + HEADER_SIZE > stream.Length)
				throw new EndOfStreamException("expecting 2 bytes to read, not enough available!");

			var metadata = (FieldMetadata)stream.ReadByte();
			var length = stream.ReadByte();

			return new FieldHeader() { Metadata = metadata, Length = length };
		}
	}
}