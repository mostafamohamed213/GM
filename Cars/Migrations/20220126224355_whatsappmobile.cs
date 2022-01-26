using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class whatsappmobile : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "Mobile",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "Whatsapp",
                table: "AspNetUsers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Mobile",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "Whatsapp",
                table: "AspNetUsers");
        }
    }
}
