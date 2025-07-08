using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_EVALUATIONS_TABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AttendanceScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "BaseWorkScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "FinalScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "SkillImprovementScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "TeamAttitudeScore",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "FinalGrade",
                table: "Evaluations",
                newName: "Comment");

            migrationBuilder.AddColumn<int>(
                name: "Grade",
                table: "Evaluations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "KpiDivisionId",
                table: "Evaluations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Modifier",
                table: "Evaluations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 8, 5, 11, 1, 299, DateTimeKind.Utc).AddTicks(4126), new DateTime(2025, 7, 8, 5, 11, 1, 299, DateTimeKind.Utc).AddTicks(4130) });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_KpiDivisionId",
                table: "Evaluations",
                column: "KpiDivisionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Divisions_KpiDivisionId",
                table: "Evaluations",
                column: "KpiDivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Divisions_KpiDivisionId",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_KpiDivisionId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "Grade",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "KpiDivisionId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "Modifier",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "Comment",
                table: "Evaluations",
                newName: "FinalGrade");

            migrationBuilder.AddColumn<double>(
                name: "AttendanceScore",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "BaseWorkScore",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "FinalScore",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "SkillImprovementScore",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<double>(
                name: "TeamAttitudeScore",
                table: "Evaluations",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 4, 4, 55, 58, 970, DateTimeKind.Utc).AddTicks(9548), new DateTime(2025, 7, 4, 4, 55, 58, 970, DateTimeKind.Utc).AddTicks(9550) });
        }
    }
}
