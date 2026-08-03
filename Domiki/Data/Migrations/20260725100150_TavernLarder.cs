using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TavernLarder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "player_food_rules",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    resource_type_id = table.Column<int>(type: "integer", nullable: false),
                    reserve = table.Column<int>(type: "integer", nullable: false),
                    forbidden = table.Column<bool>(type: "boolean", nullable: false),
                    eaten_today = table.Column<int>(type: "integer", nullable: false),
                    eaten_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_food_rules", x => new { x.player_id, x.resource_type_id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_food_rules");
        }
    }
}
