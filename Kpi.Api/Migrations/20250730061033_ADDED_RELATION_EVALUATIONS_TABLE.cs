using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ADDED_RELATION_EVALUATIONS_TABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Divisions_KpiDivisionId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_ScoreManagements_ScoreManagementId",
                table: "Evaluations");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 30, 6, 10, 33, 559, DateTimeKind.Utc).AddTicks(9434), new DateTime(2025, 7, 30, 6, 10, 33, 559, DateTimeKind.Utc).AddTicks(9436) });

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Divisions_KpiDivisionId",
                table: "Evaluations",
                column: "KpiDivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_ScoreManagements_ScoreManagementId",
                table: "Evaluations",
                column: "ScoreManagementId",
                principalTable: "ScoreManagements",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Divisions_KpiDivisionId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_ScoreManagements_ScoreManagementId",
                table: "Evaluations");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 30, 6, 4, 48, 455, DateTimeKind.Utc).AddTicks(4205), new DateTime(2025, 7, 30, 6, 4, 48, 455, DateTimeKind.Utc).AddTicks(4206) });

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Divisions_KpiDivisionId",
                table: "Evaluations",
                column: "KpiDivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_ScoreManagements_ScoreManagementId",
                table: "Evaluations",
                column: "ScoreManagementId",
                principalTable: "ScoreManagements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
