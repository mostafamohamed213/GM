using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "InventoryID",
                table: "OrderDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Inventory",
                columns: table => new
                {
                    InventoryID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    Reason = table.Column<string>(type: "text", nullable: true),
                    Quantity = table.Column<int>(type: "integer", nullable: false),
                    SystemUserCreate = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SystemUserUpdate = table.Column<string>(type: "text", nullable: true),
                    DTsUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inventory", x => x.InventoryID);
                });

            migrationBuilder.CreateTable(
                name: "InventoryDocuments",
                columns: table => new
                {
                    FinanceDocumentID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    InventoryID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InventoryDocuments", x => x.FinanceDocumentID);
                    table.ForeignKey(
                        name: "FK_InventoryDocuments_Inventory_InventoryID",
                        column: x => x.InventoryID,
                        principalTable: "Inventory",
                        principalColumn: "InventoryID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_InventoryID",
                table: "OrderDetails",
                column: "InventoryID");

            migrationBuilder.CreateIndex(
                name: "IX_InventoryDocuments_InventoryID",
                table: "InventoryDocuments",
                column: "InventoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Inventory_InventoryID",
                table: "OrderDetails",
                column: "InventoryID",
                principalTable: "Inventory",
                principalColumn: "InventoryID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Inventory_InventoryID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "InventoryDocuments");

            migrationBuilder.DropTable(
                name: "Inventory");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_InventoryID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "InventoryID",
                table: "OrderDetails");
        }
    }
}
