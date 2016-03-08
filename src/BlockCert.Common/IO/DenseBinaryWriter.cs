using System;
using System.IO;

namespace BlockCert.Common.IO
{
	/// <summary>
	/// A binary writer with helper methods for writing densely-packed values.
	/// </summary>
	public class DenseBinaryWriter : BinaryWriter
	{
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
	}
}