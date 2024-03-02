using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDBAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateModelsAndServices : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IssueDate",
                table: "Chips");

            migrationBuilder.AddColumn<string>(
                name: "ChipInfo",
                table: "Chips",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChipInfo",
                table: "Chips");

            migrationBuilder.AddColumn<DateTime>(
                name: "IssueDate",
                table: "Chips",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
