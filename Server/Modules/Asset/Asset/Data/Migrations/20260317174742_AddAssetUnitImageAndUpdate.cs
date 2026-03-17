using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asset.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAssetUnitImageAndUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Purpose",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.RenameColumn(
                name: "ApproveBy",
                schema: "asset",
                table: "AssetUnitHistories",
                newName: "PerformedBy");

            migrationBuilder.RenameColumn(
                name: "ApproveAt",
                schema: "asset",
                table: "AssetUnitHistories",
                newName: "PerformedAt");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<string>(
                name: "ActionType",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovedAt",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ApprovedBy",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromAvailabilityStatus",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FromOperationalStatus",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "FromOwnerId",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ReferenceNo",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToAvailabilityStatus",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ToOperationalStatus",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "character varying(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ToOwnerId",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AssetUnitImages",
                schema: "asset",
                columns: table => new
                {
                    AssetUnitImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    FileName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    ContentType = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    FileSizeBytes = table.Column<long>(type: "bigint", nullable: true),
                    IsPrimary = table.Column<bool>(type: "boolean", nullable: false),
                    AssetUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUnitImages", x => x.AssetUnitImageId);
                    table.ForeignKey(
                        name: "FK_AssetUnitImages_AssetUnits_AssetUnitId",
                        column: x => x.AssetUnitId,
                        principalSchema: "asset",
                        principalTable: "AssetUnits",
                        principalColumn: "AssetUnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetUnitImages_AssetUnitId",
                schema: "asset",
                table: "AssetUnitImages",
                column: "AssetUnitId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetUnitImages",
                schema: "asset");

            migrationBuilder.DropColumn(
                name: "ActionType",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "ApprovedAt",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "ApprovedBy",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "FromAvailabilityStatus",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "FromOperationalStatus",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "FromOwnerId",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "ReferenceNo",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "ToAvailabilityStatus",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "ToOperationalStatus",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.DropColumn(
                name: "ToOwnerId",
                schema: "asset",
                table: "AssetUnitHistories");

            migrationBuilder.RenameColumn(
                name: "PerformedBy",
                schema: "asset",
                table: "AssetUnitHistories",
                newName: "ApproveBy");

            migrationBuilder.RenameColumn(
                name: "PerformedAt",
                schema: "asset",
                table: "AssetUnitHistories",
                newName: "ApproveAt");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<string>(
                name: "Purpose",
                schema: "asset",
                table: "AssetUnitHistories",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
