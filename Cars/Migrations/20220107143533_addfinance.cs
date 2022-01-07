using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class addfinance : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FinanceID",
                table: "OrderDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Finances",
                columns: table => new
                {
                    FinanceID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SystemUserCreate = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SystemUserUpdate = table.Column<string>(type: "text", nullable: true),
                    DTsUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Finances", x => x.FinanceID);
                });

            migrationBuilder.CreateTable(
                name: "FinanceDocuments",
                columns: table => new
                {
                    FinanceDocumentID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    FinanceID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinanceDocuments", x => x.FinanceDocumentID);
                    table.ForeignKey(
                        name: "FK_FinanceDocuments_Finances_FinanceID",
                        column: x => x.FinanceID,
                        principalTable: "Finances",
                        principalColumn: "FinanceID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_FinanceID",
                table: "OrderDetails",
                column: "FinanceID");

            migrationBuilder.CreateIndex(
                name: "IX_FinanceDocuments_FinanceID",
                table: "FinanceDocuments",
                column: "FinanceID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Finances_FinanceID",
                table: "OrderDetails",
                column: "FinanceID",
                principalTable: "Finances",
                principalColumn: "FinanceID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Finances_FinanceID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "FinanceDocuments");

            migrationBuilder.DropTable(
                name: "Finances");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_FinanceID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "FinanceID",
                table: "OrderDetails");
        }
    }
}
