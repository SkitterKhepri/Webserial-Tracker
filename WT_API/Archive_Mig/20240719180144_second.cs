using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WT_API.Migrations
{
    public partial class second : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "bookId",
                table: "Chapters",
                newName: "arcId");

            migrationBuilder.AddColumn<string>(
                name: "otherNextChLinkXPaths",
                table: "Serials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "secondaryNextChLinkXPath",
                table: "Serials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "otherNextChLinkXPaths",
                table: "Serials");

            migrationBuilder.DropColumn(
                name: "secondaryNextChLinkXPath",
                table: "Serials");

            migrationBuilder.RenameColumn(
                name: "arcId",
                table: "Chapters",
                newName: "bookId");
        }
    }
}
