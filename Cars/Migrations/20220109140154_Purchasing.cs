using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class Purchasing : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Reason",
                table: "OrderDetailsStatusLogs",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowID",
                table: "OrderDetailsStatusLogs",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RunnerID",
                table: "OrderDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Runners",
                columns: table => new
                {
                    RunnerID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: false),
                    Enable = table.Column<bool>(type: "boolean", nullable: false),
                    SystemUserCreate = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    SystemUserUpdate = table.Column<string>(type: "text", nullable: true),
                    DTsUpdate = table.Column<DateTime>(type: "timestamp without time zone", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Runners", x => x.RunnerID);
                });

            migrationBuilder.CreateTable(
                name: "StatusLogDocuments",
                columns: table => new
                {
                    StatusLogDocumentID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Path = table.Column<string>(type: "text", nullable: false),
                    Details = table.Column<string>(type: "text", nullable: true),
                    OrderDetailsStatusLogID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StatusLogDocuments", x => x.StatusLogDocumentID);
                    table.ForeignKey(
                        name: "FK_StatusLogDocuments_OrderDetailsStatusLogs_OrderDetailsStatu~",
                        column: x => x.OrderDetailsStatusLogID,
                        principalTable: "OrderDetailsStatusLogs",
                        principalColumn: "OrderDetailsStatusLogID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailsStatusLogs_WorkflowID",
                table: "OrderDetailsStatusLogs",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_RunnerID",
                table: "OrderDetails",
                column: "RunnerID");

            migrationBuilder.CreateIndex(
                name: "IX_StatusLogDocuments_OrderDetailsStatusLogID",
                table: "StatusLogDocuments",
                column: "OrderDetailsStatusLogID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Runners_RunnerID",
                table: "OrderDetails",
                column: "RunnerID",
                principalTable: "Runners",
                principalColumn: "RunnerID",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetailsStatusLogs_Workflows_WorkflowID",
                table: "OrderDetailsStatusLogs",
                column: "WorkflowID",
                principalTable: "Workflows",
                principalColumn: "WorkflowID",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Runners_RunnerID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetailsStatusLogs_Workflows_WorkflowID",
                table: "OrderDetailsStatusLogs");

            migrationBuilder.DropTable(
                name: "Runners");

            migrationBuilder.DropTable(
                name: "StatusLogDocuments");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetailsStatusLogs_WorkflowID",
                table: "OrderDetailsStatusLogs");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_RunnerID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Reason",
                table: "OrderDetailsStatusLogs");

            migrationBuilder.DropColumn(
                name: "WorkflowID",
                table: "OrderDetailsStatusLogs");

            migrationBuilder.DropColumn(
                name: "RunnerID",
                table: "OrderDetails");
        }
    }
}
