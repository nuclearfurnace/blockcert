using System;
using System.IO;
using BlockCert.Common.IO;
using BlockCert.Common.Compression;

namespace BlockCert.Common.Operations
{
	/// <summary>
	/// A certification operation.  This can be an issuance, showing that a learner has
	/// passed a course, or a revocation, showing that a previous issuance is now no
	/// longer valid, for whatever reason.
	/// </summary>
	public class CertificationOperation : IOperation
	{
		public string Provider { get; set; }
		public string Organization { get; set; }
		public int Course { get; set; }
		private OperationType _operationType;

		private CertificationOperation(OperationType opType)
		{
			_operationType = opType;
		}

		public static CertificationOperation CreateCertificationIssueOperation()
		{
			return new CertificationOperation(OperationType.Issue);
		}

		public static CertificationOperation CreateCertificationRevokeOperation()
		{
			return new CertificationOperation(OperationType.Revoke);
		}

		#region IOperation implementation

		public OperationType GetOperationType()
		{
			return _operationType;
		}

		public void SetOperationType(OperationType operationType)
		{
			_operationType = operationType;
		}
			
		/// <summary>
		/// Serialize this object to a stream.
		/// </summary>
		/// <param name="outputStream">the stream to write to</param>
		public void ToStream(ProtocolVersion protocolVersion, Stream outputStream)
		{
			var domainCompression = new DomainCompression();
			domainCompression.SetDictionary(Dictionaries.DefaultDomainDictionary);

			var builder = new DenseBinaryWriter(outputStream);

			// Write our operation.
			builder.Write((byte)_operationType);

			// Stick in the provider and organization.  End both with the
			// stop marker do the decoder know how to split things up.
			builder.WriteNullTerminatedSequence(domainCompression.Compress(Provider));
			builder.WriteNullTerminatedSequence(domainCompression.Compress(Organization));

			// Write the course ID as a variable integer.
			builder.WriteVariableInteger(Course);
		}

		/// <summary>
		/// Deserialize an object from a stream.
		/// </summary>
		/// <returns>the deserialized operation</returns>
		/// <param name="protocolVersion">the version of the protocol we're reading from</param>
		/// <param name="inputStream">the stream to read from</param>
		public void FromStream(ProtocolVersion protocolVersion, Stream inputStream)
		{
			var domainCompression = new DomainCompression();
			domainCompression.SetDictionary(Dictionaries.DefaultDomainDictionary);

			var reader = new DenseBinaryReader(inputStream);

			// Grab the raw bytes for the provider and organization.
			var providerRaw = reader.ReadNullTerminatedSequence();
			var organizationRaw = reader.ReadNullTerminatedSequence();

			// Read the course ID.
			var course = reader.ReadVariableInteger();

			// Decompress the provider and organization.
			var provider = domainCompression.Decompress(providerRaw);
			var organization = domainCompression.Decompress(organizationRaw);

			Provider = provider;
			Organization = organization;
			Course = course;
		}

		#endregion
	}
}
