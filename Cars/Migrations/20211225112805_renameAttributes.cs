using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class renameAttributes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "WorkflowOrderDetailsLogs",
                newName: "Details");

            migrationBuilder.RenameColumn(
                name: "OrderDetailsStatusLogLogID",
                table: "OrderDetailsStatusLogs",
                newName: "OrderDetailsStatusLogID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Details",
                table: "WorkflowOrderDetailsLogs",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "OrderDetailsStatusLogID",
                table: "OrderDetailsStatusLogs",
                newName: "OrderDetailsStatusLogLogID");
        }
    }
}
