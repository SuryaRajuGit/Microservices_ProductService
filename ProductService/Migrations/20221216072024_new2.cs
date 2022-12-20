using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductService.Migrations
{
    public partial class new2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Catalog_Catalog_CatalogId",
                table: "Catalog");

            migrationBuilder.DropIndex(
                name: "IX_Catalog_CatalogId",
                table: "Catalog");

            migrationBuilder.DropColumn(
                name: "CatalogId",
                table: "Catalog");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "CatalogId",
                table: "Catalog",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Catalog_CatalogId",
                table: "Catalog",
                column: "CatalogId");

            migrationBuilder.AddForeignKey(
                name: "FK_Catalog_Catalog_CatalogId",
                table: "Catalog",
                column: "CatalogId",
                principalTable: "Catalog",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
