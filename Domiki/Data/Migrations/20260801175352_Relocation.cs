using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class Relocation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "last_relocation_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "memory_knots",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "relocation_count",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "valley_id",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "village_started_date",
                table: "players",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "player_perks",
                columns: table => new
                {
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    perk_type = table.Column<int>(type: "integer", nullable: false),
                    level = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_player_perks", x => new { x.player_id, x.perk_type });
                    table.ForeignKey(
                        name: "fk_player_perks_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "village_chronicles",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    player_id = table.Column<int>(type: "integer", nullable: false),
                    village_name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    crest_icon = table.Column<int>(type: "integer", nullable: false),
                    crest_color = table.Column<int>(type: "integer", nullable: false),
                    valley_id = table.Column<int>(type: "integer", nullable: false),
                    village_level = table.Column<int>(type: "integer", nullable: false),
                    knots = table.Column<int>(type: "integer", nullable: false),
                    start_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    end_date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_village_chronicles", x => x.id);
                    table.ForeignKey(
                        name: "fk_village_chronicles_players_player_id",
                        column: x => x.player_id,
                        principalTable: "players",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_village_chronicles_player_id",
                table: "village_chronicles",
                column: "player_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "player_perks");

            migrationBuilder.DropTable(
                name: "village_chronicles");

            migrationBuilder.DropColumn(
                name: "last_relocation_date",
                table: "players");

            migrationBuilder.DropColumn(
                name: "memory_knots",
                table: "players");

            migrationBuilder.DropColumn(
                name: "relocation_count",
                table: "players");

            migrationBuilder.DropColumn(
                name: "valley_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "village_started_date",
                table: "players");
        }
    }
}
