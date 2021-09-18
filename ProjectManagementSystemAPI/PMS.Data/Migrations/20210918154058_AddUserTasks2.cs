using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class AddUserTasks2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_ProjectTasks_ProjectTaskId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_ProjectTaskId",
                table: "UserTasks");

            migrationBuilder.DropColumn(
                name: "ProjectTaskId",
                table: "UserTasks");

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_TaskId",
                table: "UserTasks",
                column: "TaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_ProjectTasks_TaskId",
                table: "UserTasks",
                column: "TaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserTasks_ProjectTasks_TaskId",
                table: "UserTasks");

            migrationBuilder.DropIndex(
                name: "IX_UserTasks_TaskId",
                table: "UserTasks");

            migrationBuilder.AddColumn<int>(
                name: "ProjectTaskId",
                table: "UserTasks",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserTasks_ProjectTaskId",
                table: "UserTasks",
                column: "ProjectTaskId");

            migrationBuilder.AddForeignKey(
                name: "FK_UserTasks_ProjectTasks_ProjectTaskId",
                table: "UserTasks",
                column: "ProjectTaskId",
                principalTable: "ProjectTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
