using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_USER_NAME_ADMIN : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt", "UserName" },
                values: new object[] { new DateTime(2025, 7, 4, 4, 55, 58, 970, DateTimeKind.Utc).AddTicks(9548), new DateTime(2025, 7, 4, 4, 55, 58, 970, DateTimeKind.Utc).AddTicks(9550), "CEO" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt", "UserName" },
                values: new object[] { new DateTime(2025, 7, 2, 7, 30, 42, 850, DateTimeKind.Utc).AddTicks(5868), new DateTime(2025, 7, 2, 7, 30, 42, 850, DateTimeKind.Utc).AddTicks(5871), "admin" });
        }
    }
}
