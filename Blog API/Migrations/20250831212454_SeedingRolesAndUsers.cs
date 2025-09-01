using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog_API.Migrations
{
    /// <inheritdoc />
    public partial class SeedingRolesAndUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137", "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137", "Admin", "ADMIN" },
                    { "4E204EAB-9CE3-463C-BB1B-4326B11659C5", "4E204EAB-9CE3-463C-BB1B-4326B11659C5", "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "ConcurrencyStamp", "Email", "EmailConfirmed", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2", 0, "5F4AA7CD-65EB-4821-B950-6D07B3C213FE", "admin@xvibes.com", true, false, null, "ADMIN@XVIBES.COM", "ADMIN", null, null, false, "71E0A7E3-54EC-4757-ACEA-6141748F4C05", false, "Admin" },
                    { "EC110106-E170-4C57-9EB3-1697F278E6E7", 0, "479B7E50-DCB4-4000-ADFA-7D755E946CD5", "user@gmail.com", true, false, null, "USER@GMAIL.COM", "DEFAULT USER", null, null, false, "F2FCAB9D-87F2-4CAD-A02E-685C9C4B5DF3", false, "Default User" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137", "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2" },
                    { "4E204EAB-9CE3-463C-BB1B-4326B11659C5", "EC110106-E170-4C57-9EB3-1697F278E6E7" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137", "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "4E204EAB-9CE3-463C-BB1B-4326B11659C5", "EC110106-E170-4C57-9EB3-1697F278E6E7" });

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "3DF17BFA-FB74-4D9A-B4F9-CFF374FC7137");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "4E204EAB-9CE3-463C-BB1B-4326B11659C5");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "EC110106-E170-4C57-9EB3-1697F278E6E7");
        }
    }
}
