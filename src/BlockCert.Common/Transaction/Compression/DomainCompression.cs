using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Linq;
using System.Net;

namespace BlockCert.Common.Transaction.Compression
{
	/// <summary>
	/// Efficient binary compression specific to domain names.
	/// </summary>
	public static class DomainCompression
	{
		private const byte DictionaryValueMask = (byte)0x80;

		private static IDictionary<string, byte> _replacements = new Dictionary<string, byte>();
		private static IDictionary<byte, string> _backwardsReplacements = new Dictionary<byte, string>();
		private static IComparer<string> _replacementsComparer = Comparer<string>.Create((a, b) => a.CompareTo(b));

		static DomainCompression()
		{
			SetDefaultDictionary();
		}

		/// <summary>
		/// Sets the default dictionary for compressing domains.
		/// 
		/// Useful for reinstantiating the default after setting a test-specific dictionary.
		/// </summary>
		public static void SetDefaultDictionary()
		{
			SetDictionary(Dictionaries.DefaultDomainDictionary);
		}

		/// <summary>
		/// Sets a new compression dictionary.
		/// </summary>
		/// <returns>the updated dictionary put in place, after any transformations</returns>
		/// <param name="dictionary">the replacement dictionary to use</param>
		public static IDictionary<string, byte> SetDictionary(Dictionary<string, byte> dictionary)
		{
			// Sort the dictionary so that the largest keys get checked first.
			// This ensures we're always making the most efficient replacements
			// i.e. replacing '.ac.uk' instead of '.uk' on 'www.cam.ac.uk'.
			var sortedDictionary = new SortedDictionary<string, byte>(_replacementsComparer);
			var backwardsDictionary = new Dictionary<byte, string>();

			// Make sure all of our replacements can fit above the ASCII range.
			foreach(var replacementPair in dictionary)
			{
				if(replacementPair.Value > DictionaryValueMask)
					throw new InvalidDataException("replacement characters can't be higher than 0x80");

				var upshiftedValue = (byte)(replacementPair.Value | DictionaryValueMask);
				sortedDictionary.Add(replacementPair.Key, upshiftedValue);
				backwardsDictionary.Add(upshiftedValue, replacementPair.Key);
			}

			_replacements = sortedDictionary;
			_backwardsReplacements = backwardsDictionary;

			return sortedDictionary;
		}

		/// <summary>
		/// Compresses the given hostname.
		/// </summary>
		/// <returns>byte array of the compressed hostname</returns>
		/// <param name="hostname">the hostname to compress</param>
		public static byte[] Compress(string hostname)
		{
			var urlBuffer = Encoding.ASCII.GetBytes(hostname);

			// Go through all of our dictionary replacement values, and crunch what we can.
			foreach(var replacementPair in _replacements)
			{
				var searchBytes = Encoding.ASCII.GetBytes(replacementPair.Key);
				var replacementBytes = new byte[] { replacementPair.Value };
				urlBuffer = ReplaceByteSequence(urlBuffer, searchBytes, replacementBytes);
			}

			return urlBuffer;
		}

		/// <summary>
		/// Decompresses the given buffer as a hostname.
		/// </summary>
		/// <returns>the decompressed hostname</returns>
		/// <param name="buf">the buffer holding the compressed hostname</param>
		public static string Decompress(byte[] buf)
		{
			if(buf.Length < 3)
				throw new ArgumentException("buffer must be greater than 3 bytes!", "buf");

			var stream = new MemoryStream(buf);

			StringBuilder builder = new StringBuilder();

			// Expand whatever we can.
			while(stream.Position < stream.Length)
			{
				var current = (byte)stream.ReadByte();
				if(_backwardsReplacements.ContainsKey(current))
				{
					builder.Append(_backwardsReplacements[current]);
				}
				else
				{
					builder.Append((char)current);
				}
			}

			return builder.ToString();
		}

		/// <summary>
		/// Replaces a given byte sequence with another.
		/// </summary>
		/// <returns>the modified haystack, after any replacements</returns>
		/// <param name="haystack">the buffer to search/replace in</param>
		/// <param name="needle">the byte sequence to search for</param>
		/// <param name="replace">the byte sequence to replace the needle with</param>
		public static byte[] ReplaceByteSequence(byte[] haystack, byte[] needle, byte[] replace)
		{
			if(haystack == null || haystack.Length == 0)
				throw new ArgumentException("haystack can't be null/empty", "haystack");
			
			if(needle == null || needle.Length == 0)
				throw new ArgumentException("needle can't be null/empty", "needle");

			// Copy our haystack.
			var haystackBuf = new byte[haystack.Length];
			Buffer.BlockCopy(haystack, 0, haystackBuf, 0, haystack.Length);

			// Naive approach ahead: each loop, we scan the entire buffer, looking for matching
			// sequences.  If we find one, we set a flag and then replace the sequence right then
			// and there.  If we found a match in a given loop iteration, we restart the loop. If
			// we didn't find a match, we break out of it and return.  We're always operating on
			// our buffer copy.
			while(true)
			{
				var foundMatch = false;

				for(var offset = 0; offset < haystackBuf.Length; offset++)
				{
					// See if we can find an anchor point.
					if(haystackBuf[offset] == needle[0])
					{
						// If our needle is larger than the remaining bytes from the
						// offset, then it can't possibly match.
						if(needle.Length > haystackBuf.Length - offset)
							continue;

						// Run through our entire needle, matching byte-by-byte.
						var fullMatch = true;
						for(var j = 0; j < needle.Length; j++)
						{
							if(haystackBuf[offset + j] != needle[j])
							{
								fullMatch = false;
								break;
							}
						}

						if(fullMatch)
						{
							foundMatch = true;

							var newBufLength = haystackBuf.Length - needle.Length + replace.Length;
							var newBuf = new byte[newBufLength];

							var haystackAfterStart = offset + needle.Length;
							var newAfterStart = offset + replace.Length;
							var afterLength = haystackBuf.Length - haystackAfterStart;

							// Copy over the first half, before the match.
							Buffer.BlockCopy(haystackBuf, 0, newBuf, 0, offset);

							// Splice in the replacement value.
							Buffer.BlockCopy(replace, 0, newBuf, offset, replace.Length);

							// Copy the remaining bytes that come after the match.
							Buffer.BlockCopy(haystackBuf, haystackAfterStart, newBuf, newAfterStart, afterLength);

							haystackBuf = newBuf;
							break;
						}
					}
				}

				// If we didn't find a match, it means we covered the entire haystack, so we can
				// bail out of the loop at this point.
				if(!foundMatch)
					break;
			}

      		return haystackBuf;
		}
	}
}
