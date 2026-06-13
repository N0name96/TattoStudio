using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TattoStudio.Infraestructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAppoinmentLifecycle : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Appoinments_ArtistId",
                table: "Appoinments");

            migrationBuilder.AddColumn<string>(
                name: "CancellationReason",
                table: "Appoinments",
                type: "character varying(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DurationMinutes",
                table: "Appoinments",
                type: "integer",
                nullable: false,
                defaultValue: 60);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Appoinments",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "AppoinmentAuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AppoinmentId = table.Column<Guid>(type: "uuid", nullable: false),
                    ChangedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    FieldName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    OldValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AppoinmentAuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AppoinmentAuditLogs_Appoinments_AppoinmentId",
                        column: x => x.AppoinmentId,
                        principalTable: "Appoinments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AppoinmentAuditLogs_Users_ChangedByUserId",
                        column: x => x.ChangedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StudioSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    WorkdayStart = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    WorkdayEnd = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudioSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Appoinments_ArtistId_AppoinmentDate",
                table: "Appoinments",
                columns: new[] { "ArtistId", "AppoinmentDate" });

            migrationBuilder.CreateIndex(
                name: "IX_AppoinmentAuditLogs_AppoinmentId",
                table: "AppoinmentAuditLogs",
                column: "AppoinmentId");

            migrationBuilder.CreateIndex(
                name: "IX_AppoinmentAuditLogs_ChangedByUserId",
                table: "AppoinmentAuditLogs",
                column: "ChangedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppoinmentAuditLogs");

            migrationBuilder.DropTable(
                name: "StudioSettings");

            migrationBuilder.DropIndex(
                name: "IX_Appoinments_ArtistId_AppoinmentDate",
                table: "Appoinments");

            migrationBuilder.DropColumn(
                name: "CancellationReason",
                table: "Appoinments");

            migrationBuilder.DropColumn(
                name: "DurationMinutes",
                table: "Appoinments");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Appoinments");

            migrationBuilder.CreateIndex(
                name: "IX_Appoinments_ArtistId",
                table: "Appoinments",
                column: "ArtistId");
        }
    }
}
