using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class NotificationUpdates : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DeliveryID",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_DeliveryID",
                table: "OrderDetails",
                column: "DeliveryID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_AspNetUsers_DeliveryID",
                table: "OrderDetails",
                column: "DeliveryID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_AspNetUsers_DeliveryID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_DeliveryID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DeliveryID",
                table: "OrderDetails");
        }
    }
}
