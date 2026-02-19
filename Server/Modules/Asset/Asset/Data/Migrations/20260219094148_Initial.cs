using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Asset.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "asset");

            migrationBuilder.CreateTable(
                name: "Laboratories",
                schema: "asset",
                columns: table => new
                {
                    LaboratoriesId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    LaboratoryName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RoomNo = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    TeacherId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.LaboratoriesId);
                });

            migrationBuilder.CreateTable(
                name: "OutboxMessages",
                schema: "asset",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Type = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: false),
                    Payload = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(1)", nullable: false),
                    OccurredOn = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "SYSUTCDATETIME()"),
                    ProcessedOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RetryCount = table.Column<int>(type: "int", nullable: false),
                    Error = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OutboxMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "asset",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IsAvailable = table.Column<bool>(type: "bit", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assets", x => x.AssetId);
                    table.ForeignKey(
                        name: "FK_Assets_Laboratories_LaboratoryId",
                        column: x => x.LaboratoryId,
                        principalSchema: "asset",
                        principalTable: "Laboratories",
                        principalColumn: "LaboratoriesId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetUnits",
                schema: "asset",
                columns: table => new
                {
                    AssetUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetTag = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    SerialNo = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AvailabilityStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    OperationalStatus = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AssetId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUnits", x => x.AssetUnitId);
                    table.ForeignKey(
                        name: "FK_AssetUnits_Assets_AssetId",
                        column: x => x.AssetId,
                        principalSchema: "asset",
                        principalTable: "Assets",
                        principalColumn: "AssetId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AssetUnitConditions",
                schema: "asset",
                columns: table => new
                {
                    AssetUnitConditionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEffectiveFromUnknown = table.Column<bool>(type: "bit", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsEffectiveToUnknown = table.Column<bool>(type: "bit", nullable: false),
                    AssetUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUnitConditions", x => x.AssetUnitConditionId);
                    table.ForeignKey(
                        name: "FK_AssetUnitConditions_AssetUnits_AssetUnitId",
                        column: x => x.AssetUnitId,
                        principalSchema: "asset",
                        principalTable: "AssetUnits",
                        principalColumn: "AssetUnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AssetUnitHistories",
                schema: "asset",
                columns: table => new
                {
                    AssetUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Purpose = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Remark = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ApproveBy = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ApproveAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AssetUnitHistories", x => new { x.AssetUnitId, x.Id });
                    table.ForeignKey(
                        name: "FK_AssetUnitHistories_AssetUnits_AssetUnitId",
                        column: x => x.AssetUnitId,
                        principalSchema: "asset",
                        principalTable: "AssetUnits",
                        principalColumn: "AssetUnitId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Assets_LaboratoryId",
                schema: "asset",
                table: "Assets",
                column: "LaboratoryId");

            migrationBuilder.CreateIndex(
                name: "IX_AssetUnitConditions_AssetUnitId",
                schema: "asset",
                table: "AssetUnitConditions",
                column: "AssetUnitId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AssetUnits_AssetId",
                schema: "asset",
                table: "AssetUnits",
                column: "AssetId");

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AssetUnitConditions",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "AssetUnitHistories",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "OutboxMessages",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "AssetUnits",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "Assets",
                schema: "asset");

            migrationBuilder.DropTable(
                name: "Laboratories",
                schema: "asset");
        }
    }
}
