using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Asset.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
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
                    LaboratoriesId = table.Column<Guid>(type: "uuid", nullable: false),
                    LaboratoryName = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    RoomNo = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    TeacherId = table.Column<Guid>(type: "uuid", nullable: true),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laboratories", x => x.LaboratoriesId);
                });

            migrationBuilder.CreateTable(
                name: "Assets",
                schema: "asset",
                columns: table => new
                {
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Category = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    IsAvailable = table.Column<bool>(type: "boolean", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
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
                    AssetUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    AssetTag = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    SerialNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Brand = table.Column<string>(type: "text", nullable: false),
                    AvailabilityStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    OperationalStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    Remark = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    OwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
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
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "AssetUnitConditions",
                schema: "asset",
                columns: table => new
                {
                    AssetUnitConditionId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false),
                    EffectiveFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEffectiveFromUnknown = table.Column<bool>(type: "boolean", nullable: false),
                    EffectiveTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IsEffectiveToUnknown = table.Column<bool>(type: "boolean", nullable: false),
                    AssetUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
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
                    AssetUnitId = table.Column<Guid>(type: "uuid", nullable: false),
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ActionType = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    FromAvailabilityStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ToAvailabilityStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FromOperationalStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    ToOperationalStatus = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    FromOwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    ToOwnerId = table.Column<Guid>(type: "uuid", nullable: true),
                    PerformedBy = table.Column<Guid>(type: "uuid", nullable: false),
                    PerformedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    ApprovedBy = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    ReferenceNo = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: true),
                    Remark = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false)
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

            migrationBuilder.CreateTable(
                name: "AssetUnitImages",
                schema: "asset",
                columns: table => new
                {
                    AssetUnitImageId = table.Column<Guid>(type: "uuid", nullable: false),
                    ImageUrl = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
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

            migrationBuilder.InsertData(
                schema: "asset",
                table: "Laboratories",
                columns: new[] { "LaboratoriesId", "CreateBy", "Description", "LaboratoryName", "RoomNo", "TeacherId", "UpdateBy", "UpdateOn" },
                values: new object[,]
                {
                    { new Guid("00000000-0000-0000-0000-000000000101"), "SYSTEM", "Primary lab for programming courses", "CPE Programming Lab 1", "G-601", new Guid("00000000-0000-0000-0000-000000001001"), null, null },
                    { new Guid("00000000-0000-0000-0000-000000000102"), "SYSTEM", "Advanced programming and web development", "CPE Programming Lab 2", "G-602", new Guid("00000000-0000-0000-0000-000000001002"), null, null },
                    { new Guid("00000000-0000-0000-0000-000000000103"), "SYSTEM", "Networking, routing, and server configuration", "CPE Network Lab", "G-603", new Guid("00000000-0000-0000-0000-000000001003"), null, null },
                    { new Guid("00000000-0000-0000-0000-000000000104"), "SYSTEM", "Microcontroller and IoT experiments", "CPE Embedded Systems Lab", "G-604", new Guid("00000000-0000-0000-0000-000000001004"), null, null },
                    { new Guid("00000000-0000-0000-0000-000000000105"), "SYSTEM", "Digital logic and circuit practice", "CPE Hardware Lab", "G-605", new Guid("00000000-0000-0000-0000-000000001005"), null, null },
                    { new Guid("00000000-0000-0000-0000-000000000106"), "SYSTEM", "CPU and low-level system study", "CPE Computer Architecture Lab", "G-606", new Guid("00000000-0000-0000-0000-000000001006"), null, null },
                    { new Guid("00000000-0000-0000-0000-000000000107"), "SYSTEM", "Workspace for final year projects", "CPE Senior Project Lab", "G-607", new Guid("00000000-0000-0000-0000-000000001007"), null, null }
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
                name: "IX_AssetUnitImages_AssetUnitId",
                schema: "asset",
                table: "AssetUnitImages",
                column: "AssetUnitId");

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
                name: "AssetUnitImages",
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
