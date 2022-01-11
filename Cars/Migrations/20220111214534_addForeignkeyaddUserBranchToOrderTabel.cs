using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class addForeignkeyaddUserBranchToOrderTabel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EmployeeBranchID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "Prefix",
                table: "Orders");

            migrationBuilder.AddColumn<int>(
                name: "UserBranchID",
                table: "Orders",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserBranchID",
                table: "Orders",
                column: "UserBranchID");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_UserBranches_UserBranchID",
                table: "Orders",
                column: "UserBranchID",
                principalTable: "UserBranches",
                principalColumn: "UserBranchID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_UserBranches_UserBranchID",
                table: "Orders");

            migrationBuilder.DropIndex(
                name: "IX_Orders_UserBranchID",
                table: "Orders");

            migrationBuilder.DropColumn(
                name: "UserBranchID",
                table: "Orders");

            migrationBuilder.AddColumn<long>(
                name: "EmployeeBranchID",
                table: "Orders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<string>(
                name: "Prefix",
                table: "Orders",
                type: "text",
                nullable: true);
        }
    }
}
