using System;
using Microsoft.Data.Entity;
using BlockCert.Api.Database.Models;

namespace BlockCert.Api.Database
{
	public class BlockCertContext : DbContext
	{
		public virtual DbSet<TypedKey> Keys { get; set; }

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<TypedKey>()
				.Property(k => k.KeyName)
				.IsRequired();

			modelBuilder.Entity<TypedKey>()
				.Property(k => k.KeyType)
				.IsRequired();

			modelBuilder.Entity<TypedKey>()
				.Property(k => k.PrivateKey)
				.IsRequired();

			modelBuilder.Entity<TypedKey>()
				.Property(k => k.PublicAddress)
				.IsRequired();

			modelBuilder.Entity<TypedKey>()
				.Property(k => k.CreatedAt)
				.IsRequired();
		}
	}
}

