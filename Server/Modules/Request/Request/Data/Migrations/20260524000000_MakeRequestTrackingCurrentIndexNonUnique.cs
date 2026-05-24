using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Request.Data;

#nullable disable

namespace Request.Data.Migrations
{
    [DbContext(typeof(RequestDbContext))]
    [Migration("20260524000000_MakeRequestTrackingCurrentIndexNonUnique")]
    public partial class MakeRequestTrackingCurrentIndexNonUnique : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestTrackings_RequestId",
                schema: "request",
                table: "RequestTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTrackings_RequestId",
                schema: "request",
                table: "RequestTrackings",
                column: "RequestId",
                filter: "\"IsCurrent\" = TRUE");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_RequestTrackings_RequestId",
                schema: "request",
                table: "RequestTrackings");

            migrationBuilder.CreateIndex(
                name: "IX_RequestTrackings_RequestId",
                schema: "request",
                table: "RequestTrackings",
                column: "RequestId",
                unique: true,
                filter: "\"IsCurrent\" = TRUE");
        }
    }
}
