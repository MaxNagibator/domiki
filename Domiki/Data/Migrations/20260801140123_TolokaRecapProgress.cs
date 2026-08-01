using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class TolokaRecapProgress : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "recap_toloka_id",
                table: "players",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "recap_toloka_permille",
                table: "players",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "recap_toloka_id",
                table: "players");

            migrationBuilder.DropColumn(
                name: "recap_toloka_permille",
                table: "players");
        }
    }
}
