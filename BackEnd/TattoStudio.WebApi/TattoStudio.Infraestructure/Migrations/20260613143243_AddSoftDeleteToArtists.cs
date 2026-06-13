using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TattoStudio.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddSoftDeleteToArtists : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeactivatedAt",
                table: "Artists",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Artists",
                type: "boolean",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeactivatedAt",
                table: "Artists");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Artists");
        }
    }
}
