using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceDesk.Data.Migrations
{
    public partial class AdminAssignedToTeamCorrectColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminAssignedToTeam_AspNetUsers_ClassificationId",
                table: "AdminAssignedToTeam");

            migrationBuilder.DropIndex(
                name: "IX_AdminAssignedToTeam_ClassificationId",
                table: "AdminAssignedToTeam");

            migrationBuilder.DropColumn(
                name: "ClassificationId",
                table: "AdminAssignedToTeam");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "AdminAssignedToTeam",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminAssignedToTeam_ApplicationUserId",
                table: "AdminAssignedToTeam",
                column: "ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminAssignedToTeam_AspNetUsers_ApplicationUserId",
                table: "AdminAssignedToTeam",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AdminAssignedToTeam_AspNetUsers_ApplicationUserId",
                table: "AdminAssignedToTeam");

            migrationBuilder.DropIndex(
                name: "IX_AdminAssignedToTeam_ApplicationUserId",
                table: "AdminAssignedToTeam");

            migrationBuilder.AlterColumn<string>(
                name: "ApplicationUserId",
                table: "AdminAssignedToTeam",
                nullable: true,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ClassificationId",
                table: "AdminAssignedToTeam",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AdminAssignedToTeam_ClassificationId",
                table: "AdminAssignedToTeam",
                column: "ClassificationId");

            migrationBuilder.AddForeignKey(
                name: "FK_AdminAssignedToTeam_AspNetUsers_ClassificationId",
                table: "AdminAssignedToTeam",
                column: "ClassificationId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
