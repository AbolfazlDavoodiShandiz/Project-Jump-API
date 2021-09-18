using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class AddOwnerToProjectTask : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OwnerId",
                table: "ProjectTasks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTasks_OwnerId",
                table: "ProjectTasks",
                column: "OwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_OwnerId",
                table: "ProjectTasks",
                column: "OwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProjectTasks_AspNetUsers_OwnerId",
                table: "ProjectTasks");

            migrationBuilder.DropIndex(
                name: "IX_ProjectTasks_OwnerId",
                table: "ProjectTasks");

            migrationBuilder.DropColumn(
                name: "OwnerId",
                table: "ProjectTasks");
        }
    }
}
