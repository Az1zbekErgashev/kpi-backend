using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_DATABASE_RELATIONS : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyPerformances_Goals_GoalId",
                table: "MonthlyPerformances");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 11, 35, 29, 987, DateTimeKind.Utc).AddTicks(6615), new DateTime(2025, 6, 25, 11, 35, 29, 987, DateTimeKind.Utc).AddTicks(6619) });

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyPerformances_Goals_GoalId",
                table: "MonthlyPerformances",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonthlyPerformances_Goals_GoalId",
                table: "MonthlyPerformances");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 7, 2, 45, 107, DateTimeKind.Utc).AddTicks(5627), new DateTime(2025, 6, 25, 7, 2, 45, 107, DateTimeKind.Utc).AddTicks(5629) });

            migrationBuilder.AddForeignKey(
                name: "FK_MonthlyPerformances_Goals_GoalId",
                table: "MonthlyPerformances",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
