using System;
using System.IO;
using BlockCert.Common.Transaction.Compression;
using BlockCert.Common.IO;
using System.Linq;

namespace BlockCert.Common.Transaction
{
	/// <summary>
	/// A blockcert transaction.
	/// 
	/// This contains the following:
	/// - issuance vs revocation
	/// - provider
	/// - organization
	/// - course
	/// 
	/// A fully compressed transaction looks something like:
	/// [ 2 bytes  TransactionHeader     ]
	/// [ 4 bytes  (provider) "edx.org"  ]
	/// [ 1 byte   (field stop - 0xFF)   ]
	/// [ 4 bytes  (org)      "mit.edu"  ]
	/// [ 1 byte   (field stop - 0xFF)   ]
	/// [ 4 bytes  (course)   1234567890 ]
	/// 
	/// The commom case represents a transaction size of around 17 bytes.  For larger domains,
	/// this number goes up quickly.  Our primary compression is in common subdomains and TLDs,
	/// as well as using an integer for a course identifier rather than, say, the course name.
	/// 
	/// For an example of a larger transaction:
	/// [ 2 bytes  TransactionHeader              ]
	/// [ 4 bytes  (provider) "edx.org"           ]
	/// [ 1 byte   (field stop - 0xFF)            ]
	/// [ 14 bytes (org)      "sarahlawrence.edu" ]
	/// [ 1 byte   (field stop - 0xFF)            ]
	/// [ 4 bytes  (course)   1234567890          ]
	/// 
	/// Coming in at a heavier 26 bytes, we're starting to get close to the limit of OP_RETURN.
	/// If the provider itself had a longer domain name, we'd be close to the breaking point,
	/// although we could even support "sarahlawrence.edu" being the provider address, which
	/// would weigh in at a hefty 36 bytes.  Tight fit!
	/// </summary>
	public class TransactionData : IEquatable<TransactionData>
	{
		private const byte StopMarker = (byte)0xFF;

		public TransactionMetadata Metadata { get; set; }
		public string Provider { get; set; }
		public string Organization { get; set; }
		public int Course { get; set; }

		public TransactionData()
		{
		}

		public byte[] ToBytes()
		{
			var stream = new MemoryStream();
			var builder = new DenseBinaryWriter(stream);

			// Put in the transaction metadata/version.
			builder.Write((byte)Metadata);
			builder.Write((byte)TransactionVersion.VersionSyncopate);

			// Stick in the provider.
			builder.Write(CompressedDomain.Squish(Provider));
			builder.Write(StopMarker);

			// Stick in the organization.
			builder.Write(CompressedDomain.Squish(Organization));
			builder.Write(StopMarker);

			// Anddd the course.
			builder.WriteVariableInteger(Course);

			// Fin.
			return stream.ToArray();
		}

		public static TransactionData FromStream(Stream stream)
		{
			var reader = new DenseBinaryReader(stream);

			var txMetadata = (TransactionMetadata)reader.ReadByte();
			var txVersion = (TransactionVersion)reader.ReadByte();

			var providerRaw = reader.ReadUntil(StopMarker);
			var organizationRaw = reader.ReadUntil(StopMarker);
			var course = reader.ReadVariableInteger();

			var provider = CompressedDomain.Expand(providerRaw);
			var organization = CompressedDomain.Expand(organizationRaw);

			return new TransactionData() {
				Metadata = txMetadata,
				Provider = provider,
				Organization = organization,
				Course = course
			};
		}

		#region IEquatable implementation

		public bool Equals(TransactionData other)
		{
			return Metadata.Equals(other.Metadata)
				&& Provider.Equals(other.Provider)
				&& Organization.Equals(other.Organization)
				&& Course.Equals(other.Course);
		}

		#endregion
	}
}