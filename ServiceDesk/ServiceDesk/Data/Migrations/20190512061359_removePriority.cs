using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceDesk.Data.Migrations
{
    public partial class removePriority : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Priority",
                table: "Requests");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "Requests",
                nullable: false,
                defaultValue: 0);
        }
    }
}
