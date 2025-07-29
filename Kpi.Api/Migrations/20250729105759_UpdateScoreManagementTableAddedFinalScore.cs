using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kpi.Api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScoreManagementTableAddedFinalScore : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Users_CreatedById",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Rooms_RoomId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "ScoreManagements",
                newName: "MinScore");

            migrationBuilder.AlterColumn<int>(
                name: "DivisionId",
                table: "ScoreManagements",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AddColumn<int[]>(
                name: "Divisions",
                table: "ScoreManagements",
                type: "integer[]",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsFinalScore",
                table: "ScoreManagements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsMoreDivisions",
                table: "ScoreManagements",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<double>(
                name: "MaxScore",
                table: "ScoreManagements",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 29, 10, 57, 59, 189, DateTimeKind.Utc).AddTicks(7630), new DateTime(2025, 7, 29, 10, 57, 59, 189, DateTimeKind.Utc).AddTicks(7637) });

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Users_CreatedById",
                table: "Goals",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Rooms_RoomId",
                table: "Users",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Goals_Users_CreatedById",
                table: "Goals");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Rooms_RoomId",
                table: "Users");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Divisions",
                table: "ScoreManagements");

            migrationBuilder.DropColumn(
                name: "IsFinalScore",
                table: "ScoreManagements");

            migrationBuilder.DropColumn(
                name: "IsMoreDivisions",
                table: "ScoreManagements");

            migrationBuilder.DropColumn(
                name: "MaxScore",
                table: "ScoreManagements");

            migrationBuilder.RenameColumn(
                name: "MinScore",
                table: "ScoreManagements",
                newName: "Score");

            migrationBuilder.AlterColumn<int>(
                name: "DivisionId",
                table: "ScoreManagements",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "CreatedAt", "UpdatedAt" },
                values: new object[] { new DateTime(2025, 7, 29, 6, 21, 19, 166, DateTimeKind.Utc).AddTicks(3948), new DateTime(2025, 7, 29, 6, 21, 19, 166, DateTimeKind.Utc).AddTicks(3950) });

            migrationBuilder.AddForeignKey(
                name: "FK_Goals_Users_CreatedById",
                table: "Goals",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Rooms_RoomId",
                table: "Users",
                column: "RoomId",
                principalTable: "Rooms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Teams_TeamId",
                table: "Users",
                column: "TeamId",
                principalTable: "Teams",
                principalColumn: "Id");
        }
    }
}
