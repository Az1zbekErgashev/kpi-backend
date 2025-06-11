using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class ADD_POSITION_TABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_Goals_GoalId",
                table: "Divisions");

            migrationBuilder.DropForeignKey(
                name: "FK_KpiGoals_Divisions_DivisionId",
                table: "KpiGoals");

            migrationBuilder.AddColumn<int>(
                name: "PositionId",
                table: "Users",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Positions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    IsDeleted = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Positions", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Positions",
                columns: new[] { "Id", "CreatedAt", "IsDeleted", "Name", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "personal_information_manager", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 2, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "advisor", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 3, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "manager", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 4, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "technical_security_officer", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 5, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "head_of_technical_security", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 6, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "assistant_manager", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 7, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "ceo", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 8, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "vice_president", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 9, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "staff", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 10, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "executive_director", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 11, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "senior_researcher", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 12, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "principal_researcher", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 13, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "researcher", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 14, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "director", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 15, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "personnel_security_manager", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 16, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "info_security_committee_chair", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 17, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "info_security_committee_member", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 18, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "lead_researcher", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 19, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "deputy_department_head", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 20, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "responsible_researcher", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) },
                    { 21, new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc), 0, "chairman", new DateTime(2023, 11, 23, 16, 13, 56, 461, DateTimeKind.Utc) }
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "PositionId", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 6, 52, 53, 219, DateTimeKind.Utc).AddTicks(932), null, new DateTime(2025, 6, 11, 6, 52, 53, 219, DateTimeKind.Utc).AddTicks(935) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_PositionId",
                table: "Users",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Divisions_Goals_GoalId",
                table: "Divisions",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_KpiGoals_Divisions_DivisionId",
                table: "KpiGoals",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Positions_PositionId",
                table: "Users",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Divisions_Goals_GoalId",
                table: "Divisions");

            migrationBuilder.DropForeignKey(
                name: "FK_KpiGoals_Divisions_DivisionId",
                table: "KpiGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Positions_PositionId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Users_PositionId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "PositionId",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 9, 9, 55, 38, 943, DateTimeKind.Utc).AddTicks(4039), new DateTime(2025, 6, 9, 9, 55, 38, 943, DateTimeKind.Utc).AddTicks(4040) });

            migrationBuilder.AddForeignKey(
                name: "FK_Divisions_Goals_GoalId",
                table: "Divisions",
                column: "GoalId",
                principalTable: "Goals",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_KpiGoals_Divisions_DivisionId",
                table: "KpiGoals",
                column: "DivisionId",
                principalTable: "Divisions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
