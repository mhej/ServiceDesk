using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ServiceDesk.Data.Migrations
{
    public partial class Images : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Requests");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Requests",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Image",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Image", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ImagesAssignedToRequest",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    RequestId = table.Column<int>(nullable: true),
                    ImageId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ImagesAssignedToRequest", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ImagesAssignedToRequest_Image_ImageId",
                        column: x => x.ImageId,
                        principalTable: "Image",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ImagesAssignedToRequest_Requests_RequestId",
                        column: x => x.RequestId,
                        principalTable: "Requests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Requests_ImageId",
                table: "Requests",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagesAssignedToRequest_ImageId",
                table: "ImagesAssignedToRequest",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_ImagesAssignedToRequest_RequestId",
                table: "ImagesAssignedToRequest",
                column: "RequestId");

            migrationBuilder.AddForeignKey(
                name: "FK_Requests_Image_ImageId",
                table: "Requests",
                column: "ImageId",
                principalTable: "Image",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Requests_Image_ImageId",
                table: "Requests");

            migrationBuilder.DropTable(
                name: "ImagesAssignedToRequest");

            migrationBuilder.DropTable(
                name: "Image");

            migrationBuilder.DropIndex(
                name: "IX_Requests_ImageId",
                table: "Requests");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Requests");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Requests",
                nullable: true);
        }
    }
}
