using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class maintenanceDatabase : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Runners_RunnerID",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "RunnerID",
                table: "OrderDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_AspNetUsers_RunnerID",
                table: "OrderDetails",
                column: "RunnerID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_AspNetUsers_RunnerID",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<int>(
                name: "RunnerID",
                table: "OrderDetails",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Runners_RunnerID",
                table: "OrderDetails",
                column: "RunnerID",
                principalTable: "Runners",
                principalColumn: "RunnerID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
