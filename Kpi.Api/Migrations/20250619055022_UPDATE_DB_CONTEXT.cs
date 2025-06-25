using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UPDATE_DB_CONTEXT : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_UserName",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 19, 5, 50, 21, 852, DateTimeKind.Utc).AddTicks(6846), new DateTime(2025, 6, 19, 5, 50, 21, 852, DateTimeKind.Utc).AddTicks(6847) });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 6, 11, 6, 52, 53, 219, DateTimeKind.Utc).AddTicks(932), new DateTime(2025, 6, 11, 6, 52, 53, 219, DateTimeKind.Utc).AddTicks(935) });

            migrationBuilder.CreateIndex(
                name: "IX_Users_UserName",
                table: "Users",
                column: "UserName",
                unique: true);
        }
    }
}
