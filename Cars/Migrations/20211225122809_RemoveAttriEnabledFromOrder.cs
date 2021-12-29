using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class RemoveAttriEnabledFromOrder : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "Orders");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "Orders",
                type: "boolean",
                nullable: true);
        }
    }
}
