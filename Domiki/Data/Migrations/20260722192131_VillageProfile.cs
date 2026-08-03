using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class VillageProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "profile_changed_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "profile_neighbor_id",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "village_profile_effects",
                columns: table => new
                {
                    neighbor_id = table.Column<int>(type: "integer", nullable: false),
                    domik_type_id = table.Column<int>(type: "integer", nullable: false),
                    duration_percent = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_village_profile_effects", x => new { x.neighbor_id, x.domik_type_id });
                    table.ForeignKey(
                        name: "fk_village_profile_effects_neighbors_neighbor_id",
                        column: x => x.neighbor_id,
                        principalTable: "neighbors",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData("village_profile_effects",
                columns: new[] { "neighbor_id", "domik_type_id", "duration_percent" },
                values: new object[,]
                {
                    { 1, 1, 85 }, { 1, 3, 85 },
                    { 2, 6, 85 }, { 2, 8, 85 },
                    { 3, 3, 85 }, { 3, 12, 85 },
                    { 4, 5, 85 }, { 4, 13, 85 },
                    { 5, 6, 85 }, { 5, 16, 85 },
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "village_profile_effects");

            migrationBuilder.DropColumn(
                name: "profile_changed_date",
                table: "players");

            migrationBuilder.DropColumn(
                name: "profile_neighbor_id",
                table: "players");
        }
    }
}
