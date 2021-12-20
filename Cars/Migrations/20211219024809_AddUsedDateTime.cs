using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class AddUsedDateTime : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "UsedDateTime",
                table: "OrderDetails",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedDateTime",
                table: "OrderDetails");
        }
    }
}
