using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class setOnDelteForPhorURlSetsOnNull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoUrlSets_Products_ProductId",
                table: "PhotoUrlSets");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoUrlSets_Products_ProductId",
                table: "PhotoUrlSets",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PhotoUrlSets_Products_ProductId",
                table: "PhotoUrlSets");

            migrationBuilder.AddForeignKey(
                name: "FK_PhotoUrlSets_Products_ProductId",
                table: "PhotoUrlSets",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
