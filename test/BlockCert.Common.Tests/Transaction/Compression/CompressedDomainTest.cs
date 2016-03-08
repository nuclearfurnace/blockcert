using System;
using System.Text;
using Xunit;
using BlockCert.Common.Transaction.Compression;
using System.Collections.Generic;
using System.Net;

namespace BlockCert.Common.Tests.Transaction.Compression
{
	public class CompressedDomainTest : IDisposable
	{
		private static IDictionary<string, byte> TestDictionary;

		public CompressedDomainTest()
		{
			var testDictionary = new Dictionary<string, byte>() {
				{"www.", 1},
				{".org", 2},
				{".edu", 3},
				{".uk", 4},
				{".ac.uk", 5},
			};
			TestDictionary = CompressedDomain.SetDictionary(testDictionary);
		}

		public void Dispose()
		{
			CompressedDomain.SetDefaultDictionary();
		}

    	[Fact]
		public void TestReplaceByteSequenceThrowsOnInvalidInputs()
		{
      		Assert.Throws<ArgumentException>(() => CompressedDomain.ReplaceByteSequence(null, null, null));
      		Assert.Throws<ArgumentException>(() => CompressedDomain.ReplaceByteSequence(null, new byte[] {}, null));
      		Assert.Throws<ArgumentException>(() => CompressedDomain.ReplaceByteSequence(new byte[] {}, null, null));
      		Assert.Throws<ArgumentException>(() => CompressedDomain.ReplaceByteSequence(new byte[] {}, new byte[] {}, null));
		}

  		[Fact]
		public void TestReplaceByteSequenceNoMatch()
		{
			var haystack = ByteStream.Create("www.edx.org").ToBytes();
			var needle = ByteStream.Create("zzz.").ToBytes();
			var replace = ByteStream.Create("aaa.").ToBytes();
			var expected = ByteStream.Create("www.edx.org").ToBytes();

			var result = CompressedDomain.ReplaceByteSequence(haystack, needle, replace);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void TestReplaceByteSequenceSimple()
		{
			var haystack = ByteStream.Create("www.edx.org").ToBytes();
			var needle = ByteStream.Create("www.").ToBytes();
			var replace = ByteStream.Create("zzz.").ToBytes();
			var expected = ByteStream.Create("zzz.edx.org").ToBytes();

			var result = CompressedDomain.ReplaceByteSequence(haystack, needle, replace);
			Assert.Equal(expected, result);
		}

    	[Fact]
		public void TestReplaceByteSequenceMultiple()
		{
			var haystack = ByteStream.Create("www.edx.org/edx").ToBytes();
			var needle = ByteStream.Create("edx").ToBytes();
			var replace = ByteStream.Create("bb").ToBytes();
			var expected = ByteStream.Create("www.bb.org/bb").ToBytes();

			var result = CompressedDomain.ReplaceByteSequence(haystack, needle, replace);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void TestSquishSimple()
		{
			var uri = "www.edx.org";
			var expectedByteStream = ByteStream.Create()
				.Add(TestDictionary["www."])
				.Add("edx")
				.Add(TestDictionary[".org"])
				.ToBytes();

			var result = CompressedDomain.Squish(uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Fact]
		public void TestSquishDictionaryOrdering()
		{
			var uri = "www.cam.ac.uk";
			var expectedByteStream = ByteStream.Create()
				.Add(TestDictionary["www."])
				.Add("cam")
				.Add(TestDictionary[".ac.uk"])
				.ToBytes();

			var result = CompressedDomain.Squish(uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Theory]
		[InlineData("www.edx.org", "www.edx.org")]
		[InlineData("23.43.12.32", "23.43.12.32")]
		[InlineData("www.cam.ac.uk", "www.cam.ac.uk")]
		public void TestSquishExpand(string input, string expected)
		{
			var squished = CompressedDomain.Squish(input);
			var expanded = CompressedDomain.Expand(squished);

			Assert.Equal(expected, expanded);
		}
	}
}