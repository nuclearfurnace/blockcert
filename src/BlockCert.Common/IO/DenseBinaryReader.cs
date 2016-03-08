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

			// Find the next instance of the sentinel value, or hit the EOF.
			while(BaseStream.Position < BaseStream.Length)
			{
				var current = ReadByte();
				if(current == sentinel)
				{
					break;
				}

				stopPosition++;
			}

			// Swing back to where we started.
			BaseStream.Seek(startPosition, SeekOrigin.Begin);

			// Read everything from the start point up until the sentinel value, or EOF.
			var buf = ReadBytes((int)(stopPosition - startPosition));

			// Skip over the sentinel value.
			ReadByte();

			return buf;
		}
	}
}