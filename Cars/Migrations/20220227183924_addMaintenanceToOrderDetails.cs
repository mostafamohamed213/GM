using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class addMaintenanceToOrderDetails : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Maintenance",
                table: "OrderDetails",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Maintenance",
                table: "OrderDetails");
        }
    }
}
