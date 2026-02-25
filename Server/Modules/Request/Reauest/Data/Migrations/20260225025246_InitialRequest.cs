using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Reauest.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialRequest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "request");

            migrationBuilder.CreateTable(
                name: "BorrowRequests",
                schema: "request",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetRealWorldId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    BorrowDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReturnDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Purpose = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovalRemark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ActualReturnDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BorrowRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RepairRequests",
                schema: "request",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RequesterId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RequesterName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetId = table.Column<long>(type: "bigint", nullable: false),
                    AssetName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    AssetRealWorldId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ProblemDescription = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ApprovalRemark = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ApprovedBy = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CompletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TechnicianNote = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CreateOn = table.Column<DateTime>(type: "datetime2", nullable: true, defaultValueSql: "SYSUTCDATETIME()"),
                    CreateBy = table.Column<string>(type: "nvarchar(max)", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdateBy = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RepairRequests", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_AssetId",
                schema: "request",
                table: "BorrowRequests",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_RequesterId",
                schema: "request",
                table: "BorrowRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_BorrowRequests_Status",
                schema: "request",
                table: "BorrowRequests",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequests_AssetId",
                schema: "request",
                table: "RepairRequests",
                column: "AssetId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequests_RequesterId",
                schema: "request",
                table: "RepairRequests",
                column: "RequesterId");

            migrationBuilder.CreateIndex(
                name: "IX_RepairRequests_Status",
                schema: "request",
                table: "RepairRequests",
                column: "Status");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BorrowRequests",
                schema: "request");

            migrationBuilder.DropTable(
                name: "RepairRequests",
                schema: "request");
        }
    }
}
