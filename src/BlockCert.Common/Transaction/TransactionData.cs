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
		public TransactionVersion Version { get; set; }
		public string Provider { get; set; }
		public string Organization { get; set; }
		public int Course { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="BlockCert.Common.Transaction.TransactionData"/> class.
		/// </summary>
		public TransactionData()
		{
			Version = TransactionVersion.VersionSyncopate;
		}

		public byte[] ToBytes()
		{
			var stream = new MemoryStream();
			var builder = new DenseBinaryWriter(stream);

			// Put in the transaction metadata/version.
			builder.Write((byte)Metadata);
			builder.Write((byte)Version);

			// Stick in the provider and organization.  End both with the
			// stop marker do the decoder know how to split things up.
			builder.Write(DomainCompression.Compress(Provider));
			builder.Write(StopMarker);
			builder.Write(DomainCompression.Compress(Organization));
			builder.Write(StopMarker);

			// Write the course ID as a variable integer.
			builder.WriteVariableInteger(Course);

			return stream.ToArray();
		}

		public static TransactionData FromStream(Stream stream)
		{
			var reader = new DenseBinaryReader(stream);

			// Pull out the transaction metadata and version.  Right now, we
			// only support one verion (Syncopate) but future code might require
			// knowing the transaction version for reading newer fields, etc.
			var txMetadata = (TransactionMetadata)reader.ReadByte();
			var txVersion = (TransactionVersion)reader.ReadByte();

			// Grab the raw bytes for the provider and organization.
			var providerRaw = reader.ReadUntil(StopMarker);
			var organizationRaw = reader.ReadUntil(StopMarker);

			// Read the course ID.
			var course = reader.ReadVariableInteger();

			// Decompress the provider and organization.
			var provider = DomainCompression.Decompress(providerRaw);
			var organization = DomainCompression.Decompress(organizationRaw);

			return new TransactionData() {
				Version = txVersion,
				Metadata = txMetadata,
				Provider = provider,
				Organization = organization,
				Course = course
			};
		}

		#region IEquatable implementation

		public bool Equals(TransactionData other)
		{
			return Version.Equals(other.Version)
				&& Metadata.Equals(other.Metadata)
				&& Provider.Equals(other.Provider)
				&& Organization.Equals(other.Organization)
				&& Course.Equals(other.Course);
		}

		#endregion
	}
}