using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class AddProjectMember2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Projects_ProjectMembers_ProjectTeamId1",
                table: "Projects");

            migrationBuilder.DropIndex(
                name: "IX_Projects_ProjectTeamId1",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTeamId",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "ProjectTeamId1",
                table: "Projects");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProjectTeamId",
                table: "Projects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProjectTeamId1",
                table: "Projects",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectTeamId1",
                table: "Projects",
                column: "ProjectTeamId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_ProjectMembers_ProjectTeamId1",
                table: "Projects",
                column: "ProjectTeamId1",
                principalTable: "ProjectMembers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
