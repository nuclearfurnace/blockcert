using System;
using System.IO;

namespace BlockCert.Common.IO
{
	/// <summary>
	/// A binary reader with helper methods for reading densely-packed values.
	/// </summary>
	public class DenseBinaryReader : BinaryReader
	{
		public DenseBinaryReader(Stream input) : base(input)
		{
		}

		/// <summary>
		/// Reads a 32-bit integer in a compressed format.
		/// </summary>
		/// <returns>the 32-bit integer value from the underlying stream</returns>
		public int ReadVariableInteger()
		{
			return base.Read7BitEncodedInt();
		}

		/// <summary>
		/// Reads an arbitrary amount of bytes from the stream until a sentinel
		/// value is encountered.
		/// </summary>
		/// <returns>the bytes read, without the sentinel value</returns>
		/// <param name="sentinel">the sentinel value to read until</param>
		public byte[] ReadUntil(byte sentinel)
		{
			var startPosition = BaseStream.Position;
			var stopPosition = startPosition;

			while(BaseStream.Position < BaseStream.Length)
			{
				var current = ReadByte();
				if(current == sentinel)
				{
					break;
				}

				stopPosition++;
			}

			return ReadBytes((int)(stopPosition - startPosition));
		}
	}
}