using System;
using System.Collections.Generic;
using Microsoft.Data.Entity.Migrations;

namespace BlockCert.Api.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "keys",
                columns: table => new
                {
                    key_id = table.Column<long>(nullable: false)
                        .Annotation("Npgsql:Serial", true),
                    created_at = table.Column<DateTime>(nullable: false),
                    key_name = table.Column<string>(nullable: false),
                    key_type = table.Column<string>(nullable: false),
                    private_key = table.Column<string>(nullable: false),
                    public_address = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypedKey", x => x.key_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable("keys");
        }
    }
}
