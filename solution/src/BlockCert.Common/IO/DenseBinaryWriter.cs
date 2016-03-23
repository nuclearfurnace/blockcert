using System;
using System.IO;

namespace BlockCert.Common.IO
{
	/// <summary>
	/// Writes densely-packed primitive data types as binary values to a stream in a specific encoding.
	/// </summary>
	public class DenseBinaryWriter : BinaryWriter
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="BlockCert.Common.IO.DenseBinaryWriter"/> class
		/// based on the specified stream and using UTF-8 encoding.
		/// </summary>
		/// <param name="output">the output stream</param>
		public DenseBinaryWriter(Stream output) : base(output)
		{
		}

		/// <summary>
		/// Writes a 32-bit integer in a compressed format.
		/// </summary>
		/// <param name="value">the 32-bit integer to write</param>
		public void WriteVariableInteger(int value)
		{
			base.Write7BitEncodedInt(value);
		}

		/// <summary>
		/// Writes a buffer that is terminated with a NUL byte.
		/// </summary>
		/// <param name="buf">the buffer to write</param>
		public void WriteNullTerminatedSequence(byte[] buf)
		{
			base.Write(buf);
			base.Write((byte)0x00);
		}
	}
}