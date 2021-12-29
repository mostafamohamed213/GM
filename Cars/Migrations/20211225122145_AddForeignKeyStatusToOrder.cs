using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class AddForeignKeyStatusToOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "Orders",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_StatusID",
                table: "Orders",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Status_StatusID",
                table: "Orders",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Status_StatusID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_StatusID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "Orders");
        }
    }
}
