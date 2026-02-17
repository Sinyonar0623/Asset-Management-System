using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                schema: "auth",
                table: "UserRole",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateOn",
                schema: "auth",
                table: "UserRole",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "auth",
                table: "UserRole",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                schema: "auth",
                table: "UserName",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateOn",
                schema: "auth",
                table: "UserName",
                type: "datetime2",
                nullable: true,
                defaultValueSql: "SYSUTCDATETIME()",
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "auth",
                table: "UserName",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "SYSTEM",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                schema: "auth",
                table: "UserRole",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateOn",
                schema: "auth",
                table: "UserRole",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "auth",
                table: "UserRole",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");

            migrationBuilder.AlterColumn<string>(
                name: "UpdateBy",
                schema: "auth",
                table: "UserName",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreateOn",
                schema: "auth",
                table: "UserName",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true,
                oldDefaultValueSql: "SYSUTCDATETIME()");

            migrationBuilder.AlterColumn<string>(
                name: "CreateBy",
                schema: "auth",
                table: "UserName",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldDefaultValue: "SYSTEM");
        }
    }
}
