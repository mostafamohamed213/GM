using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class sales5 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "ParentOrderDetailsID",
                table: "OrderDetails",
                type: "bigint",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ParentOrderDetailsID",
                table: "OrderDetails");
        }
    }
}
