using Microsoft.EntityFrameworkCore.Migrations;

namespace Cars.Migrations
{
    public partial class ModifyTeamDuration2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_TeamMemberAlloweds_TeamMemberAllowedID",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_TeamMemberAllowedID",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "TeamMemberAllowedID",
                table: "AspNetUsers");

            migrationBuilder.CreateIndex(
                name: "IX_TeamMemberAlloweds_Userid",
                table: "TeamMemberAlloweds",
                column: "Userid");

            migrationBuilder.AddForeignKey(
                name: "FK_TeamMemberAlloweds_AspNetUsers_Userid",
                table: "TeamMemberAlloweds",
                column: "Userid",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TeamMemberAlloweds_AspNetUsers_Userid",
                table: "TeamMemberAlloweds");

            migrationBuilder.DropIndex(
                name: "IX_TeamMemberAlloweds_Userid",
                table: "TeamMemberAlloweds");

            migrationBuilder.AddColumn<int>(
                name: "TeamMemberAllowedID",
                table: "AspNetUsers",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_TeamMemberAllowedID",
                table: "AspNetUsers",
                column: "TeamMemberAllowedID");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_TeamMemberAlloweds_TeamMemberAllowedID",
                table: "AspNetUsers",
                column: "TeamMemberAllowedID",
                principalTable: "TeamMemberAlloweds",
                principalColumn: "ID",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
