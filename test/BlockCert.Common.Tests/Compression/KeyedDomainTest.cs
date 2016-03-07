using System;
using System.Text;
using Xunit;
using BlockCert.Common.Compression;
using System.Collections.Generic;
using System.Net;

namespace BlockCert.Common.Tests.Compression
{
	public class KeyedDomainTest : IDisposable
	{
		private static IDictionary<string, byte> TestDictionary;

		public KeyedDomainTest()
		{
			var testDictionary = new Dictionary<string, byte>() {
				{"www.", 1},
				{".org", 2},
				{".edu", 3},
				{".uk", 4},
				{".ac.uk", 5},
			};
			TestDictionary = KeyedDomain.SetDictionary(testDictionary);
		}

		public void Dispose()
		{
			KeyedDomain.SetDefaultDictionary();
		}

    	[Fact]
		public void TestReplaceByteSequenceThrowsOnInvalidInputs()
		{
      		Assert.Throws<ArgumentException>(() => KeyedDomain.ReplaceByteSequence(null, null, null));
      		Assert.Throws<ArgumentException>(() => KeyedDomain.ReplaceByteSequence(null, new byte[] {}, null));
      		Assert.Throws<ArgumentException>(() => KeyedDomain.ReplaceByteSequence(new byte[] {}, null, null));
      		Assert.Throws<ArgumentException>(() => KeyedDomain.ReplaceByteSequence(new byte[] {}, new byte[] {}, null));
		}

  		[Fact]
		public void TestReplaceByteSequenceNoMatch()
		{
			var haystack = ByteStream.Create("www.edx.org").ToBytes();
			var needle = ByteStream.Create("zzz.").ToBytes();
			var replace = ByteStream.Create("aaa.").ToBytes();
			var expected = ByteStream.Create("www.edx.org").ToBytes();

			var result = KeyedDomain.ReplaceByteSequence(haystack, needle, replace);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void TestReplaceByteSequenceSimple()
		{
			var haystack = ByteStream.Create("www.edx.org").ToBytes();
			var needle = ByteStream.Create("www.").ToBytes();
			var replace = ByteStream.Create("zzz.").ToBytes();
			var expected = ByteStream.Create("zzz.edx.org").ToBytes();

			var result = KeyedDomain.ReplaceByteSequence(haystack, needle, replace);
			Assert.Equal(expected, result);
		}

    	[Fact]
		public void TestReplaceByteSequenceMultiple()
		{
			var haystack = ByteStream.Create("www.edx.org/edx").ToBytes();
			var needle = ByteStream.Create("edx").ToBytes();
			var replace = ByteStream.Create("bb").ToBytes();
			var expected = ByteStream.Create("www.bb.org/bb").ToBytes();

			var result = KeyedDomain.ReplaceByteSequence(haystack, needle, replace);
			Assert.Equal(expected, result);
		}

		[Fact]
		public void TestSquishSimple()
		{
			var uri = "http://www.edx.org";
			var keyType = KeyType.Provider;
			var expectedControlFlag = (byte)(KeyedDomainMetadata.IsProvider);
			var expectedByteStream = ByteStream.Create()
				.Add((byte)expectedControlFlag)
				.Add(TestDictionary["www."])
				.Add("edx")
				.Add(TestDictionary[".org"])
				.ToBytes();

			var result = KeyedDomain.Squish(keyType, uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Fact]
		public void TestSquishNoSchema()
		{
			var uri = "www.edx.org";
			var keyType = KeyType.Provider;
			var expectedControlFlag = (byte)(KeyedDomainMetadata.IsProvider);
			var expectedByteStream = ByteStream.Create()
				.Add((byte)expectedControlFlag)
				.Add(TestDictionary["www."])
				.Add("edx")
				.Add(TestDictionary[".org"])
				.ToBytes();

			var result = KeyedDomain.Squish(keyType, uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Fact]
		public void TestSquishFullUrl()
		{
			var uri = "https://www.edx.org/dSq2Zb";
			var keyType = KeyType.Provider;
			var expectedControlFlag = (byte)(KeyedDomainMetadata.IsProvider | KeyedDomainMetadata.IsHTTPS);
			var expectedByteStream = ByteStream.Create()
				.Add((byte)expectedControlFlag)
				.Add(TestDictionary["www."])
				.Add("edx")
				.Add(TestDictionary[".org"])
				.Add("/dSq2Zb")
				.ToBytes();

			var result = KeyedDomain.Squish(keyType, uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Fact]
		public void TestSquishHttps()
		{
			var uri = "https://www.edx.org";
			var keyType = KeyType.Provider;
			var expectedControlFlag = (byte)(KeyedDomainMetadata.IsProvider | KeyedDomainMetadata.IsHTTPS);
			var expectedByteStream = ByteStream.Create()
				.Add((byte)expectedControlFlag)
				.Add(TestDictionary["www."])
				.Add("edx")
				.Add(TestDictionary[".org"])
				.ToBytes();

			var result = KeyedDomain.Squish(keyType, uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Fact]
		public void TestSquishDictionaryOrdering()
		{
			var uri = "https://www.cam.ac.uk";
			var keyType = KeyType.Organization;
			var expectedControlFlag = (byte)(KeyedDomainMetadata.IsOrganization | KeyedDomainMetadata.IsHTTPS);
			var expectedByteStream = ByteStream.Create()
				.Add((byte)expectedControlFlag)
				.Add(TestDictionary["www."])
				.Add("cam")
				.Add(TestDictionary[".ac.uk"])
				.ToBytes();

			var result = KeyedDomain.Squish(keyType, uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Fact]
		public void TestSquishRawIpAddress()
		{
			var ipAddress = IPAddress.Parse("127.0.0.1");
			var ipAddressBytes = ipAddress.GetAddressBytes();

			var uri = "http://127.0.0.1";
			var keyType = KeyType.Provider;
			var expectedControlFlag = (byte)(KeyedDomainMetadata.IsProvider | KeyedDomainMetadata.IsIPv4);
			var expectedByteStream = ByteStream.Create()
				.Add((byte)expectedControlFlag)
				.Add(0xFF)
				.Add(ipAddressBytes)
				.ToBytes();

			var result = KeyedDomain.Squish(keyType, uri);
			Assert.Equal(expectedByteStream, result);
		}

		[Theory]
		[InlineData("www.edx.org", "http://www.edx.org", KeyedDomainMetadata.None)]
		[InlineData("https://www.edx.org", "https://www.edx.org", KeyedDomainMetadata.IsHTTPS)]
		[InlineData("https://23.43.12.32", "https://23.43.12.32", KeyedDomainMetadata.IsHTTPS | KeyedDomainMetadata.IsIPv4)]
		[InlineData("http://www.cam.ac.uk/s1G7d9", "http://www.cam.ac.uk/s1G7d9", KeyedDomainMetadata.None)]
		public void TestSquishExpand(string input, string expected, KeyedDomainMetadata metadata)
		{
			metadata |= KeyedDomainMetadata.IsProvider;

			var squished = KeyedDomain.Squish(KeyType.Provider, input);
			var expanded = KeyedDomain.Expand(squished);

			Assert.Equal(squished[0], (byte)metadata);
			Assert.Equal(expected, expanded);
		}
	}
}