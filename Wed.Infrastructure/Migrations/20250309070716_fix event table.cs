using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wed.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class fixeventtable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_Facilities_FacilityId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_FacilityId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "FacilityId",
                table: "Events");

            migrationBuilder.AddColumn<string>(
                name: "OwnerId",
                table: "Events",
                type: "varchar(255)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Events_OwnerId",
                table: "Events",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_AspNetUsers_OwnerId",
                table: "Events",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Events_AspNetUsers_OwnerId",
                table: "Events");

            migrationBuilder.DropIndex(
                name: "IX_Events_OwnerId",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "Events");

            migrationBuilder.AddColumn<Guid>(
                name: "FacilityId",
                table: "Events",
                type: "char(36)",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Events_FacilityId",
                table: "Events",
                column: "FacilityId");

            migrationBuilder.AddForeignKey(
                name: "FK_Events_Facilities_FacilityId",
                table: "Events",
                column: "FacilityId",
                principalTable: "Facilities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
