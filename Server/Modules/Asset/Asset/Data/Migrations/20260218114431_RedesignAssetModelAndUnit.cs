using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asset.Data.Migrations
{
    /// <inheritdoc />
    public partial class RedesignAssetModelAndUnit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetHistories_Assets_AssetId",
                schema: "asset",
                table: "AssetHistories");

            migrationBuilder.DropTable(
                name: "AssetComponents",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "asset");

            migrationBuilder.RenameColumn(
                name: "AssetId",
                schema: "asset",
                table: "AssetHistories",
                newName: "AssetUnitId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetHistories_AssetId",
                schema: "asset",
                table: "AssetHistories",
                newName: "IX_AssetHistories_AssetUnitId");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                schema: "asset",
                table: "AssetHistories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "AssetModels",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetModels", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetModels_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "asset",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetUnits",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetModelId = table.Column<long>(type: "bigint", nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    AvailabilityStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OperationalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetUnits_AssetModels_AssetModelId",
                        column: x => x.AssetModelId,
                        principalSchema: "asset",
                        principalTable: "AssetModels",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_LaboratoryId",
                schema: "asset",
                table: "AssetModels",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetModels_LaboratoryId_Name",
                schema: "asset",
                table: "AssetModels",
                columns: new[] { "LaboratoryId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetUnits_AssetModelId",
                schema: "asset",
                table: "AssetUnits",
                column: "AssetModelId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetUnits_AssetTag",
                schema: "asset",
                table: "AssetUnits",
                column: "AssetTag",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetUnits_SerialNo",
                schema: "asset",
                table: "AssetUnits",
                column: "SerialNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetHistories_AssetUnits_AssetUnitId",
                schema: "asset",
                table: "AssetHistories",
                column: "AssetUnitId",
                principalSchema: "asset",
                principalTable: "AssetUnits",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AssetHistories_AssetUnits_AssetUnitId",
                schema: "asset",
                table: "AssetHistories");

            migrationBuilder.DropTable(
                name: "AssetUnits",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "AssetModels",
                schema: "asset");

            migrationBuilder.RenameColumn(
                name: "AssetUnitId",
                schema: "asset",
                table: "AssetHistories",
                newName: "AssetId");

            migrationBuilder.RenameIndex(
                name: "IX_AssetHistories_AssetUnitId",
                schema: "asset",
                table: "AssetHistories",
                newName: "IX_AssetHistories_AssetId");

            migrationBuilder.AlterColumn<string>(
                name: "Remark",
                schema: "asset",
                table: "AssetHistories",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    LaboratoryId = table.Column<long>(type: "bigint", nullable: false),
                    Amount = table.Column<long>(type: "bigint", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RealWorldId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Assets_Laboratory_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "asset",
                        principalTable: "Laboratories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetComponents",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RealWorldId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetComponents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AssetComponents_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "asset",
                        principalTable: "Assets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_AssetId",
                schema: "asset",
                table: "AssetComponents",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetComponents_AssetId_SerialNo",
                schema: "asset",
                table: "AssetComponents",
                columns: new[] { "AssetId", "SerialNo" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LaboratoryId",
                schema: "asset",
                table: "Assets",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Assets_RealWorldId",
                schema: "asset",
                table: "Assets",
                column: "RealWorldId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assets_SerialNo",
                schema: "asset",
                table: "Assets",
                column: "SerialNo",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AssetHistories_Assets_AssetId",
                schema: "asset",
                table: "AssetHistories",
                column: "AssetId",
                principalSchema: "asset",
                principalTable: "Assets",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
