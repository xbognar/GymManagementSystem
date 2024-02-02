using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GymDBAccess.Migrations
{
    /// <inheritdoc />
    public partial class RemoveUnnecessaryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GymAccessRecords");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GymAccessRecords",
                columns: table => new
                {
                    AccessID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ChipID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GymAccessRecords", x => x.AccessID);
                });
        }
    }
}
