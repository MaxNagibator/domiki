﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Domiki.Data.Migrations
{
    public partial class RenameDomik : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Domik",
                table: "Domik");

            migrationBuilder.RenameTable(
                name: "Domik",
                newName: "Domiks");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Domiks",
                table: "Domiks",
                columns: new[] { "PlayerId", "Id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Domiks",
                table: "Domiks");

            migrationBuilder.RenameTable(
                name: "Domiks",
                newName: "Domik");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Domik",
                table: "Domik",
                columns: new[] { "PlayerId", "Id" });
        }
    }
}