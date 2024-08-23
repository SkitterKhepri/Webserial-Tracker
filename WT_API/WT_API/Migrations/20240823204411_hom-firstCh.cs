using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WT_API.Migrations
{
    public partial class homfirstCh : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "link",
                table: "Serials",
                newName: "home");

            migrationBuilder.AddColumn<string>(
                name: "firstCh",
                table: "Serials",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "link",
                table: "Chapters",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "firstCh",
                table: "Serials");

            migrationBuilder.DropColumn(
                name: "link",
                table: "Chapters");

            migrationBuilder.RenameColumn(
                name: "home",
                table: "Serials",
                newName: "link");
        }
    }
}
