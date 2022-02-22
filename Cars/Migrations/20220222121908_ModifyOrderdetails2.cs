using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class ModifyOrderdetails2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "NotificationSent",
                table: "OrderDetails",
                type: "boolean",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NotificationSent",
                table: "OrderDetails");
        }
    }
}
