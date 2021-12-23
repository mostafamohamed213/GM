using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class labor_attributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "Labor_Hours",
                table: "OrderDetails",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "Labor_Value",
                table: "OrderDetails",
                type: "double precision",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Labor_Hours",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Labor_Value",
                table: "OrderDetails");
        }
    }
}
