using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class _UPDATE_COMMENT_TABLE : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 7, 2, 45, 107, DateTimeKind.Utc).AddTicks(5627), new DateTime(2025, 6, 25, 7, 2, 45, 107, DateTimeKind.Utc).AddTicks(5629) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 25, 6, 10, 51, 198, DateTimeKind.Utc).AddTicks(1383), new DateTime(2025, 6, 25, 6, 10, 51, 198, DateTimeKind.Utc).AddTicks(1385) });
        }
    }
}
