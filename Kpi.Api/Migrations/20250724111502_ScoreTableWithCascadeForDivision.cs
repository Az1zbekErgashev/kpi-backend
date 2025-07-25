using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ScoreTableWithCascadeForDivision : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreManagements_Divisions_DivisionId",
                table: "ScoreManagements");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 11, 15, 1, 677, DateTimeKind.Utc).AddTicks(2530), new DateTime(2025, 7, 24, 11, 15, 1, 677, DateTimeKind.Utc).AddTicks(2538) });

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreManagements_Divisions_DivisionId",
                table: "ScoreManagements",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreManagements_Divisions_DivisionId",
                table: "ScoreManagements");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 6, 5, 50, 889, DateTimeKind.Utc).AddTicks(6327), new DateTime(2025, 7, 24, 6, 5, 50, 889, DateTimeKind.Utc).AddTicks(6329) });

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreManagements_Divisions_DivisionId",
                table: "ScoreManagements",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
