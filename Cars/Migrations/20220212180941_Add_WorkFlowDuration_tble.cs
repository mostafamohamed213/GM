using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class Add_WorkFlowDuration_tble : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "WorkFlowDuration",
                columns: table => new
                {
                    WorkFlowDurationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    WorkflowID = table.Column<int>(type: "integer", nullable: false),
                    Duration = table.Column<double>(type: "double precision", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WorkFlowDuration", x => x.WorkFlowDurationID);
                    table.ForeignKey(
                        name: "FK_WorkFlowDuration_Workflows_WorkflowID",
                        column: x => x.WorkflowID,
                        principalTable: "Workflows",
                        principalColumn: "WorkflowID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_WorkFlowDuration_WorkflowID",
                table: "WorkFlowDuration",
                column: "WorkflowID",
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "WorkFlowDuration");
        }
    }
}
