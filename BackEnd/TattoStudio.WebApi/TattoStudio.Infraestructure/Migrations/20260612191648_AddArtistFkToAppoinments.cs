using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TattoStudio.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddArtistFkToAppoinments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Appoinments_ArtistId",
                table: "Appoinments",
                column: "ArtistId");

            migrationBuilder.AddForeignKey(
                name: "FK_Appoinments_Artists_ArtistId",
                table: "Appoinments",
                column: "ArtistId",
                principalTable: "Artists",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Appoinments_Artists_ArtistId",
                table: "Appoinments");

            migrationBuilder.DropIndex(
                name: "IX_Appoinments_ArtistId",
                table: "Appoinments");
        }
    }
}
