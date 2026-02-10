using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Auth.Data.Migrations
{
    /// <inheritdoc />
    public partial class InitialAuthContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Auth",
                schema: "auth",
                table: "Auth");

            migrationBuilder.RenameTable(
                name: "Auth",
                schema: "auth",
                newName: "UserName",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_Auth_RoleId",
                schema: "auth",
                table: "UserName",
                newName: "IX_UserName_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserName",
                schema: "auth",
                table: "UserName",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_UserName",
                schema: "auth",
                table: "UserName");

            migrationBuilder.RenameTable(
                name: "UserName",
                schema: "auth",
                newName: "Auth",
                newSchema: "auth");

            migrationBuilder.RenameIndex(
                name: "IX_UserName_RoleId",
                schema: "auth",
                table: "Auth",
                newName: "IX_Auth_RoleId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Auth",
                schema: "auth",
                table: "Auth",
                column: "Id");
        }
    }
}
