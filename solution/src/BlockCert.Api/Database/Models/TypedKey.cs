using System;
using System.ComponentModel.DataAnnotations;
using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlockCert.Api.Database.Models
{
	/// <summary>
	/// A private keypair with a name and type.
	/// </summary>
	[Table("keys")]
	public class TypedKey
	{
		[Key]
		[Column("key_id")]
		[JsonIgnore]
		public Int64 TypedKeyId { get; set; }

		[Required, StringLength(64, MinimumLength = 6)]
		[Column("key_name")]
		[JsonProperty("keyName")]
		public string KeyName { get; set; }

		[Required, StringLength(16, MinimumLength = 6)]
		[Column("key_type")]
		[JsonProperty("keyType")]
		public string KeyType { get; set; }

		[Required]
		[Column("private_key")]
		[JsonProperty("privateKey")]
		public string PrivateKey { get; set; }

		[Required]
		[Column("public_address")]
		[JsonProperty("publicAddress")]
		public string PublicAddress { get; set; }

		[Required]
		[Column("created_at")]
		[JsonProperty("createdAt")]
		public DateTime CreatedAt { get; set; }

		public TypedKey()
		{
		}

		public TypedKey(PartialKey partialKey)
		{
			KeyName = partialKey.KeyName;
			KeyType = partialKey.KeyType;
		}

		public override string ToString()
		{
			return string.Format("TypedKey[KeyName: {0}, KeyType: {1}, PublicAddress: {2}]", KeyName, KeyType, PublicAddress);
		}
	}
}