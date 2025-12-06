using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Blog_API.Migrations
{
    /// <inheritdoc />
    public partial class AddBlogCategoryAsATableAndSeedingCategories : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BlogCategory",
                table: "BlogPosts");

            migrationBuilder.AddColumn<Guid>(
                name: "BlogCategoryId",
                table: "BlogPosts",
                type: "uniqueidentifier",
                nullable: false);

            migrationBuilder.CreateTable(
                name: "BlogCategories",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BlogCategories", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEP7Q1CbK9lF5dTD+pwqljzNdGxiJ1Rf6sxyCCba4OFNoZ4WR2TR4H4iIQUxViG8yRA==");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "EC110106-E170-4C57-9EB3-1697F278E6E7",
                column: "PasswordHash",
                value: "AQAAAAIAAYagAAAAEBKxyIbI8dextG2jFrzngwYaxw2fMlNfVQZEGnh7iE3BP2QFctZWUsvGkQiYLE3hLQ==");

            migrationBuilder.InsertData(
                table: "BlogCategories",
                columns: new[] { "Id", "Name" },
                values: new object[,]
                {
                    { new Guid("3746f212-985b-49c5-88b7-b18f803c17f8"), "FOOD" },
                    { new Guid("42df7a0c-e7ab-4c8f-83ef-a469eee86bbe"), "LIFESTYLE" },
                    { new Guid("4978ac5f-f634-4da7-bee1-c535798ca75b"), "ENTERTAINMENT" },
                    { new Guid("4beccfe6-5bc8-4497-9c6c-42340f1e0310"), "SPORTS" },
                    { new Guid("63f7ee84-62c1-43a1-abd7-2d08f8c3d9be"), "TECHNOLOGY" },
                    { new Guid("6b12653e-ac69-4c06-922b-81337a62457d"), "SCIENCE" },
                    { new Guid("6d8772fc-3234-462c-8d03-160530c32638"), "TRAVEL" },
                    { new Guid("79ad67a0-9fc0-44c4-93e3-48421a3c814c"), "BUSINESS" },
                    { new Guid("807ec475-addc-48f6-b712-963d8f9ce788"), "EDUCATION" },
                    { new Guid("8321f4f0-b946-4f9a-aab6-bf3a5696f06d"), "OPINION" },
                    { new Guid("8c22c7e2-b967-45fb-9260-e083ffaabf67"), "NEWS" },
                    { new Guid("91d5137e-599f-498c-84aa-a78c9d4c908f"), "FINANCE" },
                    { new Guid("c2985499-a435-4b59-83bf-42539a593dcc"), "HEALTH" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_BlogPosts_BlogCategoryId",
                table: "BlogPosts",
                column: "BlogCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_BlogCategories_Name",
                table: "BlogCategories",
                column: "Name",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BlogPosts_BlogCategories_BlogCategoryId",
                table: "BlogPosts",
                column: "BlogCategoryId",
                principalTable: "BlogCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BlogPosts_BlogCategories_BlogCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropTable(
                name: "BlogCategories");

            migrationBuilder.DropIndex(
                name: "IX_BlogPosts_BlogCategoryId",
                table: "BlogPosts");

            migrationBuilder.DropColumn(
                name: "BlogCategoryId",
                table: "BlogPosts");

            migrationBuilder.AddColumn<string>(
                name: "BlogCategory",
                table: "BlogPosts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "D19A77BA-2F5E-4A95-A29F-32B5FF1C54E2",
                column: "PasswordHash",
                value: null);

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "EC110106-E170-4C57-9EB3-1697F278E6E7",
                column: "PasswordHash",
                value: null);
        }
    }
}
