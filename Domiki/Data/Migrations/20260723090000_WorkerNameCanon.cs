using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Web.Data.Migrations
{
    /// <inheritdoc />
    public partial class WorkerNameCanon : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Савва' WHERE \"name\" = 'Сава';");
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Агафья' WHERE \"name\" = 'Агата';");
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Аксинья' WHERE \"name\" = 'Есения';");
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Марфа' WHERE \"name\" = 'Марта';");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Сава' WHERE \"name\" = 'Савва';");
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Агата' WHERE \"name\" = 'Агафья';");
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Есения' WHERE \"name\" = 'Аксинья';");
            migrationBuilder.Sql("UPDATE \"workers\" SET \"name\" = 'Марта' WHERE \"name\" = 'Марфа';");
        }
    }
}
