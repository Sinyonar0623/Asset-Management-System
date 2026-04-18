using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Request.Data.Migrations
{
    /// <inheritdoc />
    public partial class SimplifyRequestItems : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Note",
                schema: "request",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "QuantityApproved",
                schema: "request",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "QuantityRequested",
                schema: "request",
                table: "RequestItems");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                schema: "request",
                table: "RequestItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                schema: "request",
                table: "RequestItems",
                type: "timestamp with time zone",
                nullable: false,
                defaultValueSql: "CURRENT_TIMESTAMP");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                schema: "request",
                table: "RequestItems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                schema: "request",
                table: "RequestItems");

            migrationBuilder.AddColumn<string>(
                name: "Note",
                schema: "request",
                table: "RequestItems",
                type: "character varying(1000)",
                maxLength: 1000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityApproved",
                schema: "request",
                table: "RequestItems",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "QuantityRequested",
                schema: "request",
                table: "RequestItems",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
