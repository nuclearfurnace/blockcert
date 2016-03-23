using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using NBitcoin;
using BlockCert.Api.Database.Models;
using BlockCert.Api.Database.Validation;
using BlockCert.Api.Transformations;
using BlockCert.Api.Database;
using BlockCert.Api.Results;
using System.Threading;

namespace BlockCert.Api.Controllers
{
	[Route("api/[controller]")]
	[ValidateModelState]
	[CompliantResponse]
	public class KeysController : Controller
	{
		private BlockCertContext _context;

		public KeysController(BlockCertContext context)
		{
			_context = context;
		}

		/// <summary>
		/// Creates a new key based on the name and type given.
		/// 
		/// This generates a new private key on-the-fly, and in turn, generates a public address
		/// which is then associated with the name and type given.  This is used for generating
		/// addresses for providers, organizations, courses, and learners.
		/// </summary>
		/// <param name="partialKey">Partial key.</param>
		[HttpPost("create")]
		public TypedKey Create([FromBody]PartialKey partialKey)
		{
			var typedKey = new TypedKey(partialKey);

			// Augment the key with a newly-generated private keypair.
			var keyPair = new Key();
			typedKey.PrivateKey = keyPair.ToString(Network.Main);
			typedKey.PublicAddress = keyPair.PubKey.GetAddress (Network.Main).ToString();
			typedKey.CreatedAt = DateTime.UtcNow;

			// Save everything to the database.
			_context.Keys.Add(typedKey);
			_context.SaveChanges();

			return typedKey;
		}

		[HttpGet("counts")]
		public KeyCounts Counts()
		{
			var perKeyCounts = _context.Keys.GroupBy(k => k.KeyType).Select(k => new { Name = k.Key, Count = k.Count() }).AsEnumerable();
			var totalKeyCount = perKeyCounts.Select(k => k.Count).Sum();

			var keyCounts = new KeyCounts();
			keyCounts.Total = totalKeyCount;

			foreach(var count in perKeyCounts)
			{
				keyCounts.AddCount(count.Name, count.Count);
			}

			return keyCounts;
		}
	}
}