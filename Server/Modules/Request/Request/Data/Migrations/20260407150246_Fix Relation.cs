using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Request.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixRelation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestedAssetCategory",
                schema: "request",
                table: "Requests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "TargetLaboratoryId",
                schema: "request",
                table: "Requests",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TargetLaboratoryId",
                schema: "request",
                table: "Requests",
                column: "TargetLaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TargetLaboratoryId_RequestedAssetCategory",
                schema: "request",
                table: "Requests",
                columns: new[] { "TargetLaboratoryId", "RequestedAssetCategory" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_TargetLaboratoryId",
                schema: "request",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_TargetLaboratoryId_RequestedAssetCategory",
                schema: "request",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestedAssetCategory",
                schema: "request",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "TargetLaboratoryId",
                schema: "request",
                table: "Requests");
        }
    }
}
