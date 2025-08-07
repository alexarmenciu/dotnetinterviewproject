using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class Second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Description", "DueDate", "IsCompleted", "Title" },
                values: new object[,]
                {
                    { new Guid("11111111-1111-1111-1111-111111111111"), "This is the first seeded task.", new DateTime(2025, 8, 14, 0, 0, 0, 0, DateTimeKind.Utc), false, "Sample Task 1" },
                    { new Guid("22222222-2222-2222-2222-222222222222"), "This is the second seeded task.", new DateTime(2025, 8, 26, 0, 0, 0, 0, DateTimeKind.Utc), false, "Sample Task 2" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"));

            migrationBuilder.DeleteData(
                table: "Tasks",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"));
        }
    }
}
