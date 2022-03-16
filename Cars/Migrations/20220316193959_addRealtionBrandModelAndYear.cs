using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class addRealtionBrandModelAndYear : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DraftOrderDetails");

            migrationBuilder.DropTable(
                name: "DraftOrders");

            migrationBuilder.DropColumn(
                name: "Brand",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Model",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Vehicle");

            migrationBuilder.AddColumn<long>(
                name: "BrandID",
                table: "Vehicle",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "BrandModelID",
                table: "Vehicle",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModelYearID",
                table: "Vehicle",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_BrandID",
                table: "Vehicle",
                column: "BrandID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_BrandModelID",
                table: "Vehicle",
                column: "BrandModelID");

            migrationBuilder.CreateIndex(
                name: "IX_Vehicle_ModelYearID",
                table: "Vehicle",
                column: "ModelYearID");

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_Brand_BrandID",
                table: "Vehicle",
                column: "BrandID",
                principalTable: "Brand",
                principalColumn: "BrandID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_BrandModels_BrandModelID",
                table: "Vehicle",
                column: "BrandModelID",
                principalTable: "BrandModels",
                principalColumn: "ModelID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Vehicle_ModelYears_ModelYearID",
                table: "Vehicle",
                column: "ModelYearID",
                principalTable: "ModelYears",
                principalColumn: "ModelYearID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_Brand_BrandID",
                table: "Vehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_BrandModels_BrandModelID",
                table: "Vehicle");

            migrationBuilder.DropForeignKey(
                name: "FK_Vehicle_ModelYears_ModelYearID",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_BrandID",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_BrandModelID",
                table: "Vehicle");

            migrationBuilder.DropIndex(
                name: "IX_Vehicle_ModelYearID",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "BrandID",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "BrandModelID",
                table: "Vehicle");

            migrationBuilder.DropColumn(
                name: "ModelYearID",
                table: "Vehicle");

            migrationBuilder.AddColumn<string>(
                name: "Brand",
                table: "Vehicle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Model",
                table: "Vehicle",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Year",
                table: "Vehicle",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "DraftOrders",
                columns: table => new
                {
                    DraftOrderID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Brand = table.Column<string>(type: "text", nullable: true),
                    Chases = table.Column<string>(type: "text", nullable: true),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DTsUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    EmployeeBranchID = table.Column<long>(type: "bigint", nullable: false),
                    Enable = table.Column<bool>(type: "boolean", nullable: false),
                    Model = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Phone = table.Column<string>(type: "text", nullable: true),
                    SystemUserCreate = table.Column<string>(type: "text", nullable: false),
                    SystemUserUpdate = table.Column<string>(type: "text", nullable: true),
                    WithMaintenance = table.Column<bool>(type: "boolean", nullable: true),
                    Year = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftOrders", x => x.DraftOrderID);
                });

            migrationBuilder.CreateTable(
                name: "DraftOrderDetails",
                columns: table => new
                {
                    DraftOrderDetailsId = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    DTsUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true),
                    DraftOrderID = table.Column<long>(type: "bigint", nullable: false),
                    Items = table.Column<string>(type: "text", nullable: true),
                    OrderDetailsTypeID = table.Column<int>(type: "integer", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: true),
                    SystemUserCreate = table.Column<string>(type: "text", nullable: false),
                    SystemUserUpdate = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DraftOrderDetails", x => x.DraftOrderDetailsId);
                    table.ForeignKey(
                        name: "FK_DraftOrderDetails_DraftOrders_DraftOrderID",
                        column: x => x.DraftOrderID,
                        principalTable: "DraftOrders",
                        principalColumn: "DraftOrderID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DraftOrderDetails_OrderDetailsType_OrderDetailsTypeID",
                        column: x => x.OrderDetailsTypeID,
                        principalTable: "OrderDetailsType",
                        principalColumn: "OrderDetailsTypeID",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DraftOrderDetails_DraftOrderID",
                table: "DraftOrderDetails",
                column: "DraftOrderID");

            migrationBuilder.CreateIndex(
                name: "IX_DraftOrderDetails_OrderDetailsTypeID",
                table: "DraftOrderDetails",
                column: "OrderDetailsTypeID");
        }
    }
}
