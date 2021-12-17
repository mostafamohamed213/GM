using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class userlogs : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UsersLogs",
                columns: table => new
                {
                    UsersLogsID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserIP = table.Column<string>(type: "text", nullable: true),
                    UserCity = table.Column<string>(type: "text", nullable: true),
                    UserRegion = table.Column<string>(type: "text", nullable: true),
                    CreateDts = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    UserID = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UsersLogs", x => x.UsersLogsID);
                    table.ForeignKey(
                        name: "FK_UsersLogs_AspNetUsers_UserID",
                        column: x => x.UserID,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UsersLogs_UserID",
                table: "UsersLogs",
                column: "UserID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UsersLogs");
        }
    }
}
