using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Data.Migrations
{
    public partial class CreateUniqueIndexAspNetUserId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Player_AspNetUserId",
                table: "Player",
                column: "AspNetUserId",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Player_AspNetUserId",
                table: "Player");
        }
    }
}
