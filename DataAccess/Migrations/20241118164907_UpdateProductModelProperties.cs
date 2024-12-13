using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProductModelProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherPhotos",
                table: "Products",
                newName: "OtherPhotosUrls");

            migrationBuilder.RenameColumn(
                name: "MainPhoto",
                table: "Products",
                newName: "MainPhotoUrl");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "OtherPhotosUrls",
                table: "Products",
                newName: "OtherPhotos");

            migrationBuilder.RenameColumn(
                name: "MainPhotoUrl",
                table: "Products",
                newName: "MainPhoto");
        }
    }
}
