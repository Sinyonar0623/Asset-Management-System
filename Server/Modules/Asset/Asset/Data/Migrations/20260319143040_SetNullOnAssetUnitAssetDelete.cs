using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asset.Data.Migrations
{
    /// <inheritdoc />
    public partial class SetNullOnAssetUnitAssetDelete : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetUnits_Assets_AssetId",
                schema: "asset",
                table: "AssetUnits");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetUnits_Assets_AssetId",
                schema: "asset",
                table: "AssetUnits",
                column: "AssetId",
                principalSchema: "asset",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetUnits_Assets_AssetId",
                schema: "asset",
                table: "AssetUnits");

            migrationBuilder.AddForeignKey(
                name: "FK_AssetUnits_Assets_AssetId",
                schema: "asset",
                table: "AssetUnits",
                column: "AssetId",
                principalSchema: "asset",
                principalTable: "Assets",
                principalColumn: "AssetId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
