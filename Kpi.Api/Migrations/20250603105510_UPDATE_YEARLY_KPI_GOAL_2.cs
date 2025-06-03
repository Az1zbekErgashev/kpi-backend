using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_YEARLY_KPI_GOAL_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyTargets_KpiGoals_KpiGoalId",
                table: "MonthlyTargets");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyTargets_KpiGoalId",
                table: "MonthlyTargets");

            migrationBuilder.DropColumn(
                name: "KpiGoalId",
                table: "MonthlyTargets");

            migrationBuilder.RenameColumn(
                name: "Month",
                table: "MonthlyTargets",
                newName: "GoalId");

            migrationBuilder.AlterColumn<string>(
                name: "ValueText",
                table: "TargetValues",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "EvaluationText",
                table: "TargetValues",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "TargetValueText",
                table: "MonthlyTargets",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(100)",
                oldMaxLength: 100);

            migrationBuilder.AddColumn<string>(
                name: "TargetEvaluationText",
                table: "MonthlyTargets",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 3, 10, 55, 10, 566, DateTimeKind.Utc).AddTicks(6047), new DateTime(2025, 6, 3, 10, 55, 10, 566, DateTimeKind.Utc).AddTicks(6049) });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargets_GoalId",
                table: "MonthlyTargets",
                column: "GoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyTargets_Goals_GoalId",
                table: "MonthlyTargets",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyTargets_Goals_GoalId",
                table: "MonthlyTargets");

            migrationBuilder.DropIndex(
                name: "IX_MonthlyTargets_GoalId",
                table: "MonthlyTargets");

            migrationBuilder.DropColumn(
                name: "TargetEvaluationText",
                table: "MonthlyTargets");

            migrationBuilder.RenameColumn(
                name: "GoalId",
                table: "MonthlyTargets",
                newName: "Month");

            migrationBuilder.AlterColumn<string>(
                name: "ValueText",
                table: "TargetValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "EvaluationText",
                table: "TargetValues",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "TargetValueText",
                table: "MonthlyTargets",
                type: "character varying(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "KpiGoalId",
                table: "MonthlyTargets",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 3, 4, 11, 41, 77, DateTimeKind.Utc).AddTicks(452), new DateTime(2025, 6, 3, 4, 11, 41, 77, DateTimeKind.Utc).AddTicks(454) });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargets_KpiGoalId",
                table: "MonthlyTargets",
                column: "KpiGoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyTargets_KpiGoals_KpiGoalId",
                table: "MonthlyTargets",
                column: "KpiGoalId",
                principalTable: "KpiGoals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
