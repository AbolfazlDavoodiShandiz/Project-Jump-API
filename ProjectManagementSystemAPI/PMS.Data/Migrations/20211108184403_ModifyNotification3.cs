using Microsoft.EntityFrameworkCore.Migrations;

namespace PMS.Data.Migrations
{
    public partial class ModifyNotification3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RelatedObjectName",
                table: "Notifications",
                newName: "RelatedObjectTitle");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "RelatedObjectTitle",
                table: "Notifications",
                newName: "RelatedObjectName");
        }
    }
}
