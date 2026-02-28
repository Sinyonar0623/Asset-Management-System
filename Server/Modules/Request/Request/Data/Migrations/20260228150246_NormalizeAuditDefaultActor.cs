using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Request.Data.Migrations
{
    /// <inheritdoc />
    public partial class NormalizeAuditDefaultActor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "request",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: " SYSTEM");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "request",
                table: "RequestDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: " SYSTEM");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "request",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: " SYSTEM");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "request",
                table: "Requests",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: " SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "request",
                table: "RequestDetails",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: " SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "request",
                table: "OutboxMessages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: " SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");
        }
    }
}
