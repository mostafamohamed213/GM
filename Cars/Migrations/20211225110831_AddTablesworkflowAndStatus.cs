using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class AddTablesworkflowAndStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Layers_LayerID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "Layers");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_LayerID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "CanceledByUserID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DTsUpdate",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "DeletedByUserID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "Enabled",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "LayerID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "SystemUserUpdate",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "SystemUserCreate",
                table: "OrderDetails",
                type: "text",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "StatusID",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "WorkflowID",
                table: "OrderDetails",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Status",
                columns: table => new
                {
                    StatusID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Status", x => x.StatusID);
                });

            migrationBuilder.CreateTable(
                name: "Workflows",
                columns: table => new
                {
                    WorkflowID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Workflows", x => x.WorkflowID);
                });

            migrationBuilder.CreateTable(
                name: "OrderDetailsStatusLogs",
                columns: table => new
                {
                    OrderDetailsStatusLogLogID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Detatils = table.Column<string>(type: "text", nullable: true),
                    StatusID = table.Column<int>(type: "integer", nullable: false),
                    OrderDetailsID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrderDetailsStatusLogs", x => x.OrderDetailsStatusLogLogID);
                    table.ForeignKey(
                        name: "FK_OrderDetailsStatusLogs_OrderDetails_OrderDetailsID",
                        column: x => x.OrderDetailsID,
                        principalTable: "OrderDetails",
                        principalColumn: "OrderDetailsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrderDetailsStatusLogs_Status_StatusID",
                        column: x => x.StatusID,
                        principalTable: "Status",
                        principalColumn: "StatusID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WorkflowOrderDetailsLogs",
                columns: table => new
                {
                    WorkflowOrderDetailsLogID = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Active = table.Column<bool>(type: "boolean", nullable: false),
                    WorkflowID = table.Column<int>(type: "integer", nullable: false),
                    OrderDetailsID = table.Column<long>(type: "bigint", nullable: false),
                    SystemUserID = table.Column<string>(type: "text", nullable: false),
                    DTsCreate = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkflowOrderDetailsLogs", x => x.WorkflowOrderDetailsLogID);
                    table.ForeignKey(
                        name: "FK_WorkflowOrderDetailsLogs_OrderDetails_OrderDetailsID",
                        column: x => x.OrderDetailsID,
                        principalTable: "OrderDetails",
                        principalColumn: "OrderDetailsID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_WorkflowOrderDetailsLogs_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_StatusID",
                table: "OrderDetails",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_WorkflowID",
                table: "OrderDetails",
                column: "WorkflowID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailsStatusLogs_OrderDetailsID",
                table: "OrderDetailsStatusLogs",
                column: "OrderDetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetailsStatusLogs_StatusID",
                table: "OrderDetailsStatusLogs",
                column: "StatusID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowOrderDetailsLogs_OrderDetailsID",
                table: "WorkflowOrderDetailsLogs",
                column: "OrderDetailsID");

            migrationBuilder.CreateIndex(
                name: "IX_WorkflowOrderDetailsLogs_WorkflowID",
                table: "WorkflowOrderDetailsLogs",
                column: "WorkflowID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Status_StatusID",
                table: "OrderDetails",
                column: "StatusID",
                principalTable: "Status",
                principalColumn: "StatusID",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Workflows_WorkflowID",
                table: "OrderDetails",
                column: "WorkflowID",
                principalTable: "Workflows",
                principalColumn: "WorkflowID",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Status_StatusID",
                table: "OrderDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_OrderDetails_Workflows_WorkflowID",
                table: "OrderDetails");

            migrationBuilder.DropTable(
                name: "OrderDetailsStatusLogs");

            migrationBuilder.DropTable(
                name: "WorkflowOrderDetailsLogs");

            migrationBuilder.DropTable(
                name: "Status");

            migrationBuilder.DropTable(
                name: "Workflows");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_StatusID",
                table: "OrderDetails");

            migrationBuilder.DropIndex(
                name: "IX_OrderDetails_WorkflowID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "StatusID",
                table: "OrderDetails");

            migrationBuilder.DropColumn(
                name: "WorkflowID",
                table: "OrderDetails");

            migrationBuilder.AlterColumn<string>(
                name: "SystemUserCreate",
                table: "OrderDetails",
                type: "text",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CanceledByUserID",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DTsUpdate",
                table: "OrderDetails",
                type: "timestamp without time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DeletedByUserID",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "Enabled",
                table: "OrderDetails",
                type: "boolean",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "LayerID",
                table: "OrderDetails",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SystemUserUpdate",
                table: "OrderDetails",
                type: "text",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Layers",
                columns: table => new
                {
                    LayerID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Description = table.Column<string>(type: "text", nullable: true),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Layers", x => x.LayerID);
                });

            migrationBuilder.CreateIndex(
                name: "IX_OrderDetails_LayerID",
                table: "OrderDetails",
                column: "LayerID");

            migrationBuilder.AddForeignKey(
                name: "FK_OrderDetails_Layers_LayerID",
                table: "OrderDetails",
                column: "LayerID",
                principalTable: "Layers",
                principalColumn: "LayerID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
