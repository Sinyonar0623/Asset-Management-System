using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Request.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRequestNoAndRequestedAssetCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestNo",
                schema: "request",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_TargetLaboratoryId_RequestedAssetCategory",
                schema: "request",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestNo",
                schema: "request",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestedAssetCategory",
                schema: "request",
                table: "Requests");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "RequestNo",
                schema: "request",
                table: "Requests",
                type: "character varying(30)",
                maxLength: 30,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RequestedAssetCategory",
                schema: "request",
                table: "Requests",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestNo",
                schema: "request",
                table: "Requests",
                column: "RequestNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_TargetLaboratoryId_RequestedAssetCategory",
                schema: "request",
                table: "Requests",
                columns: new[] { "TargetLaboratoryId", "RequestedAssetCategory" });
        }
    }
}
