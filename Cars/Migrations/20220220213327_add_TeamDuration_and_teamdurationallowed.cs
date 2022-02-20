using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace Cars.Migrations
{
    public partial class add_TeamDuration_and_teamdurationallowed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TeamDurations",
                columns: table => new
                {
                    TeamDurationID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Roleid = table.Column<string>(type: "text", nullable: true),
                    Duration = table.Column<double>(type: "double precision", nullable: false),
                    isAssigned = table.Column<bool>(type: "boolean", nullable: false),
                    Userid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamDurations", x => x.TeamDurationID);
                    table.ForeignKey(
                        name: "FK_TeamDurations_AspNetRoles_Roleid",
                        column: x => x.Roleid,
                        principalTable: "AspNetRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TeamMemberAlloweds",
                columns: table => new
                {
                    ID = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    TeamDurationID = table.Column<int>(type: "integer", nullable: false),
                    isAssigned = table.Column<bool>(type: "boolean", nullable: false),
                    Roleid = table.Column<string>(type: "text", nullable: true),
                    Userid = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeamMemberAlloweds", x => x.ID);
                    table.ForeignKey(
                        name: "FK_TeamMemberAlloweds_TeamDurations_TeamDurationID",
                        column: x => x.TeamDurationID,
                        principalTable: "TeamDurations",
                        principalColumn: "TeamDurationID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TeamDurations_Roleid",
                table: "TeamDurations",
                column: "Roleid");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberAlloweds_TeamDurationID",
                table: "TeamMemberAlloweds",
                column: "TeamDurationID");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TeamMemberAlloweds");

            migrationBuilder.DropTable(
                name: "TeamDurations");
        }
    }
}
