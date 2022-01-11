using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class add_attributes_to_vendorLocation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DTsCreate",
                table: "VendorLocations",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<bool>(
                name: "Enable",
                table: "VendorLocations",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "SystemUserCreate",
                table: "VendorLocations",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DTsCreate",
                table: "VendorLocations");

            migrationBuilder.DropColumn(
                name: "Enable",
                table: "VendorLocations");

            migrationBuilder.DropColumn(
                name: "SystemUserCreate",
                table: "VendorLocations");
        }
    }
}
