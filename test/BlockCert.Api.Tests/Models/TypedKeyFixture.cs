using System;
using BlockCert.Api.Database.Models;
using System.Linq;
using System.Collections.Generic;
using NBitcoin;

namespace BlockCert.Api.Tests.Models
{
	public static class TypedKeyFixture
	{
		public static IQueryable<TypedKey> GenerateTypedKeyQueryable(int initialEntries)
		{
			var data = new List<TypedKey>();
			for(int i = 0; i < initialEntries; i++)
			{
				var newKeypair = new Key();
				var newTypedKey = new TypedKey() {
					KeyName = string.Format("Typed Key #{0}", i),
					KeyType = "magical",
					PrivateKey = newKeypair.GetWif(Network.Main).ToWif(),
					PublicAddress = newKeypair.PubKey.GetAddress(Network.Main).ToString(),
					CreatedAt = DateTime.UtcNow
				};

				data.Add(newTypedKey);
			}

			return data.AsQueryable();
		}
	}
}

