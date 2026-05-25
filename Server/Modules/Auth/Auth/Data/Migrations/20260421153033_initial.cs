using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "auth");

            migrationBuilder.CreateTable(
                name: "UserRole",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    RoleCode = table.Column<string>(type: "text", nullable: false),
                    RoleName = table.Column<string>(type: "text", nullable: false),
                    RoleDescription = table.Column<string>(type: "text", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserRole", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserName",
                schema: "auth",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Username = table.Column<string>(type: "text", nullable: false),
                    Email = table.Column<string>(type: "text", nullable: false),
                    PasswordHash = table.Column<string>(type: "text", nullable: false),
                    LaboratoryId = table.Column<Guid>(type: "uuid", nullable: true),
                    Session = table.Column<Guid>(type: "uuid", nullable: true),
                    SessionActiveOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    RoleId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true, defaultValueSql: "CURRENT_TIMESTAMP"),
                    CreateBy = table.Column<string>(type: "text", nullable: false, defaultValue: "SYSTEM"),
                    UpdateOn = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UpdateBy = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserName", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auth_RoleId",
                        column: x => x.RoleId,
                        principalSchema: "auth",
                        principalTable: "UserRole",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                schema: "auth",
                table: "UserRole",
                columns: new[] { "Id", "CreateBy", "RoleCode", "RoleDescription", "RoleName", "UpdateBy", "UpdateOn" },
                values: new object[,]
                {
                    { new Guid("1b4dc80d-c3e8-4e6d-a9d6-2bbd478d2d00"), "SYSTEM", "ADMIN", "System administrator", "ADMIN", null, null },
                    { new Guid("52400f2b-0eaf-4921-9ad7-e527fb52bb02"), "SYSTEM", "TEACHER", "Lecturer", "TEACHER", null, null },
                    { new Guid("b89541a9-8ce6-4e95-820d-7c7785f96f01"), "SYSTEM", "HOD", "Department head", "HOD", null, null },
                    { new Guid("f801e95d-340b-4f34-8a6a-9deac4168003"), "SYSTEM", "STUDENT", "Student", "STUDENT", null, null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserName_RoleId",
                schema: "auth",
                table: "UserName",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserName",
                schema: "auth");

            migrationBuilder.DropTable(
                name: "UserRole",
                schema: "auth");
        }
    }
}
