using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Asset.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000101"));

            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000102"));

            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000103"));

            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000104"));

            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000105"));

            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000106"));

            migrationBuilder.DeleteData(
                schema: "asset",
                table: "Laboratories",
                keyColumn: "LaboratoriesId",
                keyValue: new Guid("00000000-0000-0000-0000-000000000107"));
        }
    }
}
