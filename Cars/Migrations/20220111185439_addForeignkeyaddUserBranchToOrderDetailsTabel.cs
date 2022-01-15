using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class addForeignkeyaddUserBranchToOrderDetailsTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BranchID",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "UserBranchID",
                table: "OrderDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_UserBranchID",
                table: "OrderDetails",
                column: "UserBranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_UserBranches_UserBranchID",
                table: "OrderDetails",
                column: "UserBranchID",
                principalTable: "UserBranches",
                principalColumn: "UserBranchID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_UserBranches_UserBranchID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_UserBranchID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UserBranchID",
                table: "OrderDetails");

            migrationBuilder.AddColumn<int>(
                name: "BranchID",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
