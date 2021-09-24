using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class AddCompleteDateToTasks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CompleteDate",
                table: "ProjectTasks",
                type: "datetime2",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CompleteDate",
                table: "ProjectTasks");
        }
    }
}
