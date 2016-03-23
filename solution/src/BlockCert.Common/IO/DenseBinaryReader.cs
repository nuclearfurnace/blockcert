using System;
using System.IO;

namespace BlockCert.Common.IO
{
	/// <summary>
	/// Reads densely-packed primitive data types as binary values from a stream in a specific encoding.
	/// </summary>
	public class DenseBinaryReader : BinaryReader
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BlockCert.Common.IO.DenseBinaryReader"/> class
		/// based on the specified stream and using UTF-8 encoding.
		/// </summary>
		/// <param name="input">the input stream</param>
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
		/// Reads an arbitrary amount of bytes terminated by a NUL byte.
		/// </summary>
		/// <returns>the bytes read, without the NUL byte</returns>
		/// <param name="sentinel">the sentinel value to read until</param>
		public byte[] ReadNullTerminatedSequence()
		{
			var startPosition = BaseStream.Position;
			var stopPosition = startPosition;

			// Find the next instance of the sentinel value, or hit the EOF.
			while(BaseStream.Position < BaseStream.Length)
			{
				var current = ReadByte();
				if(current == 0x00)
				{
					break;
				}

				stopPosition++;
			}

			// Swing back to where we started.
			BaseStream.Seek(startPosition, SeekOrigin.Begin);

			// Read everything from the start point up until the sentinel value, or EOF.
			var buf = ReadBytes((int)(stopPosition - startPosition));

			// Skip over the NUL byte.
			ReadByte();

			return buf;
		}

		public T ReadEnumeration<T>()
			where T : struct, IConvertible, IFormattable, IComparable
		{
			var enumRaw = base.ReadByte();
			return (T)Enum.Parse(typeof(T), enumRaw.ToString());
		}
	}
}