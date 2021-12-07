using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace DocVault.DAL.Migrations
{
    public partial class DocumentSaltProperty : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "Salt",
                table: "Documents",
                type: "varbinary(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Salt",
                table: "Documents");
        }
    }
}
