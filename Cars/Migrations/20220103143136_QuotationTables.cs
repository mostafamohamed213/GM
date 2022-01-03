using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class QuotationTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "QuotationID",
                table: "OrderDetails",
                type: "bigint",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branches",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BranchIP",
                table: "Branches",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "Quotations",
                columns: table => new
                {
                    QuotationID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StatusID = table.Column<int>(type: "integer", nullable: false),
                    Confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    SystemUserCreate = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SystemUserUpdate = table.Column<string>(type: "text", nullable: true),
                    DTsUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Quotations", x => x.QuotationID);
                    table.ForeignKey(
                        name: "FK_Quotations_Status_StatusID",
                        column: x => x.StatusID,
                        principalTable: "Status",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationDocuments",
                columns: table => new
                {
                    QuotationDocumentID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "text", nullable: true),
                    Comment = table.Column<string>(type: "text", nullable: true),
                    QuotationID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationDocuments", x => x.QuotationDocumentID);
                    table.ForeignKey(
                        name: "FK_QuotationDocuments_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuotationStatusLogs",
                columns: table => new
                {
                    QuotationStatusLogID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Detatils = table.Column<string>(type: "text", nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: false),
                    QuotationID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuotationStatusLogs", x => x.QuotationStatusLogID);
                    table.ForeignKey(
                        name: "FK_QuotationStatusLogs_Quotations_QuotationID",
                        column: x => x.QuotationID,
                        principalTable: "Quotations",
                        principalColumn: "QuotationID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuotationStatusLogs_Status_StatusID",
                        column: x => x.StatusID,
                        principalTable: "Status",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_QuotationID",
                table: "OrderDetails",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationDocuments_QuotationID",
                table: "QuotationDocuments",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_Quotations_StatusID",
                table: "Quotations",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationStatusLogs_QuotationID",
                table: "QuotationStatusLogs",
                column: "QuotationID");

            migrationBuilder.CreateIndex(
                name: "IX_QuotationStatusLogs_StatusID",
                table: "QuotationStatusLogs",
                column: "StatusID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Quotations_QuotationID",
                table: "OrderDetails",
                column: "QuotationID",
                principalTable: "Quotations",
                principalColumn: "QuotationID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Quotations_QuotationID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "QuotationDocuments");

            migrationBuilder.DropTable(
                name: "QuotationStatusLogs");

            migrationBuilder.DropTable(
                name: "Quotations");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_QuotationID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "QuotationID",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Branches",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AlterColumn<string>(
                name: "BranchIP",
                table: "Branches",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");
        }
    }
}
