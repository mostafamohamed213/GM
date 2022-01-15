using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class SystemUserUpdate_and_DTsUpdate_to_VendorLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DTsUpdate",
                table: "VendorLocations",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemUserUpdate",
                table: "VendorLocations",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DTsUpdate",
                table: "VendorLocations");

            migrationBuilder.DropColumn(
                name: "SystemUserUpdate",
                table: "VendorLocations");
        }
    }
}
