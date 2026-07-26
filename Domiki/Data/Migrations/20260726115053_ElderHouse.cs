using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ElderHouse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_labor_days",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    worked_seconds = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_labor_days", x => new { x.player_id, x.date });
                });

            migrationBuilder.CreateTable(
                name: "player_resource_flows",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    resource_type_id = table.Column<int>(type: "integer", nullable: false),
                    gained = table.Column<int>(type: "integer", nullable: false),
                    spent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_resource_flows", x => new { x.player_id, x.date, x.resource_type_id });
                });

            migrationBuilder.InsertData("domik_types",
                columns: new[] { "id", "name", "logic_name", "max_count", "unlock_level" },
                values: new object[] { 19, "Изба старосты", "elder_house", 1, 32 });

            migrationBuilder.InsertData("domik_type_levels",
                columns: new[] { "domik_type_id", "value", "upgrade_seconds", "max_manufacture_count" },
                values: new object[,]
                {
                    { 19, 1, 60, 0 },
                    { 19, 2, 3600, 0 },
                    { 19, 3, 36000, 0 },
                });

            migrationBuilder.InsertData("domik_type_level_resources",
                columns: new[] { "domik_type_level_domik_type_id", "domik_type_level_value", "resource_type_id", "value" },
                values: new object[,]
                {
                    { 19, 1, 1, 400 }, { 19, 1, 7, 10 },
                    { 19, 2, 1, 1200 }, { 19, 2, 6, 20 },
                    { 19, 3, 1, 3000 }, { 19, 3, 6, 30 }, { 19, 3, 7, 20 }, { 19, 3, 17, 10 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "domik_type_level_resources",
                keyColumns: new[] { "domik_type_level_domik_type_id", "domik_type_level_value", "resource_type_id" },
                keyValues: new object[,]
                {
                    { 19, 1, 1 }, { 19, 1, 7 },
                    { 19, 2, 1 }, { 19, 2, 6 },
                    { 19, 3, 1 }, { 19, 3, 6 }, { 19, 3, 7 }, { 19, 3, 17 },
                });

            migrationBuilder.DeleteData(
                table: "domik_type_levels",
                keyColumns: new[] { "domik_type_id", "value" },
                keyValues: new object[,]
                {
                    { 19, 1 },
                    { 19, 2 },
                    { 19, 3 },
                });

            migrationBuilder.DeleteData(
                table: "domik_types",
                keyColumn: "id",
                keyValue: 19);

            migrationBuilder.DropTable(
                name: "player_labor_days");

            migrationBuilder.DropTable(
                name: "player_resource_flows");
        }
    }
}
