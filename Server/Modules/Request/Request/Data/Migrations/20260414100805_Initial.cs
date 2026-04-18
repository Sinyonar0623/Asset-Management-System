using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Request.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "request");

            migrationBuilder.CreateTable(
                name: "Requests",
                schema: "request",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestNo = table.Column<string>(type: "character varying(30)", maxLength: 30, nullable: false),
                    RequestType = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    TargetLaboratoryId = table.Column<Guid>(type: "uuid", nullable: false),
                    RequestedAssetCategory = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    RequesterId = table.Column<Guid>(type: "uuid", nullable: false),
                    Reason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: false),
                    CurrentStepNo = table.Column<int>(type: "integer", nullable: true),
                    NextApproverId = table.Column<Guid>(type: "uuid", nullable: true),
                    SubmittedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FinalizedOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Requests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RequestDetails",
                schema: "request",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Purpose = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    BorrowFrom = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    BorrowTo = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    IssueDescription = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RetireReason = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    ExtraNote = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestDetail_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "request",
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestItems",
                schema: "request",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssetId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuantityRequested = table.Column<int>(type: "integer", nullable: false),
                    QuantityApproved = table.Column<int>(type: "integer", nullable: true),
                    Note = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestItems_Requests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "request",
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RequestTrackings",
                schema: "request",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StepNo = table.Column<int>(type: "integer", nullable: false),
                    RequiredRoleCode = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    AssignedApproverId = table.Column<Guid>(type: "uuid", nullable: true),
                    Status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                    ActionByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    ActionOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    Comment = table.Column<string>(type: "character varying(2000)", maxLength: 2000, nullable: true),
                    IsCurrent = table.Column<bool>(type: "boolean", nullable: false),
                    RequestId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestTrackings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RequestTrackings_Requests_RequestId",
                        column: x => x.RequestId,
                        principalSchema: "request",
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_RequestDetails_RequestId",
                schema: "request",
                table: "RequestDetails",
                column: "RequestId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_AssetId",
                schema: "request",
                table: "RequestItems",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_RequestItems_RequestId_AssetId",
                schema: "request",
                table: "RequestItems",
                columns: new[] { "RequestId", "AssetId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequesterId",
                schema: "request",
                table: "Requests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestNo",
                schema: "request",
                table: "Requests",
                column: "RequestNo",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_Status_NextApproverId",
                schema: "request",
                table: "Requests",
                columns: new[] { "Status", "NextApproverId" });

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

            migrationBuilder.CreateIndex(
                name: "IX_RequestTrackings_AssignedApproverId_Status",
                schema: "request",
                table: "RequestTrackings",
                columns: new[] { "AssignedApproverId", "Status" });

            migrationBuilder.CreateIndex(
                name: "IX_RequestTrackings_RequestId",
                schema: "request",
                table: "RequestTrackings",
                column: "RequestId",
                unique: true,
                filter: "\"IsCurrent\" = TRUE");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTrackings_RequestId_StepNo",
                schema: "request",
                table: "RequestTrackings",
                columns: new[] { "RequestId", "StepNo" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RequestDetails",
                schema: "request");

            migrationBuilder.DropTable(
                name: "RequestItems",
                schema: "request");

            migrationBuilder.DropTable(
                name: "RequestTrackings",
                schema: "request");

            migrationBuilder.DropTable(
                name: "Requests",
                schema: "request");
        }
    }
}
