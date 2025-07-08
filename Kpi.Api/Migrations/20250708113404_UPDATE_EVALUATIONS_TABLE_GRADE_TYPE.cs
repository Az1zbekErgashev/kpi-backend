using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_EVALUATIONS_TABLE_GRADE_TYPE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Modifier",
                table: "Evaluations",
                type: "text",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 8, 11, 34, 4, 590, DateTimeKind.Utc).AddTicks(4551), new DateTime(2025, 7, 8, 11, 34, 4, 590, DateTimeKind.Utc).AddTicks(4554) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Modifier",
                table: "Evaluations",
                type: "integer",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 8, 5, 11, 1, 299, DateTimeKind.Utc).AddTicks(4126), new DateTime(2025, 7, 8, 5, 11, 1, 299, DateTimeKind.Utc).AddTicks(4130) });
        }
    }
}
