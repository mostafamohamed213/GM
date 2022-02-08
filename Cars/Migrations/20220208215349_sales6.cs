using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class sales6 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UsedByUser2",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UsedDateTime2",
                table: "OrderDetails",
                type: "timestamp without time zone",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UsedByUser2",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UsedDateTime2",
                table: "OrderDetails");
        }
    }
}
