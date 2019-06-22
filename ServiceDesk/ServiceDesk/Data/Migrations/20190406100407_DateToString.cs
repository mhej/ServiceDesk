using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceDesk.Data.Migrations
{
    public partial class DateToString : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SubmitDate",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(DateTime));

            migrationBuilder.AlterColumn<string>(
                name: "ClosedDate",
                table: "Requests",
                nullable: true,
                oldClrType: typeof(DateTime));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "SubmitDate",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ClosedDate",
                table: "Requests",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }
    }
}
