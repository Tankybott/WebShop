using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccess.Migrations
{
    /// <inheritdoc />
    public partial class Aa : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Carrier",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "OrderTotal",
                table: "OrderHeaders");

            migrationBuilder.RenameColumn(
                name: "OrderDate",
                table: "OrderHeaders",
                newName: "OrderCreationDate");

            migrationBuilder.AddColumn<int>(
                name: "CarrierId",
                table: "OrderHeaders",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "ShippingPriceFactor",
                table: "OrderDetail",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateIndex(
                name: "IX_OrderHeaders_CarrierId",
                table: "OrderHeaders",
                column: "CarrierId");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderHeaders_Carrier_CarrierId",
                table: "OrderHeaders",
                column: "CarrierId",
                principalTable: "Carrier",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderHeaders_Carrier_CarrierId",
                table: "OrderHeaders");

            migrationBuilder.DropIndex(
                name: "IX_OrderHeaders_CarrierId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "CarrierId",
                table: "OrderHeaders");

            migrationBuilder.DropColumn(
                name: "ShippingPriceFactor",
                table: "OrderDetail");

            migrationBuilder.RenameColumn(
                name: "OrderCreationDate",
                table: "OrderHeaders",
                newName: "OrderDate");

            migrationBuilder.AddColumn<string>(
                name: "Carrier",
                table: "OrderHeaders",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "OrderTotal",
                table: "OrderHeaders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }
    }
}
