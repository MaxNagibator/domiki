using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class BarracksLadder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE domik_types SET max_count = 8 WHERE logic_name = 'barracks';");
            migrationBuilder.Sql(
                """
                INSERT INTO domik_type_count_gates (domik_type_id, ordinal, unlock_level)
                SELECT t.id, g.ordinal, g.unlock_level
                FROM domik_types t
                CROSS JOIN (VALUES (6, 60), (7, 110), (8, 175)) AS g(ordinal, unlock_level)
                WHERE t.logic_name = 'barracks';
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM domik_type_count_gates
                WHERE ordinal IN (6, 7, 8)
                  AND domik_type_id IN (SELECT id FROM domik_types WHERE logic_name = 'barracks');
                """);
            migrationBuilder.Sql("UPDATE domik_types SET max_count = 5 WHERE logic_name = 'barracks';");
        }
    }
}
