using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ADDED_PERFORMANCE_EVALUTION : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Users_AssignedToId",
                table: "Goals");

            migrationBuilder.DropTable(
                name: "MonthlyTargets");

            migrationBuilder.DropIndex(
                name: "IX_Goals_AssignedToId",
                table: "Goals");

            migrationBuilder.DropColumn(
                name: "AssignedToId",
                table: "Goals");

            migrationBuilder.CreateTable(
                name: "MonthlyPerformances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    GoalId = table.Column<int>(type: "integer", nullable: false),
                    Year = table.Column<int>(type: "integer", nullable: false),
                    Month = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    IsSended = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyPerformances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyPerformances_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyTargetComments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Content = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPerformanceId = table.Column<int>(type: "integer", nullable: false),
                    Status = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyTargetComments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyTargetComments_MonthlyPerformances_MonthlyPerformanc~",
                        column: x => x.MonthlyPerformanceId,
                        principalTable: "MonthlyPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonthlyTargetComments_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MonthlyTargetValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ValueRatio = table.Column<double>(type: "double precision", nullable: true),
                    ValueRatioStatus = table.Column<double>(type: "double precision", nullable: true),
                    ValueNumber = table.Column<double>(type: "double precision", nullable: true),
                    ValueText = table.Column<string>(type: "text", nullable: true),
                    TargetValueId = table.Column<int>(type: "integer", nullable: false),
                    MonthlyPerformanceId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyTargetValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyTargetValues_MonthlyPerformances_MonthlyPerformanceId",
                        column: x => x.MonthlyPerformanceId,
                        principalTable: "MonthlyPerformances",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 6, 10, 51, 198, DateTimeKind.Utc).AddTicks(1383), new DateTime(2025, 6, 25, 6, 10, 51, 198, DateTimeKind.Utc).AddTicks(1385) });

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyPerformances_GoalId",
                table: "MonthlyPerformances",
                column: "GoalId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargetComments_CreatedById",
                table: "MonthlyTargetComments",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargetComments_MonthlyPerformanceId",
                table: "MonthlyTargetComments",
                column: "MonthlyPerformanceId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargetValues_MonthlyPerformanceId",
                table: "MonthlyTargetValues",
                column: "MonthlyPerformanceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MonthlyTargetComments");

            migrationBuilder.DropTable(
                name: "MonthlyTargetValues");

            migrationBuilder.DropTable(
                name: "MonthlyPerformances");

            migrationBuilder.AddColumn<int>(
                name: "AssignedToId",
                table: "Goals",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "MonthlyTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    AssignedToId = table.Column<int>(type: "integer", nullable: true),
                    CreatedById = table.Column<int>(type: "integer", nullable: false),
                    GoalId = table.Column<int>(type: "integer", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    IsDeleted = table.Column<int>(type: "integer", nullable: false),
                    TargetEvaluationText = table.Column<string>(type: "text", nullable: true),
                    TargetValueNumber = table.Column<double>(type: "double precision", nullable: true),
                    TargetValueRatio = table.Column<double>(type: "double precision", nullable: true),
                    TargetValueText = table.Column<string>(type: "text", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MonthlyTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MonthlyTargets_Goals_GoalId",
                        column: x => x.GoalId,
                        principalTable: "Goals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MonthlyTargets_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MonthlyTargets_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 5, 50, 21, 852, DateTimeKind.Utc).AddTicks(6846), new DateTime(2025, 6, 19, 5, 50, 21, 852, DateTimeKind.Utc).AddTicks(6847) });

            migrationBuilder.CreateIndex(
                name: "IX_Goals_AssignedToId",
                table: "Goals",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargets_AssignedToId",
                table: "MonthlyTargets",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargets_CreatedById",
                table: "MonthlyTargets",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MonthlyTargets_GoalId",
                table: "MonthlyTargets",
                column: "GoalId");

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Users_AssignedToId",
                table: "Goals",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
