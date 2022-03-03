using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class addAlternativestoOrderDetalis : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_ParentOrderDetailsID",
                table: "OrderDetails",
                column: "ParentOrderDetailsID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_OrderDetails_ParentOrderDetailsID",
                table: "OrderDetails",
                column: "ParentOrderDetailsID",
                principalTable: "OrderDetails",
                principalColumn: "OrderDetailsID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_OrderDetails_ParentOrderDetailsID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_ParentOrderDetailsID",
                table: "OrderDetails");
        }
    }
}
