using System;
using Microsoft.Data.Entity;
using Microsoft.Data.Entity.Infrastructure;
using Microsoft.Data.Entity.Metadata;
using Microsoft.Data.Entity.Migrations;
using BlockCert.Api.Database;

namespace BlockCert.Api.Migrations
{
    [DbContext(typeof(BlockCertContext))]
    [Migration("20160304025830_Initial")]
    partial class Initial
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.0-rc1-16348");

            modelBuilder.Entity("BlockCert.Api.Database.Models.TypedKey", b =>
                {
                    b.Property<long>("TypedKeyId")
                        .ValueGeneratedOnAdd()
                        .HasAnnotation("Relational:ColumnName", "key_id");

                    b.Property<DateTime>("CreatedAt")
                        .HasAnnotation("Relational:ColumnName", "created_at");

                    b.Property<string>("KeyName")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 64)
                        .HasAnnotation("Relational:ColumnName", "key_name");

                    b.Property<string>("KeyType")
                        .IsRequired()
                        .HasAnnotation("MaxLength", 16)
                        .HasAnnotation("Relational:ColumnName", "key_type");

                    b.Property<string>("PrivateKey")
                        .IsRequired()
                        .HasAnnotation("Relational:ColumnName", "private_key");

                    b.Property<string>("PublicAddress")
                        .IsRequired()
                        .HasAnnotation("Relational:ColumnName", "public_address");

                    b.HasKey("TypedKeyId");

                    b.HasAnnotation("Relational:TableName", "keys");
                });
        }
    }
}
