using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScoremanagementTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "Score",
                table: "ScoreManagements",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 6, 5, 50, 889, DateTimeKind.Utc).AddTicks(6327), new DateTime(2025, 7, 24, 6, 5, 50, 889, DateTimeKind.Utc).AddTicks(6329) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Score",
                table: "ScoreManagements",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 32, 58, 903, DateTimeKind.Utc).AddTicks(2518), new DateTime(2025, 7, 24, 5, 32, 58, 903, DateTimeKind.Utc).AddTicks(2520) });
        }
    }
}
