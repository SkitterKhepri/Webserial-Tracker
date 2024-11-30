using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WT_API.Migrations
{
    public partial class robust : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "otherNextChLinkXPaths",
                table: "Serials",
                newName: "otherNextChLinkXPathsJSONString");

            migrationBuilder.RenameColumn(
                name: "otherNextChLinkXPaths",
                table: "Chapters",
                newName: "otherNextChLinkXPathsJSONString");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "otherNextChLinkXPathsJSONString",
                table: "Serials",
                newName: "otherNextChLinkXPaths");

            migrationBuilder.RenameColumn(
                name: "otherNextChLinkXPathsJSONString",
                table: "Chapters",
                newName: "otherNextChLinkXPaths");
        }
    }
}
