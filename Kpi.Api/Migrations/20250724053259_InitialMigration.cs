using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 32, 58, 903, DateTimeKind.Utc).AddTicks(2518), new DateTime(2025, 7, 24, 5, 32, 58, 903, DateTimeKind.Utc).AddTicks(2520) });

            migrationBuilder.CreateIndex(
                name: "IX_ScoreManagements_DivisionId",
                table: "ScoreManagements",
                column: "DivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_ScoreManagements_Divisions_DivisionId",
                table: "ScoreManagements",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ScoreManagements_Divisions_DivisionId",
                table: "ScoreManagements");

            migrationBuilder.DropIndex(
                name: "IX_ScoreManagements_DivisionId",
                table: "ScoreManagements");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 24, 5, 8, 36, 66, DateTimeKind.Utc).AddTicks(5687), new DateTime(2025, 7, 24, 5, 8, 36, 66, DateTimeKind.Utc).AddTicks(5689) });
        }
    }
}
