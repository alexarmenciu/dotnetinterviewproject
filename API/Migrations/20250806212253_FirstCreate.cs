using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace API.Migrations
{
    /// <inheritdoc />
    public partial class FirstCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                });

            // Seed initial data
            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "Title", "Description", "DueDate", "IsCompleted" },
                values: new object[,]
                {
                    { Guid.NewGuid(), "Buy groceries", "Milk, Bread, Eggs", new DateTime(2025, 8, 7), false },
                    { Guid.NewGuid(), "Finish project", "Complete the .NET interview project", new DateTime(2025, 8, 10), false },
                    { Guid.NewGuid(), "Call Alex", "Discuss project requirements", new DateTime(2025, 8, 8), true }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");
        }
    }
}
