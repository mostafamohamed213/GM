using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class AddCanceledByUserIDAndDeletedByUserID : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CanceledByUserID",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserID",
                table: "OrderDetails",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CanceledByUserID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DeletedByUserID",
                table: "OrderDetails");
        }
    }
}
