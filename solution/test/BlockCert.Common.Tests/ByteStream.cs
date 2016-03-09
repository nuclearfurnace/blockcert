using System;
using System.IO;
using System.Text;

namespace BlockCert.Common.Tests
{
	/// <summary>
	/// A memory stream better suited to easily constructing buffers used for testing compression routines.
	/// </summary>
	public class ByteStream
	{
		private MemoryStream _stream;

		/// <summary>
		/// Creates a new, empty bytestream.
		/// </summary>
		public static ByteStream Create()
		{
			return new ByteStream() { _stream = new MemoryStream() };
		}

		/// <summary>
		/// Creates a new bytestream with an initial value.
		/// </summary>
		/// <param name="s">starting string</param>
		public static ByteStream Create(string s)
		{
			var byteStream = new ByteStream() { _stream = new MemoryStream() };
			return byteStream.Add(s);
		}

		/// <summary>
		/// Add a single byte.
		/// </summary>
		/// <param name="b">the byte value</param>
		public ByteStream Add(byte b)
		{
			_stream.WriteByte(b);
			return this;
		}

		/// <summary>
		/// Adds a buffer.
		/// </summary>
		/// <param name="buf">the buffer to add</param>
		public ByteStream Add(byte[] buf)
		{
			_stream.Write(buf, 0, buf.Length);
			return this;
		}

		/// <summary>
		/// Adds a string.
		/// </summary>
		/// <param name="s">the string to add</param>
		public ByteStream Add(string s)
		{
			var buf = Encoding.ASCII.GetBytes(s);
			_stream.Write(buf, 0, buf.Length);
			return this;
		}

		/// <summary>
		/// Gets the finalized buffer.
		/// </summary>
		/// <returns>finalized buffer</returns>
		public byte[] ToBytes()
		{
			return _stream.ToArray();
		}
	}
}
