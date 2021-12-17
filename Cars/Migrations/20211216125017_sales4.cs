using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class sales4 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Comments",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PartNumber",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Price",
                table: "OrderDetails",
                type: "numeric",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UsedByUser",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VendorLocationID",
                table: "OrderDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "VendorLocations",
                columns: table => new
                {
                    VendorLocationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    NameEn = table.Column<string>(type: "text", nullable: false),
                    NameAr = table.Column<string>(type: "text", nullable: true),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VendorLocations", x => x.VendorLocationID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_VendorLocationID",
                table: "OrderDetails",
                column: "VendorLocationID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_VendorLocations_VendorLocationID",
                table: "OrderDetails",
                column: "VendorLocationID",
                principalTable: "VendorLocations",
                principalColumn: "VendorLocationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_VendorLocations_VendorLocationID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "VendorLocations");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_VendorLocationID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Comments",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "PartNumber",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Price",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "UsedByUser",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "VendorLocationID",
                table: "OrderDetails");
        }
    }
}
