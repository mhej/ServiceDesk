using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceDesk.Data.Migrations
{
    public partial class NoteAsDbSet : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RequestNotes",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Content = table.Column<string>(nullable: true),
                    Date = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RequestNotes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "NotesAssignedToRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestId = table.Column<int>(nullable: false),
                    RequestNoteId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotesAssignedToRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_NotesAssignedToRequest_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_NotesAssignedToRequest_RequestNotes_RequestNoteId",
                        column: x => x.RequestNoteId,
                        principalTable: "RequestNotes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_NotesAssignedToRequest_RequestId",
                table: "NotesAssignedToRequest",
                column: "RequestId");

            migrationBuilder.CreateIndex(
                name: "IX_NotesAssignedToRequest_RequestNoteId",
                table: "NotesAssignedToRequest",
                column: "RequestNoteId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotesAssignedToRequest");

            migrationBuilder.DropTable(
                name: "RequestNotes");
        }
    }
}
