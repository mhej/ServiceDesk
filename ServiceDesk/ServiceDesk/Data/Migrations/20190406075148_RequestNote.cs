using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceDesk.Data.Migrations
{
    public partial class RequestNote : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Status",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AssigneeId",
                table: "Requests",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "RequestorId",
                table: "Requests",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Requests_AssigneeId",
                table: "Requests",
                column: "AssigneeId");

            migrationBuilder.CreateIndex(
                name: "IX_Requests_RequestorId",
                table: "Requests",
                column: "RequestorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_AssigneeId",
                table: "Requests",
                column: "AssigneeId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_AspNetUsers_RequestorId",
                table: "Requests",
                column: "RequestorId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_AssigneeId",
                table: "Requests");

            migrationBuilder.DropForeignKey(
                name: "FK_Requests_AspNetUsers_RequestorId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_AssigneeId",
                table: "Requests");

            migrationBuilder.DropIndex(
                name: "IX_Requests_RequestorId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "AssigneeId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "RequestorId",
                table: "Requests");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(int));
        }
    }
}
