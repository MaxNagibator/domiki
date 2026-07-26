using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class ElderHouseMeasure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "measure_resource_type_id",
                table: "manufactures",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "measure_value",
                table: "manufactures",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "player_resource_reserves",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    resource_type_id = table.Column<int>(type: "integer", nullable: false),
                    reserve = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_resource_reserves", x => new { x.player_id, x.resource_type_id });
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_resource_reserves");

            migrationBuilder.DropColumn(
                name: "measure_resource_type_id",
                table: "manufactures");

            migrationBuilder.DropColumn(
                name: "measure_value",
                table: "manufactures");
        }
    }
}
