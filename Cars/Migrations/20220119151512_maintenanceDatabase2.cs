using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class maintenanceDatabase2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_VendorLocations_VendorLocationID",
                table: "OrderDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Branches_VendorLocationID",
                table: "OrderDetails",
                column: "VendorLocationID",
                principalTable: "Branches",
                principalColumn: "BranchID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Branches_VendorLocationID",
                table: "OrderDetails");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_VendorLocations_VendorLocationID",
                table: "OrderDetails",
                column: "VendorLocationID",
                principalTable: "VendorLocations",
                principalColumn: "VendorLocationID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
