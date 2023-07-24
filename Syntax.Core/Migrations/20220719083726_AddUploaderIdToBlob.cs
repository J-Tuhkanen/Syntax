using Microsoft.EntityFrameworkCore.Migrations;

namespace Syntax.Core.Migrations
{
    public partial class AddUploaderIdToBlob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Blobs",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Blobs");
        }
    }
}
