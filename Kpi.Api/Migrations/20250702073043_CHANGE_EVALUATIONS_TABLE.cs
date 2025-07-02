using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class CHANGE_EVALUATIONS_TABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Goals_GoalId",
                table: "Evaluations");

            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Users_EvaluatedById",
                table: "Evaluations");

            migrationBuilder.DropIndex(
                name: "IX_Evaluations_GoalId",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "Comment",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "Grade",
                table: "Evaluations",
                newName: "FinalGrade");

            migrationBuilder.RenameColumn(
                name: "GoalId",
                table: "Evaluations",
                newName: "Year");

            migrationBuilder.RenameColumn(
                name: "EvaluatedById",
                table: "Evaluations",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_EvaluatedById",
                table: "Evaluations",
                newName: "IX_Evaluations_UserId");

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

            migrationBuilder.AddColumn<int>(
                name: "Month",
                table: "Evaluations",
                type: "integer",
                nullable: false,
                defaultValue: 0);

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
                values: new object[] { new DateTime(2025, 7, 2, 7, 30, 42, 850, DateTimeKind.Utc).AddTicks(5868), new DateTime(2025, 7, 2, 7, 30, 42, 850, DateTimeKind.Utc).AddTicks(5871) });

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Users_UserId",
                table: "Evaluations",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Evaluations_Users_UserId",
                table: "Evaluations");

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
                name: "Month",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "SkillImprovementScore",
                table: "Evaluations");

            migrationBuilder.DropColumn(
                name: "TeamAttitudeScore",
                table: "Evaluations");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Evaluations",
                newName: "GoalId");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Evaluations",
                newName: "EvaluatedById");

            migrationBuilder.RenameColumn(
                name: "FinalGrade",
                table: "Evaluations",
                newName: "Grade");

            migrationBuilder.RenameIndex(
                name: "IX_Evaluations_UserId",
                table: "Evaluations",
                newName: "IX_Evaluations_EvaluatedById");

            migrationBuilder.AddColumn<string>(
                name: "Comment",
                table: "Evaluations",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 11, 35, 29, 987, DateTimeKind.Utc).AddTicks(6615), new DateTime(2025, 6, 25, 11, 35, 29, 987, DateTimeKind.Utc).AddTicks(6619) });

            migrationBuilder.CreateIndex(
                name: "IX_Evaluations_GoalId",
                table: "Evaluations",
                column: "GoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Goals_GoalId",
                table: "Evaluations",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Evaluations_Users_EvaluatedById",
                table: "Evaluations",
                column: "EvaluatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
