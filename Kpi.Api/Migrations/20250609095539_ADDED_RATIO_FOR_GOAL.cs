using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ADDED_RATIO_FOR_GOAL : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Ratio",
                table: "Divisions",
                type: "double precision",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 9, 55, 38, 943, DateTimeKind.Utc).AddTicks(4039), new DateTime(2025, 6, 9, 9, 55, 38, 943, DateTimeKind.Utc).AddTicks(4040) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ratio",
                table: "Divisions");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 3, 10, 55, 10, 566, DateTimeKind.Utc).AddTicks(6047), new DateTime(2025, 6, 3, 10, 55, 10, 566, DateTimeKind.Utc).AddTicks(6049) });
        }
    }
}
